using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using DXGame.Core.Components.Advanced.Impulse;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Map;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using MathNet.Numerics;
using NLog;

namespace DXGame.Core.Models
{
    public delegate DxVector2 DisplacementFunction(TimeSpan offset);

    public delegate float PositionalFunction(float x);

    /**

        <summary>
            Describes a link between two Nodes on a NavigableSurface.
            A Path is an ordered list of Commandments. The time between each commandment is assumed to be 
            constant and defined by whatever our simulation uses as a constant time frame (ie, 16ms)
        </summary>
    */

    internal sealed class Path
    {
        public TimeSpan Time { get; } // TODO
        public Commandment[] Directions { get; }
        public NavigableSurface.Node End { get; }

        private Path(Commandment[] directions, TimeSpan time, NavigableSurface.Node end)
        {
            Directions = directions;
            Time = time;
            End = end;
        }
        public static Path From(Commandment[] commandmentChain, TimeSpan time, NavigableSurface.Node end)
        {
            return new Path(commandmentChain, time, end);
        }
    }

    /**
        <summary>
            Maintains the known Paths between NavigableSurfaceNodes. This is constructed on-the-fly in 
            order to do A* pathfinding (where we do not know the directional links between Nodes, but can find them)
        </summary>
    */

    internal sealed class ExplorableMesh
    {
        private readonly Dictionary<NavigableSurface.Node, HashSet<Commandment[]>> exhaustedCommandments_ =
            new Dictionary<NavigableSurface.Node, HashSet<Commandment[]>>();

        private readonly Dictionary<NavigableSurface.Node, HashSet<Path>> paths_ =
            new Dictionary<NavigableSurface.Node, HashSet<Path>>();

        public IEnumerable<NavigableSurface.Node> Nodes => paths_.Keys;
        public NavigableSurface NavigableSurface { get; }

        public HashSet<NavigableSurface.Node> Exhausted  { get; }
        
        public ExplorableMesh(NavigableSurface surface)
        {
            Validate.IsNotNullOrDefault(surface);
            foreach (var node in surface.NodeQuery.Elements)
            {
                paths_[node] = new HashSet<Path>();
                exhaustedCommandments_[node] = new HashSet<Commandment[]>();
            }
            NavigableSurface = surface;
            Exhausted = new HashSet<NavigableSurface.Node>();
        }

        /**
            <summary>
                "Notifies" the mesh that there is a new path available (explored), attaching it and updating the appropriate Nodes
            </summary>
        */

        public void AttachPath(NavigableSurface.Node start, Path path)
        {
            Validate.IsNotNull(start);
            Validate.IsNotNull(path);
            // Don't loop to yourself
            if (!Objects.Equals(start, path.End))
            {
                ExhaustCommandment(start, path.Directions);
                paths_[start].Add(path);
            }
        }

        public void ExhaustCommandment(NavigableSurface.Node start, params Commandment[] exhausted)
        {
            exhaustedCommandments_[start].Add(exhausted);
        }

        /**
            <summary>
                Retrieves all known Paths for the provided node.
                Note: The returned List will never be null, only (potentially) empty
            </summary>
        */

        public List<Path> PathsFrom(NavigableSurface.Node start)
        {
            Validate.IsNotNull(start);
            return paths_[start].ToList();
        }

        /**

            <summary>
                Retrieves all of the Commandments that have already been attempted for paths that start at the specified Node.
                This is useful to mark Nodes as "Exhausted"
            </summary>
        */

        public List<Commandment[]> AvailableCommandments(NavigableSurface.Node start)
        {
            HashSet<Commandment[]> exhausted = exhaustedCommandments_[start];
            List<Commandment[]> result;
            if (exhausted.Any())
            {
                result = PathfindingModel.AVAILABLE_COMMANDMENTS.Except(exhausted).ToList();
            } else {
                result = PathfindingModel.AVAILABLE_COMMANDMENTS.ToList();
            }

            result.Sort((first, second) => first.Length.CompareTo(second.Length));
            return result;
        }
    }

