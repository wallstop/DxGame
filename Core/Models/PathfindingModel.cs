using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using DXGame.Core.Components.Advanced.Command;
using DXGame.Core.Components.Advanced.Impulse;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.DataStructures;
using DXGame.Core.Map;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Main;
using MathNet.Numerics;
using NLog;

namespace DXGame.Core.Models
{
    public delegate DxVector2 DisplacementFunction(TimeSpan offset);

    /**

        <summary>
            Describes a link between two Nodes on a NavigableSurface.
            A Path is an ordered list of Commandments. The time between each commandment is assumed to be 
            constant and defined by whatever our simulation uses as a constant time frame (ie, 16ms)
        </summary>
    */

    internal sealed class Path
    {
        public List<Commandment> Directions { get; }
        public Optional<Commandment> Beginning { get; }
        public NavigableSurface.Node End { get; }

        private Path(List<Commandment> directions, NavigableSurface.Node end)
        {
            Directions = directions;
            Beginning = directions.Any() ? directions[0] : Optional<Commandment>.Empty;
            End = end;
        }

        public static PathBuilder Builder()
        {
            return new PathBuilder();
        }

        public class PathBuilder
        {
            private readonly List<Commandment> directions_ = new List<Commandment>();

            public void WithStep(Commandment commandment)
            {
                directions_.Add(commandment);
            }

            public Path Build(NavigableSurface.Node end)
            {
                Validate.IsNotNullOrDefault(end);
                return new Path(directions_, end);
            }
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
        private readonly Dictionary<NavigableSurface.Node, HashSet<Path>> paths_ =
            new Dictionary<NavigableSurface.Node, HashSet<Path>>();

        public ExplorableMesh(NavigableSurface mesh)
        {
            Validate.IsNotNullOrDefault(mesh);
            foreach (var node in mesh.NodeQuery.Elements)
            {
                paths_[node] = new HashSet<Path>();
            }
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
                paths_[start].Add(path);
            }
        }

