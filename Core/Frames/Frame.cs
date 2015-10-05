using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;
using NLog;

namespace DXGame.Core.Frames
{
    [Serializable]
    [DataContract]
    public class Frame
    {
        // TODO: Make this a thing

        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [DataMember] private readonly Dictionary<UniqueId, IIdentifiable> frameObjects_ =
            new Dictionary<UniqueId, IIdentifiable>();

        [DataMember] public readonly DxGameTime GameTime;

        public Frame(DxGameTime gameTime)
        {
            GameTime = gameTime;
        }

        public void AddToFrame<T>(T identifiableObject) where T : IIdentifiable
        {
            var id = identifiableObject.Id;
            if (frameObjects_.ContainsKey(id))
            {
                var existingObject = frameObjects_[id];
                if (existingObject.Equals(identifiableObject))
                {
                    var logMessage =
                        $"Could not insert {identifiableObject} into frame; a different object" +
                        $" already exists with that Id {existingObject}";
                    LOG.Error(logMessage);
                    throw new ArgumentException(logMessage);
                }
            }
            else
            {
                frameObjects_[id] = identifiableObject;
            }
        }
    }
}