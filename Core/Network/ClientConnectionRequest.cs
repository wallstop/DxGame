namespace DXGame.Core.Network
{
    public class ClientConnectionRequest : NetworkMessage
    {
        public string PlayerName { get; set; }

        // TODO: Maybe store IP here too so we can do some kind of reconnect logic later?

        // TODO: Finish fleshing out what needs to be in a request
    }
}