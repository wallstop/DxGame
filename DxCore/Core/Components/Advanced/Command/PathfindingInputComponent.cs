using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Map;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using NLog;

namespace DxCore.Core.Components.Advanced.Command
{
    [Serializable]
    [DataContract]
    public class PathfindingInputComponent : AbstractCommandComponent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Queue<NavigableMeshNode> Path { get; set; }

        public PathfindingInputComponent()
        {
            Path = new Queue<NavigableMeshNode>();
            UpdatePriority = UpdatePriority.High;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<PathfindingResponse>(HandlePathfindingResponse);
            base.OnAttach();
        }

        private void HandlePathfindingResponse(PathfindingResponse pathfindingResponse)
        {
            Path = new Queue<NavigableMeshNode>(pathfindingResponse.Path);
        }

        protected override void Update(DxGameTime gameTime)
        {
            /* Super simple to start things off */
            if(!Path.Any())
            {
                new CommandMessage(Commandment.None, Parent.Id).Emit();
                return;
            }

            /* Need a spatial */
            ISpatial spatial = Parent.Components.OfType<ISpatial>().FirstOrDefault();
            if(ReferenceEquals(spatial, null))
            {
                Logger.Debug("Could not pathfind - no spatial :(");
                new CommandMessage(Commandment.None, Parent.Id).Emit();
                return;
            }

            /* Remove any no-op commands - ie, "go to exactly where you are" */
            NavigableMeshNode target = null;
            while(Path.Any() && ((target = Path.Peek()).Space.Contains(spatial.Space) || spatial.Space.Contains(target.Space)))
            {
                Path.Dequeue();
                target = null;
            }

            /* No path left? Hang out, dog. */
            if(ReferenceEquals(target, null))
            {
                new CommandMessage(Commandment.None, Parent.Id).Emit();
                return;
            }

            /* Alright, we have a valid path. Figure out how to proceed */
            DxVector2 discrepancy = target.Space.Center - spatial.Space.Center;
            Commandment command;
            if((target.Space.Height + spatial.Space.Height) / 2 < -discrepancy.Y)
            {
                command = Commandment.MoveUp;
            }
            else if(discrepancy.X < 0)
            {
                command = Commandment.MoveLeft;
            }
            else if(0 < discrepancy.X)
            {
                command = Commandment.MoveRight;
            }
            else
            {
                command = Commandment.None;
            }

            new CommandMessage(command, Parent.Id).Emit();
        }
    }
}