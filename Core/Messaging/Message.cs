using System;
using System.Runtime.Serialization;
using DXGame.Main;

namespace DXGame.Core.Messaging
{
    /**
        <summary>
            Simple base class for all of your messaging needs :^)

            TODO: Make interface? Abstract? Core... message ...methods? This isn't ideal.
        </summary>
    */
    [Serializable]
    [DataContract]
    public class Message : IIdentifiable
    {
        private static readonly Message EMPTY_MESSAGE = new Message();

        public virtual UniqueId Id { get; } = new UniqueId();

        public static Message EmptyMessage => EMPTY_MESSAGE;

        protected Message()
        {
        }
    }
}