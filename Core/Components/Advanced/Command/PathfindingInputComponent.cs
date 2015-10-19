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

        public PathfindingInputComponent(DxGame game, CommandmentProducer commandmentFeeder)
            : base(game)
        {
            Validate.IsNotNullOrDefault(commandmentFeeder,
                StringUtils.GetFormattedNullOrDefaultMessage(this, commandmentFeeder));
            CommandmentFeeder = commandmentFeeder;
        }

        protected override void Update(DxGameTime gameTime)
        {
            var commandMessage = new CommandMessage {Commandment = CommandmentFeeder()};
            Parent?.BroadcastMessage(commandMessage);
        }
    }
}