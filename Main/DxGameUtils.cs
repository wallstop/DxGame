using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXGame.Main
{
    public struct DxGameArguments
    {
        public bool IsServer;

        public string ServerIp;
        public int ServerPort;
    }

    public enum DxGameRole
    {
        None,
        Server,
        Client
    }
}