        public List<NavigableSurface.Node> DetermineAvailable(NavigableSurface.Node start)
        {
            var seenNodes = new HashSet<NavigableSurface.Node>();
            var unexplored = new Queue<NavigableSurface.Node>();
            unexplored.Enqueue(start);
            do
            {
                var current = unexplored.Dequeue();
                var paths = PathsFrom(start);
                foreach (var node in paths.Select(path => path.End))
                {
                    if (seenNodes.Add(node))
                    {
                        unexplored.Enqueue(node);
                    }
                }
            } while (unexplored.Any());

            seenNodes.Remove(start);
            return seenNodes.ToList();
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

        public HashSet<Commandment> AttemptedCommandments(NavigableSurface.Node start)
        {
            return
                new HashSet<Commandment>(
                    paths_[start] /*.Where(path => path.Beginning.HasValue)*/.Select(path => path.Beginning.Value));
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
    public class PathfindingModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private static readonly TimeSpan FRAME_TIME_SLICE =
            TimeSpan.FromMilliseconds(DxGame.Instance.TargetElapsedTime.Milliseconds);

        private static readonly TimeSpan COMPUTATION_TIMEOUT = TimeSpan.FromMilliseconds(150);
        // Only budget a partial frame for this shit

        public LinkedList<ImmutablePair<TimeSpan, Commandment>> Pathfind(GameObject entity, DxVector2 target)
        {
            var entityIsNull = Check.IsNullOrDefault(entity);
            if (entityIsNull)
            {
                LOG.Info("Pathfinding called with null entity");
            }

            /* If the provided entity is null or we're already at our destination, there's nothing to do */
            if (entityIsNull || entity.ComponentOfType<SpatialComponent>().Space.Contains(target))
            {
                return new LinkedList<ImmutablePair<TimeSpan, Commandment>>();
            }

            var actionComponent = entity.ComponentOfType<StandardActionComponent>();
            ReadOnlyDictionary<Commandment, Force> movementCommendments = actionComponent.MovementForces.Copy();

            var commandmentQueue = new Queue<Commandment>();
            commandmentQueue.Enqueue(Commandment.None);

            var mapModel = DxGame.Instance.Model<MapModel>();
            var navigationMesh = NavigableSurface.SurfaceFor(mapModel);
            var closestNode = navigationMesh.NodeQuery.Closest(target);
            if (!closestNode.HasValue)
            {
                LOG.Info($"Could not find a Node close to {target}");
                return new LinkedList<ImmutablePair<TimeSpan, Commandment>>();
            }

            var targetNode = closestNode.Value;

            var explorableMesh = ExplorableMeshCache.MeshFor(entity, navigationMesh);


            var displacementFunctions = DetermineDisplacementFunction(entity);

            return null;
        }

        private static Dictionary<ImmutablePair<Commandment, Commandment>, DisplacementFunction>
            DetermineDisplacementFunction(GameObject entity)
        {
            Commandment[] movementCommandments =
            {
                Commandment.MoveLeft,
                Commandment.MoveRight,
                Commandment.MoveUp
            };

            var displacementFunctionsByCommandment =
                new Dictionary<ImmutablePair<Commandment, Commandment>, DisplacementFunction>();

            var properties = entity.ComponentOfType<EntityPropertiesComponent>();
            /* Pair every commandment with every commandment. This lets us determine "intersting" combinations of forces */
            foreach (var firstCommandment in movementCommandments)
            {
                foreach (var secondCommandment in movementCommandments)
                {
                    if (firstCommandment == secondCommandment)
                    {
                        continue;
                    }
                    var firstForce = properties.MovementForceFor(firstCommandment);
                    var secondForce = properties.MovementForceFor(secondCommandment);

                    Dictionary<Force, bool> forceReapplication = new Dictionary<Force, bool>
                    {
                        [firstForce] = firstCommandment != Commandment.MoveUp,
                        [secondForce] = secondCommandment != Commandment.MoveUp
                    };
                    var polynomial = RegressForces(forceReapplication);
                    displacementFunctionsByCommandment[
                        new ImmutablePair<Commandment, Commandment>(firstCommandment, secondCommandment)] =
                        polynomial.DisplacementFunction;
                }
            }
            return displacementFunctionsByCommandment;
        }

        private static SimplePolynomial RegressForces(Dictionary<Force, bool> forcesAndReapplication)
        {
            const double maxTime = 10.0; // TODO: Uncap / figure out a better heuristic
            const double timeStep = 0.01;
            var timeSlice = TimeSpan.FromSeconds(timeStep);
            var forcesAsList = forcesAndReapplication.Keys.ToList();
            var position = DxVector2.EmptyVector;
            var velocity = forcesAsList.Aggregate(DxVector2.EmptyVector, (current, force) => current + force.InitialVelocity);
            var gameTime = new DxGameTime(TimeSpan.Zero, TimeSpan.Zero, false);
            var timeList = new List<double>((int) ((maxTime  + 1) / timeStep));
            var xPositions = new List<double>((int)((maxTime + 1) / timeStep));
            var yPositions = new List<double>((int) ((maxTime + 1) / timeStep));
            for (double i = 0; i < maxTime; i += timeStep)
            {
                var newPositionAndVelocity = PhysicsComponent.ForceComputation(gameTime, position, velocity,
                    forcesAsList);
                position = newPositionAndVelocity.Item1;
                velocity = newPositionAndVelocity.Item2;
                forcesAsList.RemoveAll(force => force.Dissipated && !forcesAndReapplication[force]);
                gameTime = new DxGameTime(gameTime.TotalGameTime + timeSlice, timeSlice, false);
                timeList.Add(i);
                xPositions.Add(position.X);
                yPositions.Add(position.Y);
            }
            var times = timeList.ToArray();
            var x = xPositions.ToArray();
            var y = yPositions.ToArray();

            const int polynomialOrder = 2;
            var xRegression = Fit.Polynomial(times, x, polynomialOrder);
            var yRegression = Fit.Polynomial(times, y, polynomialOrder);
            return new SimplePolynomial(xRegression, yRegression);
        }
    }

    [Serializable]
    /* Can only track displacemnt in a single Axis, X or Y */
    internal sealed class SimplePolynomial
    {
        [DataMember]
        public DisplacementFunction DisplacementFunction { get; }

        [DataMember]
        private Tuple<double, double, double> YTerms { get; }

        [DataMember]
        private Tuple<double, double, double> XTerms { get; }

        public SimplePolynomial(double[] xRegression, double[] yRegression)
        {
            const int polynomialTerms = 3;
            Validate.IsNotNullOrDefault(xRegression, $"Cannot create a {GetType()} with a null {nameof(xRegression)}");
            Validate.IsNotNullOrDefault(yRegression, $"Cannot create a {GetType()} with a null {nameof(yRegression)}");
            Validate.AreEqual(xRegression.Length, polynomialTerms);
            Validate.AreEqual(yRegression.Length, polynomialTerms);
            XTerms = Tuple.Create(xRegression[0], xRegression[1], xRegression[2]);
            YTerms = Tuple.Create(yRegression[0], yRegression[1], yRegression[2]);
            DisplacementFunction = Polynomial;
        }

        public DxVector2 Polynomial(TimeSpan offset)
        {
            var time = offset.TotalMilliseconds;
            var x = (float) (XTerms.Item1 + XTerms.Item2 * time + YTerms.Item3 * time * time);
            var y = (float) (YTerms.Item1 + YTerms.Item2 * time + YTerms.Item3 * time * time);
            return new DxVector2(x, y);
        }
    }
}