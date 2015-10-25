using DXGame.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Pathfinding
{
    public static class PathfindingConstants
    {
        public static readonly int SimulationUpperBound = 10;
        public static readonly double SimulationStep = 0.01;
        public static readonly TimeSpan MaxSimulationTime = TimeSpan.FromSeconds(SimulationUpperBound);
        public static readonly TimeSpan SimulationTimeStep = TimeSpan.FromSeconds(SimulationStep);

        public static readonly ReadOnlyCollection<Commandment> SurfaceTraversalCommandments =
            new ReadOnlyCollection<Commandment>(new List<Commandment> { Commandment.MoveLeft, Commandment.MoveRight });

        public static readonly ReadOnlyCollection<Commandment> VerticalTraversalCommandments =
            new ReadOnlyCollection<Commandment>(new List<Commandment> { Commandment.MoveUp, Commandment.MoveDown });

        public static readonly ReadOnlyCollection<CommandChain> AvailableCommandments = 
            new ReadOnlyCollection<CommandChain>(new List<CommandChain>
        {
            new CommandChain(Commandment.None),
            new CommandChain(Commandment.MoveUp),
            new CommandChain(Commandment.MoveLeft),
            new CommandChain(Commandment.MoveRight),
            new CommandChain(Commandment.MoveDown),
            new CommandChain(Commandment.MoveUp, Commandment.MoveLeft),
            new CommandChain(Commandment.MoveUp, Commandment.MoveRight),
            new CommandChain(Commandment.MoveDown, Commandment.MoveLeft),
            new CommandChain(Commandment.MoveDown, Commandment.MoveRight)
        });
    }
}
