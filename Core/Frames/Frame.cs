﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Wrappers;
using log4net;

namespace DXGame.Core.Frames
{
    [Serializable]
    [DataContract]
    public class Frame
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (Frame));

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
                        String.Format(
                            "Could not insert {0} into frame; a different object" + " already exists with that Id {1}",
                            identifiableObject, existingObject);
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
