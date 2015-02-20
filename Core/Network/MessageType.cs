namespace DXGame.Core.Network
{
    /*
        TODO: Enumerate these as need be. These represent the individual "kinds" of message that
        can be received over the network and will generally have classes associated with them.
    */

    public enum MessageType
    {
        CLIENT_REQUEST_CONNECTION,  // Client connect to server
        CLIENT_DATA_DIFF,           // Client info of how it thought a frame went
        SERVER_DATA_DIFF,           // Server info of the diff between it's last update and "now"
        SERVER_DATA_KEYFRAME        // Server full-state dump
    }
}