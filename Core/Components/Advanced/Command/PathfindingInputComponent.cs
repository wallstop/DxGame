using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Command
{
    /**

        <summary>
            Should only be used by the PathfindingModel to dogfood commands into an entity
        </summary>

        Note: This should NOT be serializable/DataContracted. It should only exist as a temporary in memory.
    */

    public delegate Commandment CommandmentProducer();

    public class PathfindingInputComponent : AbstractCommandComponent
    {
        private CommandmentProducer CommandmentFeeder { get; }

        public PathfindingInputComponent(CommandmentProducer commandmentFeeder)
        {
            Validate.IsNotNullOrDefault(commandmentFeeder,
                StringUtils.GetFormattedNullOrDefaultMessage(this, commandmentFeeder));
            CommandmentFeeder = commandmentFeeder;
        }

        protected override void Update(DxGameTime gameTime)
        {
            var commandment = CommandmentFeeder();
            var commandMessage = new CommandMessage {Commandment = commandment};
            Parent?.BroadcastMessage(commandMessage);
        }
    }
}