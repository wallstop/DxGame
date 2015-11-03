using DXGame.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DXGame.Core.Pathfinding
{
    public static class PathfindingConstants
    {
        public static readonly double SimpleSimulationUpperBound = 1;
        public static readonly double ComplexSimulationUpperBound = 5;
        public static readonly double SimulationStep = 0.01;
        public static readonly TimeSpan MaxComplexSimulationTime = TimeSpan.FromSeconds(ComplexSimulationUpperBound);
        public static readonly TimeSpan MaxSimpleSimulationTime = TimeSpan.FromSeconds(SimpleSimulationUpperBound);
        public static readonly TimeSpan SimulationTimeStep = TimeSpan.FromSeconds(SimulationStep);

        public static readonly ReadOnlyCollection<Commandment> SurfaceTraversalCommandments =
            new ReadOnlyCollection<Commandment>(new List<Commandment> { Commandment.MoveLeft, Commandment.MoveRight });

        public static readonly ReadOnlyCollection<Commandment> VerticalTraversalCommandments =
            new ReadOnlyCollection<Commandment>(new List<Commandment> { Commandment.MoveUp, Commandment.MoveDown });

        public static readonly CommandChain CommandChainNone = new CommandChain(Commandment.None);
        public static readonly CommandChain CommandChainUp = new CommandChain(Commandment.MoveUp);
        public static readonly CommandChain CommandChainLeft = new CommandChain(Commandment.MoveLeft);
        public static readonly CommandChain CommandChainRight = new CommandChain(Commandment.MoveRight);
        public static readonly CommandChain CommandChainDown = new CommandChain(Commandment.MoveDown);
        public static readonly CommandChain CommandChainUpLeft = new CommandChain(Commandment.MoveUp, Commandment.MoveLeft);
        public static readonly CommandChain CommandChainUpRight = new CommandChain(Commandment.MoveUp, Commandment.MoveRight);
        public static readonly CommandChain CommandChainDownLeft = new CommandChain(Commandment.MoveDown, Commandment.MoveLeft);
        public static readonly CommandChain CommandChainDownRight = new CommandChain(Commandment.MoveDown, Commandment.MoveRight);

        public static readonly ReadOnlyCollection<CommandChain> AvailableCommandments = 
            new ReadOnlyCollection<CommandChain>(new List<CommandChain>
        {
                CommandChainNone, CommandChainUp, CommandChainLeft, CommandChainRight, CommandChainDown, CommandChainUpLeft, CommandChainUpRight, CommandChainDownLeft, CommandChainDownRight
        });

        public static readonly ReadOnlyDictionary<CommandChain, TimeSpan> SimulationTimes = new ReadOnlyDictionary<CommandChain, TimeSpan>(new Dictionary<CommandChain, TimeSpan>()
        {
            [CommandChainNone] = MaxSimpleSimulationTime,
            [CommandChainUp] = MaxComplexSimulationTime,
            [CommandChainLeft] = MaxSimpleSimulationTime,
            [CommandChainRight] = MaxSimpleSimulationTime,
            [CommandChainDown] = MaxSimpleSimulationTime,
            [CommandChainUpLeft] = MaxComplexSimulationTime,
            [CommandChainUpRight] = MaxComplexSimulationTime,
            [CommandChainDownLeft] = MaxSimpleSimulationTime,
            [CommandChainDownRight] = MaxSimpleSimulationTime
        });
    }
}
