using System.Text;
using Lidgren.Network;

namespace DXGame.Core.Utils
{
    public static class LidgrenUtils
    {
        public static string MessageContents(this NetIncomingMessage message)
        {
            return Encoding.Default.GetString(message.PeekDataBuffer());
        }
    }
}