    internal sealed class ExplorableMeshCache
    {
        private static readonly ThreadLocal<Dictionary<ImmutablePair<UniqueId, NavigableSurface>, ExplorableMesh>> CACHE
            =
            new ThreadLocal<Dictionary<ImmutablePair<UniqueId, NavigableSurface>, ExplorableMesh>>(
                () => new Dictionary<ImmutablePair<UniqueId, NavigableSurface>, ExplorableMesh>());

        public static ExplorableMesh MeshFor(GameObject entity, NavigableSurface surface)
        {
            Validate.IsNotNullOrDefault(entity,
                $"Cannot retrieve an {typeof (ExplorableMesh)} for a null {nameof(entity)}");
            Validate.IsNotNullOrDefault(surface,
                $"Cannot retrieve an {typeof (ExplorableMesh)} for a null {surface.GetType()}");
            return PopulateOrRetrieveMesh(entity, surface);
        }

        private static ExplorableMesh PopulateOrRetrieveMesh(GameObject entity, NavigableSurface surface)
        {
            var cachedNavigationMesh = CACHE.Value;
            var entityId = entity.Id;
            var key = new ImmutablePair<UniqueId, NavigableSurface>(entityId, surface);
            if (cachedNavigationMesh.ContainsKey(key))
            {
                return cachedNavigationMesh[key];
            }

            var newExplorableMesh = new ExplorableMesh(surface);
            cachedNavigationMesh[key] = newExplorableMesh;
            return newExplorableMesh;
        }
    }

    [Serializable]
    [DataContract]
    public class PathfindingResult
    {
        public LinkedList<ImmutablePair<TimeSpan, Commandment[]>> Path { get; }

        public LinkedList<DxVector2> WayPoints { get; }

