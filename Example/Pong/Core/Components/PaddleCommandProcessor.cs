using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Physics;
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
        private static readonly DxVector2 Up = new DxVector2(0, -150);
        private static readonly DxVector2 Down = new DxVector2(0, 150);

        private Impulse MoveUpImpulse { get; } = new Impulse(Up);

        private Impulse MoveDownImpulse { get; } = new Impulse(Down);

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
                    new PhysicsAttachment(MoveUpImpulse, Parent.Id).Emit();
                    break;
                }
                case Commandment.MoveDown:
                {
                    new PhysicsAttachment(MoveDownImpulse, Parent.Id).Emit();
                    break;
                }
                case Commandment.None:
                {
                    new PhysicsAttachment(Nullification.Horizontal, Parent.Id).Emit();
                    break;
                }
            }
        }
    }
}
