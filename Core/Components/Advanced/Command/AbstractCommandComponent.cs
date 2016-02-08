using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Command
{
    /**
        All components that issue "commandments" must derive from this type or else chaos will ensue.

        <summary>
            Base class for all Components that will issue Commandments
        </summary>
    */

    [Serializable]
    [DataContract]
    [ProtoContract]
    public abstract class AbstractCommandComponent : Component
    {
        protected AbstractCommandComponent()
        {
        }
    }
}