using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils;

namespace DxCore.Core.Network
{
    /**
        <summary>
            Simple wrapper with validation for Ports (TCP/IP)
        </summary>
    */

    [DataContract]
    [Serializable]
    public struct Port
    {
        [DataMember]
        public ushort Number { get; }

        public Port(int portNumber)
        {
            Number = checked((ushort) portNumber);
        }

        public static implicit operator Port(int portNumber)
        {
            return new Port(portNumber);
        }

        public static implicit operator Port(ushort portNumber)
        {
            return new Port(portNumber);
        }

        public static implicit operator int(Port port)
        {
            return port.Number;
        }

        public static implicit operator ushort(Port port)
        {
            return port.Number;
        }

        public static bool operator !=(Port lhs, Port rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(Port lhs, Port rhs)
        {
            return lhs.Equals(rhs);
        }

        public override bool Equals(object other)
        {
            return other is Port && this == (Port) other;
        }

        public bool Equals(Port other)
        {
            return Number == other.Number;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
