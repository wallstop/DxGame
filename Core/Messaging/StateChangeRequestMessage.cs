namespace DXGame.Core.Messaging
{
    public class StateChangeRequestMessage : Message
    {
        public string State { get; set; }
    }
}