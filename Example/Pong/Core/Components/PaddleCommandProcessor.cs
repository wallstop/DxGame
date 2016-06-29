using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;

namespace Pong.Core.Components
{
    /**
        <summary>
            Explicitly handles Up/Down commands by applying relevant forces
        </summary>
    */
    public class PaddleCommandProcessor : Component
    {
        private static readonly DxVector2 Up = new DxVector2(0, -1);
        private static readonly DxVector2 Down = new DxVector2(0, 1);

        private Force MoveUpForce => new Force(DxVector2.EmptyVector, Up, Force.OneFrameDissipation(Up), "PaddleUp");

        private Force MoveDownForce
            => new Force(DxVector2.EmptyVector, Down, Force.OneFrameDissipation(Down), "PaddleDown");

        public override void OnAttach()
        {
            RegisterMessageHandler<CommandMessage>(HandleCommandMessage);
            base.OnAttach();
        }

        private void HandleCommandMessage(CommandMessage commandment)
        {
            switch(commandment.Commandment)
            {
                case Commandment.MoveUp:
                {
                    new AttachForce(Parent.Id, MoveUpForce).Emit();
                    break;
                }
                case Commandment.MoveDown:
                {
                    new AttachForce(Parent.Id, MoveDownForce).Emit();
                    break;
                }
            }
        }
    }
}
