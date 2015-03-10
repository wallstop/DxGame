using DXGame.Core.Components.Basic;
using DXGame.Main;
using Lidgren.Network;

namespace DXGame.Core.Components.Advanced
{
    public class PropertiesComponent : Component
    {
        public PropertiesComponent(DxGame game)
            : base(game)
        {
        }

        public override void SerializeTo(NetOutgoingMessage message)
        {
            throw new System.NotImplementedException();
        }

        public override void DeserializeFrom(NetIncomingMessage messsage)
        {
            throw new System.NotImplementedException();
        }
    }
}