        public PathfindingResult(LinkedList<ImmutablePair<TimeSpan, Commandment[]>> path, LinkedList<DxVector2> waypoints)
        {
            Validate.IsNotNull(path, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(path)));
            Validate.IsNotNull(waypoints, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(waypoints)));
            Path = path;
            WayPoints = waypoints;
        }
    }

    [Serializable]
    [DataContract]
    public class PathfindingModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        internal static readonly int SIMULATION_UPPER_BOUND = 10;
        internal static readonly double SIMULATION_STEP = 0.01;
        internal static readonly TimeSpan MAX_SIMULATION_TIME = TimeSpan.FromSeconds(SIMULATION_UPPER_BOUND);
        internal static readonly TimeSpan SIMULATION_TIME_STEP = TimeSpan.FromSeconds(SIMULATION_STEP);

        internal static readonly ReadOnlyCollection<Commandment[]> AVAILABLE_COMMANDMENTS =
            new ReadOnlyCollection<Commandment[]>(new List<Commandment[]>
            {
                new [] { Commandment.None },
                new[] {Commandment.MoveUp},
                new[] {Commandment.MoveLeft},
                new[] {Commandment.MoveRight},
                new[] {Commandment.MoveDown},
                new[] {Commandment.MoveUp, Commandment.MoveLeft},
                new[] {Commandment.MoveUp, Commandment.MoveRight},
                new[] {Commandment.MoveDown, Commandment.MoveLeft},
                new[] {Commandment.MoveDown, Commandment.MoveRight}
            });

        // Only budget a partial frame for this shit
        private static readonly TimeSpan EXPLORATION_TIMEOUT = TimeSpan.FromSeconds(0.005);

        private static PathfindingResult EmptyPath => new PathfindingResult(new LinkedList<ImmutablePair<TimeSpan, Commandment[]>>(), new LinkedList<DxVector2>());

        public PathfindingResult Pathfind(GameObject entity, DxVector2 target)
        {
            var entityIsNull = Check.IsNullOrDefault(entity);
            if (entityIsNull)
            {
                LOG.Info("Pathfinding called with null entity");
            }

            /* If the provided entity is null or we're already at our destination, there's nothing to do */
            if (entityIsNull || entity.ComponentOfType<SpatialComponent>().Space.Contains(target))
            {
                LOG.Warn($"No work to do - we're either null or at our destination");
                return EmptyPath;
            }
            
            var commandmentQueue = new Queue<Commandment>();
            commandmentQueue.Enqueue(Commandment.None);

            var mapModel = DxGame.Instance.Model<MapModel>();
            var navigationMesh = NavigableSurface.SurfaceFor(mapModel);
            var closestNode = navigationMesh.NodeQuery.Closest(target);
            if (!closestNode.HasValue)
            {
                LOG.Info($"Could not find a Node close to {target}");
                return EmptyPath;
            }

            var targetNode = closestNode.Value;
            SpatialComponent spatial = entity.ComponentOfType<SpatialComponent>();

            Optional<NavigableSurface.Node> maybeOriginalNode =
                navigationMesh.NodeQuery.Closest(new DxVector2(spatial.Space.X, spatial.Space.Y + spatial.Space.Height - 0.01f));
            if (!maybeOriginalNode.HasValue)
            {
                LOG.Warn($"Could not determine the closest node to {spatial.Space}");
                return EmptyPath;
            }
            NavigableSurface.Node originalNode = maybeOriginalNode.Value;
            NavigableSurface.Node current = originalNode;

            var explorableMesh = ExplorableMeshCache.MeshFor(entity, navigationMesh);
            var displacementFunctions = DetermineDisplacementFunction(entity);

            Explore(entity, targetNode, explorableMesh, displacementFunctions);

            HashSet<NavigableSurface.Node> exhausted = new HashSet<NavigableSurface.Node>();
            SortedSet < NavigableSurface.Node > available = new SortedSet<NavigableSurface.Node>(Comparer<NavigableSurface.Node>.Create(
                    (first, second) =>
                        (targetNode.Position - first.Position).MagnitudeSquared.CompareTo(
                            (targetNode.Position - second.Position).MagnitudeSquared)
                    )) { current };

            Dictionary<NavigableSurface.Node, NavigableSurface.Node> traveled =
                new Dictionary<NavigableSurface.Node, NavigableSurface.Node>();

            while (current != targetNode)
            {
                if (!available.Any())
                {
                    LOG.Warn($"Could not find path between {spatial.Space} and {target}");
                    break;
                }
                NavigableSurface.Node last = current;
                current = available.Min;
                traveled[last] = current;
                available.Remove(current);
                exhausted.Add(current);
                List<Path> paths = explorableMesh.PathsFrom(current);
                foreach (Path path in paths)
                {
                    NavigableSurface.Node end = path.End;
                    if (!exhausted.Contains(end))
                    {
                        available.Add(end);
                    }
                }
            }

            PathfindingResult result = ReconstructPath(explorableMesh, originalNode,
                targetNode, traveled);
            return result;
        }

        private static PathfindingResult ReconstructPath(ExplorableMesh mesh, NavigableSurface.Node start, NavigableSurface.Node end, Dictionary<NavigableSurface.Node, NavigableSurface.Node> traveled)
        {
            
            NavigableSurface.Node current = start;
            LinkedList<ImmutablePair<TimeSpan, Commandment []>> directions =
                new LinkedList<ImmutablePair<TimeSpan, Commandment[]>>();
            LinkedList<DxVector2> waypoints = new LinkedList<DxVector2>();
            waypoints.AddLast(current.Position);
            while (current != end)
            {
                List<Path> paths = mesh.PathsFrom(current);
                if (!traveled.ContainsKey(current))
                {
                    LOG.Warn($"Could not properly reconstruct the path (no link between {current} and {end}. Bailing early.");
                    break;
                }
                NavigableSurface.Node nextStep = traveled[current];
                Path correctPath = paths.FirstOrDefault(path => path.End == nextStep);
                if (correctPath == null)
                {
                    // weird
                    LOG.Warn($"Could not properly reconstruct the path (no link between {current} and {nextStep}. Bailing early.");
                    break;
                }
                directions.AddLast(new ImmutablePair<TimeSpan, Commandment[]>(correctPath.Time, correctPath.Directions));
                current = nextStep;
                waypoints.AddLast(current.Position);
            }
            return new PathfindingResult(directions, waypoints);
        }

        private static void ReachInitialNode(GameObject entity, LinkedList<ImmutablePair<TimeSpan, Commandment[]>> directions, NavigableSurface.Node start )
        {
            // TODO

        }

        private void Explore(GameObject entity, NavigableSurface.Node target, ExplorableMesh mesh,
            Dictionary<Commandment[], SimplePolynomial> displacementFunctions)
        {
            SpatialComponent spatial = entity.ComponentOfType<SpatialComponent>();
            DxVector2 space = spatial.Dimensions;
            DxVector2 position = spatial.Position;
            position.Y += space.Y - 0.01f;

            // TODO: Fix this and go to closest node instead of "starting" there
            Optional<NavigableSurface.Node> maybeStartingPoint = mesh.NavigableSurface.NodeQuery.Closest(position);
            if (!maybeStartingPoint.HasValue)
            {
                LOG.Warn($"Could not explore the mesh - there is no point near {position}");
                return;
            }

            NavigableSurface.Node startingPoint = maybeStartingPoint.Value;
            NavigableSurface.Node current = startingPoint;
            TimeSpan maxTime = TimeSpan.FromSeconds(0.016);
            Stopwatch timer = Stopwatch.StartNew();
            SortedSet<NavigableSurface.Node> available =new SortedSet<NavigableSurface.Node>(Comparer<NavigableSurface.Node>.Create(
                    (first, second) =>
                        (target.Position - first.Position).MagnitudeSquared.CompareTo(
                            (target.Position - second.Position).MagnitudeSquared)
                    )) {current};

            while (timer.Elapsed < maxTime)
            {
                mesh.Exhausted.Add(current);
                if (!available.Any())
                {
                    break;
                }
                current = available.Min;
                available.Remove(current);

                List<Commandment[]> availableCommandments = mesh.AvailableCommandments(current);
                foreach (var entry in displacementFunctions)
                {
                    Commandment[] commandChain = entry.Key;
                    if (!availableCommandments.Contains(commandChain))
                    {
                        continue;
                    }
                    SimplePolynomial polynomial = entry.Value;
                    DxRectangle bounds = polynomial.Bounds;
                    bounds.Height += space.Y;
                    bounds.X += current.Position.X;
                    bounds.Y += current.Position.Y;
                    DxRectangle searchSpace = FudgeBounds(bounds);
                    List<NavigableSurface.Node> maybeReachableNodes =
                        mesh.NavigableSurface.NodeQuery.InRange(searchSpace);
                    if (!maybeReachableNodes.Any())
                    {
                        // No nodes? Truck on!
                        continue;
                    }
                    if (maybeReachableNodes.Count == 1 && maybeReachableNodes.Contains(current))
                    {
                        // Only ourself? Truck on!
                        continue;
                    }

                    foreach (NavigableSurface.Node node in maybeReachableNodes)
                    {
                        DxVector2 displacement = node.Position - current.Position;
                        if (displacement.Y <= 0 && commandChain.Contains(Commandment.MoveDown))
                        {
                            continue;
                        }
                        double y = polynomial.PositionalDisplacement(displacement.X);
                        /* Should pretty much always be false */
                        if (!FudgeBounds(polynomial.Bounds).Contains(new DxVector2(displacement.X, y)))
                        {
                            continue;
                        }
                        /* Hey, maybe we can reach! */
                        TimeSpan time = polynomial.TimeFor(displacement.X);
                        /* One last sanity check... */
                        if (time <= MAX_SIMULATION_TIME  /*&& (node.Position.Y.FuzzyCompare((float)(current.Position.Y + y), 1.0f) == 0) */)
                        {
                            if (!mesh.Exhausted.Contains(node))
                            {
                                available.Add(node);
                            }
                            /* We're GOOD BOYS */
                            Path path = Path.From(commandChain, time, node);
                            // TODO: We currently don't handle gravity (woops), so we need to add some shit in here to deal with drops between platforms
                            mesh.AttachPath(current, path);
                        }
                    }
                    mesh.ExhaustCommandment(current, commandChain);
                }
            }
        }

        private static DxRectangle FudgeBounds(DxRectangle bounds)
        {
            const float offset = 0.01f;
            bounds.X -= offset;
            bounds.Y -= offset;
            bounds.Width += offset * 2;
            bounds.Height += offset * 2;
            return bounds;
        }

        private static DxRectangle DetermineMaximumBounds(IEnumerable<SimplePolynomial> movements)
        {
            float xMin = float.MaxValue;
            float xMax = float.MinValue;
            float yMin = float.MaxValue;
            float yMax = float.MinValue;
            foreach (SimplePolynomial polynomial in movements)
            {
                DxRectangle bounds = polynomial.Bounds;
                xMin = Math.Min(bounds.X, xMin);
                xMax = Math.Max(bounds.Right, xMax);
                yMin = Math.Min(bounds.Y, yMin);
                yMax = Math.Max(bounds.Bottom, yMax);
            }

            return new DxRectangle(xMin, yMin, (xMax - xMin), (yMax - yMin));
        }

        private static Dictionary<Commandment[], SimplePolynomial>
            DetermineDisplacementFunction(GameObject entity)
        {
            var commendmentCombinations = AVAILABLE_COMMANDMENTS;
            var displacementFunctionsByCommandment = new Dictionary<Commandment[], SimplePolynomial>();

            var properties = entity.ComponentOfType<EntityPropertiesComponent>();

            foreach (var commandmentChain in commendmentCombinations)
            {
                SimplePolynomial polynomial = RegressForces(commandmentChain, properties);
                displacementFunctionsByCommandment[commandmentChain] = polynomial;
            }

            return displacementFunctionsByCommandment;
        }

        private static SimplePolynomial RegressForces(Commandment [] commandChain, EntityPropertiesComponent properties)
        {
            var forcesAsList = commandChain.Select(properties.MovementForceFor).ToList();
            bool isVerticalForce = forcesAsList.Count == 2;
            Func<Force> regenerateForce = () => Force.NullForce;
            // TODO Cleanup wtf is this specific ordering bullshit of arrays fuck this sucks
            if (isVerticalForce)
            {
                forcesAsList.Add(WorldForces.Gravity);
                regenerateForce = () => properties.MovementForceFor(commandChain[1]);
            }
            else
            {
                Commandment potentialConstantCommandment = commandChain[0];
                if (potentialConstantCommandment != Commandment.MoveUp && potentialConstantCommandment != Commandment.MoveDown)
                {
                    regenerateForce = () => properties.MovementForceFor(commandChain[0]);
                }
                else if(potentialConstantCommandment != Commandment.None)
                {
                    forcesAsList.Add(WorldForces.Gravity);
                }
            }
            /* We should have a single directional force - this should be re-applied every frame, as it will dissipate in one frame! */

            forcesAsList.Add(WorldForces.AirResistance);
            var position = DxVector2.EmptyVector;
            var velocity = forcesAsList.Aggregate(DxVector2.EmptyVector,
                (current, force) => current + force.InitialVelocity);
            var gameTime = new DxGameTime(TimeSpan.Zero, TimeSpan.Zero, false);
            var timeList = new List<double>((int) ((SIMULATION_UPPER_BOUND + 1) / SIMULATION_STEP));
            var xPositions = new List<double>((int) ((SIMULATION_UPPER_BOUND + 1) / SIMULATION_STEP));
            var yPositions = new List<double>((int) ((SIMULATION_UPPER_BOUND + 1) / SIMULATION_STEP));
            for (double i = 0; i < SIMULATION_UPPER_BOUND; i += SIMULATION_STEP)
            {
                var newPositionAndVelocity = PhysicsComponent.ForceComputation(gameTime, position, velocity,
                    forcesAsList);
                position = newPositionAndVelocity.Item1;
                velocity = newPositionAndVelocity.Item2;
                forcesAsList.RemoveAll(force => force.Dissipated);
                Force newForce = regenerateForce();
                forcesAsList.Add(newForce);
                velocity += newForce.InitialVelocity;
                
                gameTime = new DxGameTime(gameTime.TotalGameTime + SIMULATION_TIME_STEP, SIMULATION_TIME_STEP, false);
                timeList.Add(i);
                xPositions.Add(position.X);
                yPositions.Add(position.Y);
            }
            var times = timeList.ToArray();
            double xMin = xPositions.Min();
            double xMax = xPositions.Max();
            double yMin = yPositions.Min();
            double yMax = yPositions.Max();
            DxRectangle bounds = new DxRectangle(xMin, yMin, (xMax - xMin), (yMax - yMin));
            double[] x = xPositions.ToArray();
            SanitizeXValues(x);
            double[] y = yPositions.ToArray();
            const int polynomialOrder = 2;
            double[] xRegression = Fit.Polynomial(times, x, polynomialOrder);
            double[] yRegression = Fit.Polynomial(times, y, polynomialOrder);
            double[] positionalRegression = Fit.Polynomial(x, y, polynomialOrder);
            double[] xToTimeRegression = Fit.Polynomial(x, times, polynomialOrder);
            return new SimplePolynomial(xRegression, yRegression, positionalRegression, xToTimeRegression, bounds);
        }

        private static double[][] Combine(double[] timeSeries, double[] xValues)
        {
            double[][] result = new double[timeSeries.Length][];
            for (int i = 0; i < timeSeries.Length; ++i)
            {
                result[i] = new double[2];
                result[i][0] = timeSeries[i];
                result[i][1] = xValues[i];
            }
            return result;
        }

        private static void SanitizeXValues(double[] xValues)
        {
            double x = xValues[0];
            if (xValues.Any(xValue => x != xValue))
            {
                return;
            }
            /* Pad stuff so we have sane functions :^) */
            const double initial = 0.0001;
            double accumulator = initial;
            for (int i = 0; i < xValues.Length; ++i)
            {
                xValues[i] += accumulator;
                accumulator += initial;
            }
        }
    }

    [Serializable]
    [DataContract]
    internal sealed class SimplePolynomial
    {
        [DataMember]
        private double[] YTerms { get; }

        [DataMember]
        private double[] XTerms { get; }

        [DataMember]
        private double[] PositionalTerms { get; }

        [DataMember]
        private double[] XToTimeRegression { get; }

        [DataMember]
        public DxRectangle Bounds { get; }

        public SimplePolynomial(double[] xRegression, double[] yRegression, double[] positionalRegression,
            double[] xToTimeRegression, DxRectangle bounds)
        {
            Validate.IsNotNullOrDefault(xRegression, $"Cannot create a {GetType()} with a null {nameof(xRegression)}");
            Validate.IsNotNullOrDefault(yRegression, $"Cannot create a {GetType()} with a null {nameof(yRegression)}");
            Validate.IsNotNullOrDefault(positionalRegression,
                $"Cannot create a {GetType()} with a null {nameof(positionalRegression)}");
            Validate.IsNotNullOrDefault(xToTimeRegression,
                $"Cannot create a {GetType()} with a null {nameof(xToTimeRegression)}");
            XTerms = xRegression;
            YTerms = yRegression;
            PositionalTerms = positionalRegression;
            XToTimeRegression = xToTimeRegression;
            Bounds = bounds;
        }

        public TimeSpan TimeFor(double x)
        {
            double timeInMillis = Evaluate.Polynomial(x, XToTimeRegression);
            if (timeInMillis < 0)
            {
                return TimeSpan.FromSeconds(int.MaxValue);
            }
            return TimeSpan.FromSeconds(timeInMillis);
        }

        public double PositionalDisplacement(double x)
        {
            return Evaluate.Polynomial(x, PositionalTerms);
        }

        public DxVector2 TimeDisplacement(TimeSpan offset)
        {
            double time = offset.TotalSeconds;
            float x = (float) Evaluate.Polynomial(time, XTerms);
            if (float.IsNaN(x))
            {
                x = 0;
            }
            float y = (float) Evaluate.Polynomial(time, YTerms);
            if (float.IsNaN(y))
            {
                y = 0;
            }
            return new DxVector2(x, y);
        }
    }
}