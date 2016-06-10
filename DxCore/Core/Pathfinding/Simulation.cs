using System;
using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Messaging;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Cache.Simple;
using MathNet.Numerics;

namespace DxCore.Core.Pathfinding
{
    public static class Simulation
    {
        private static readonly
            UnboundedLoadingSimpleCache<EntityTypeComponent, Dictionary<CommandChain, DisplacementApproximator>>
            ENTITY_MOVEMENT_APPROXIMATORS =
                new UnboundedLoadingSimpleCache<EntityTypeComponent, Dictionary<CommandChain, DisplacementApproximator>>
                    (entityTypeComponent =>
                        ApproximatorForProperties(
                            entityTypeComponent.Parent.ComponentOfType<EntityPropertiesComponent>()));

        public static Dictionary<CommandChain, DisplacementApproximator> DetermineDisplacementApproximators(
            GameObject entity, bool cacheResults = false)
        {
            EntityTypeComponent entityType = entity.ComponentOfType<EntityTypeComponent>();
            return ENTITY_MOVEMENT_APPROXIMATORS.Get(entityType);
        }

        private static Dictionary<CommandChain, DisplacementApproximator> ApproximatorForProperties(
            EntityPropertiesComponent entityProperties)
        {
            Dictionary<CommandChain, DisplacementApproximator> approximatorsByCommand =
                new Dictionary<CommandChain, DisplacementApproximator>();
            foreach(CommandChain commandChain in PathfindingConstants.AvailableCommandments)
            {
                DisplacementApproximator approximator = RegressForces(commandChain, entityProperties);
                approximatorsByCommand[commandChain] = approximator;
            }
            return approximatorsByCommand;
        }

        /**
            <summary>
                Creates a DisplacementApproximator for the provided CommandChain and EntityProperties.
                This lets us approximate our CommandChain via the DisplacementApproximator interface.
            </summary>
        */

        public static DisplacementApproximator RegressForces(CommandChain commandChain,
            EntityPropertiesComponent properties)
        {
            List<Force> initialForces = commandChain.Commandments.Select(properties.MovementForceFor).ToList();
            List<Func<Force>> forceRegenerators = new List<Func<Force>>();
            /* 
                We want to regenerate directional forces every single step of the simulation, but we need to generate a NEW force each time.
                In order to do this, we maintain a list of functions that are capable of producing brand new forces that should be re-applied
                every tick of the sim :^)
            */
            foreach(Commandment commandment in commandChain.Commandments)
            {
                if(PathfindingConstants.SurfaceTraversalCommandments.Contains(commandment))
                {
                    Commandment concreteCommandment = commandment;
                    forceRegenerators.Add(() => properties.MovementForceFor(concreteCommandment));
                }
            }

            TimeSpan maxSimulationTime = PathfindingConstants.SimulationTimes[commandChain];
            /* 
                Additionally, we want to apply gravity to only CommandChains that contain a vertical movement component. 
                This lets us properly "model" traveling across a surface 
             */
            if(PathfindingConstants.VerticalTraversalCommandments.Intersect(commandChain.Commandments).Any())
            {
                initialForces.Add(WorldForces.Gravity);
            }
            /* And finally, everything is affected by air resistance */
            initialForces.Add(WorldForces.AirResistance);

            return SimulateAndApproximate(initialForces, forceRegenerators, maxSimulationTime);
        }

        private static DisplacementApproximator SimulateAndApproximate(List<Force> initialForces,
            List<Func<Force>> forceRegenerators, TimeSpan maxSimulationTime)
        {
            /* Initialize our simulation shit */
            DxVector2 position = DxVector2.EmptyVector;
            DxVector2 velocity = initialForces.Aggregate(DxVector2.EmptyVector,
                (current, force) => current + force.InitialVelocity);
            DxGameTime gameTime = new DxGameTime(TimeSpan.Zero, TimeSpan.Zero, false);

            int expectedNumberOfElements =
                (int) ((maxSimulationTime.TotalSeconds + 1) / PathfindingConstants.SimulationStep);
            List<double> times = new List<double>(expectedNumberOfElements);
            List<double> xPositions = new List<double>(expectedNumberOfElements);
            List<double> yPositions = new List<double>(expectedNumberOfElements);

            /* Basic sim loop: step "time frame by time frame" through our wonderful physics world */
            for(double i = 0; i < maxSimulationTime.TotalSeconds; i += PathfindingConstants.SimulationStep)
            {
                var newPositionAndVelocity = PhysicsComponent.ForceComputation(gameTime, position, velocity,
                    initialForces);
                position = newPositionAndVelocity.Item1;
                velocity = newPositionAndVelocity.Item2;
                initialForces.RemoveAll(force => force.Dissipated);
                List<Force> newForces = forceRegenerators.Select(regenerator => regenerator()).ToList();
                initialForces.AddRange(newForces);
                newForces.ForEach(newForce => velocity += newForce.InitialVelocity);

                gameTime = new DxGameTime(gameTime.TotalGameTime + PathfindingConstants.SimulationTimeStep,
                    PathfindingConstants.SimulationTimeStep, false);
                /* Make sure to track our progress */
                times.Add(i);
                xPositions.Add(position.X);
                yPositions.Add(position.Y);
            }

            /* 
                Determine actual positional bounds, if we do it via our fitted functions it won't be truthful 
                (may as well do it here, where we know) 
            */
            double xMin = xPositions.Min();
            double xMax = xPositions.Max();
            double yMin = yPositions.Min();
            double yMax = yPositions.Max();
            DxRectangle bounds = new DxRectangle(xMin, yMin, xMax - xMin, yMax - yMin);

            /* Convert to double arrays for polynomial fitting goodness */
            double[] timesAsArray = times.ToArray();
            double[] x = xPositions.ToArray();
            SanitizeXValues(x);
            double[] y = yPositions.ToArray();
            const int polynomialOrder = 5;
            double[] xRegression = Fit.Polynomial(timesAsArray, x, polynomialOrder);
            double[] yRegression = Fit.Polynomial(timesAsArray, y, polynomialOrder);
            double[] positionalRegression = Fit.Polynomial(x, y, polynomialOrder);
            double[] xToTimeRegression = Fit.Polynomial(x, timesAsArray, polynomialOrder);
            return new DisplacementApproximator(xRegression, yRegression, positionalRegression, xToTimeRegression,
                bounds, maxSimulationTime);
        }

        /**
            <summary>
                In order for a function to... be a function, we need monotonically increasing x values (or close enough).
                However, we KNOW that we'll have forces that are just like "jump straight up" and "drop through this platform straight down".
                In these cases, we need to "fake" our data just a little bit. 
                This function takes an array of values. If they're all the same, then it pads them to be monotonically increasing.
            </summary>
        */

        private static void SanitizeXValues(double[] xValues)
        {
            double x = xValues[0];
            if(xValues.Any(xValue => x != xValue))
            {
                return;
            }
            /* Pad stuff so we have sane functions :^) */
            const double initial = 0.0001;
            double accumulator = initial;
            for(int i = 0; i < xValues.Length; ++i)
            {
                xValues[i] += accumulator;
                accumulator += initial;
            }
        }
    }
}
