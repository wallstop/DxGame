using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Basic;
using DXGame.Core.Wrappers;
using DXGame.Main;
using NUnit.Framework;

namespace DXGameTest.Core.Network
{
    public class SimpleNetworkSerialization
    {
        private BoundedSpatialComponent spatial_;
        private SimpleSpriteComponent sprite_;
        private MapCollideablePhysicsComponent physics_;
        private GameObject gameObject_;

        private DxGame game_;

        [Serializable]
        [DataContract]
        private class TestSerializable : IEquatable<TestSerializable>
        {
            [DataMember]
            public DxGameTime GameTime { get; set; }

            [DataMember]
            public List<Component> Components { get; set; }

            [DataMember]
            public List<GameObject> GameObjects { get; set; }


            public bool Equals(TestSerializable other)
            {
                if (!GameTime.Equals(other.GameTime))
                {
                    return false;
                }

                if (Components.Count != other.Components.Count)
                {
                    return false;
                }

                if (Components.Any(component => !other.Components.Contains(component)))
                {
                    return false;
                }

                if (GameObjects.Count != other.GameObjects.Count)
                {
                    return false;
                }

                if (GameObjects.Any(gameObject => !other.GameObjects.Contains(gameObject)))
                {
                    return false;
                }

                return true;
            }
        }

        // TODO
        [SetUp]
        public void SetUp()
        {
            game_ = new DxGame();
            var bounds = new DxRectangle(0, 0, 5000, 5000);
            spatial_ = new BoundedSpatialComponent(game_).WithBounds(bounds);
            sprite_ = new SimpleSpriteComponent(game_).WithAsset("Orb").WithPosition(spatial_);
            physics_ = new MapCollideablePhysicsComponent(game_).WithSpatialComponent(spatial_);
            gameObject_ = new GameObject() .WithComponents(spatial_, sprite_, physics_ );
        }

        [Test]
        public void SimpleSerialization()
        {
            TestSerializable serializable = new TestSerializable
            {
                Components = new List<Component> {spatial_, sprite_, physics_ },
                GameTime = new DxGameTime(TimeSpan.FromSeconds(23), TimeSpan.FromSeconds(1.2)),
                GameObjects = new List<GameObject> {gameObject_}
            };

            var binaryOutput = Serializer<TestSerializable>.BinarySerialize(serializable);
            var convertedSerializable = Serializer<TestSerializable>.BinaryDeserialize(binaryOutput);
            // TODO: Figure out how to inject the GameInstance into the components on serialization...
            Assert.AreEqual(serializable, convertedSerializable);
        }
    }
}
