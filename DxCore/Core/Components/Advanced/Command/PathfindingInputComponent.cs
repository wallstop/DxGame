using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Messaging;
using DxCore.Core.Pathfinding;
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

            ISpatial spatial = Parent.Components.OfType<ISpatial>().FirstOrDefault();
            if(ReferenceEquals(spatial, null))
            {
                Logger.Debug("Could not pathfind - no spatial :(");
                new CommandMessage(Commandment.None, Parent.Id).Emit();
                return;
            }
            NavigableMeshNode target = Path.Peek();
            if(target.Space.Contains(spatial.Space.Center))
            {
                Path.Dequeue();
                new CommandMessage(Commandment.None, Parent.Id).Emit();
                return;
                // We'll get it next frame
            }

            DxVector2 discrepancy = target.Space.Center - spatial.Space.Center;
            if(discrepancy.X < 0)
            {
                new CommandMessage(Commandment.MoveLeft, Parent.Id).Emit();
            }
            else if(discrepancy.X > 0)
            {
                new CommandMessage(Commandment.MoveRight, Parent.Id).Emit();
            }
            else
            {
                new CommandMessage(Commandment.None, Parent.Id).Emit();
            }
        }
    }
}