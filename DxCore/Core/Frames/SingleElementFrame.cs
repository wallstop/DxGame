using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.Frames
{
    /**
        Simple Frame that contains only a single element (game object)

        This is useful for things like "expected gamestate" where GameTime is unimportant
    */
    [Serializable]
    [DataContract]
    public class SingleElementFrame : Frame
    {
        protected SingleElementFrame(DxGameTime gameTime, IDictionary<UniqueId, GameObject> gameObjects)
            : base(gameTime, gameObjects)
        {
        }

        public new static SingleElementFrameBuilder Builder()
        {
            return new SingleElementFrameBuilder();
        }

        public class SingleElementFrameBuilder : FrameBuilder
        {
            private GameObject gameObject_;

            public new SingleElementFrameBuilder WithGameObject(GameObject gameObject)
            {
                gameObject_ = gameObject;
                return this;
            }

            public override Frame Build()
            {
                Validate.IsNotNull(gameObject_, StringUtils.GetFormattedNullOrDefaultMessage(this, gameObject_));
                return new SingleElementFrame(null, new Dictionary<UniqueId, GameObject> {{gameObject_.Id, gameObject_}});
            }
        }
    }
}