using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DxCore.Core.Frames
{
    [Serializable]
    [DataContract]
    public class Frame
    {
        [DataMember] public readonly DxGameTime GameTime;
        public static Frame EmptyFrame { get; } = new Frame(null, new Dictionary<UniqueId, GameObject>());

        [DataMember]
        public ReadOnlyDictionary<UniqueId, GameObject> ObjectMapping { get; }

        protected Frame(DxGameTime gameTime, IDictionary<UniqueId, GameObject> gameObjects)
        {
            ObjectMapping = new ReadOnlyDictionary<UniqueId, GameObject>(gameObjects);
            GameTime = gameTime;
        }

        public static FrameBuilder Builder()
        {
            return new FrameBuilder();
        }

        public class FrameBuilder : IBuilder<Frame>
        {
            private readonly Dictionary<UniqueId, GameObject> gameObjects_ = new Dictionary<UniqueId, GameObject>();
            private DxGameTime gameTime_;

            public virtual Frame Build()
            {
                Validate.IsNotNull(gameTime_, this.GetFormattedNullOrDefaultMessage(gameTime_));
                Validate.NoNullElements(gameObjects_.Keys,
                    $"Cannot create a {typeof(Frame)} that has a null {typeof(UniqueId)} in it's object mapping");
                Validate.NoNullElements(gameObjects_.Values,
                    $"Cannot create a {typeof(Frame)} that has a null {typeof(GameObject)} in it's object mapping");

                return new Frame(gameTime_, gameObjects_);
            }

            public FrameBuilder WithGameTime(DxGameTime gameTime)
            {
                gameTime_ = gameTime;
                return this;
            }

            public FrameBuilder WithGameObject(GameObject gameObject)
            {
                Validate.IsNotNull(gameObject);
                gameObjects_[gameObject.Id] = gameObject;
                return this;
            }
        }
    }
}