using DXGame.Core.Utils;
using DXGame.Main;
using Lidgren.Network;

namespace DXGame.Core.Components.Basic
{
    /*
        Basic networked component. In general, there should really only be two types of these - Client & Server
    */
    public abstract class NetworkComponent : Component
    {
        public NetPeer Connection { get; set; }

        protected NetworkComponent(DxGame game)
            : base(game)
        {
        }

        public NetworkComponent WithConnection(NetPeer connection)
        {
            GenericUtils.CheckNullOrDefault(connection, "Cannot create a NetworkComponent with a null/default NetPeer");
            Connection = connection;
            return this;
        }

        public NetworkComponent WithConfiguration(NetPeerConfiguration config)
        {
            GenericUtils.CheckNull(config, "Cannot create a NetworkComponent with a null NetPeerConfiguration");
            Connection = new NetPeer(config);
            return this;
        }

        protected abstract void EstablishConnection();

        public abstract void ReceiveData();

        public abstract void SendData();
    }
}