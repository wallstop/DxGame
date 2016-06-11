﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using NUnit.Framework;

namespace DXGameTest.Core.Network
{
    public class SimpleNetworkSerialization
    {
        private DxGame game_;
        private GameObject gameObject_;
        private MapCollidablePhysicsComponent physics_;
        private SpatialComponent spatial_;
        // TODO
        [SetUp]
        public void SetUp()
        {
            game_ = DxGame.Instance;
            game_.RunOneFrame();
            var bounds = new DxRectangle(0, 0, 5000, 5000);
            spatial_ = (SpatialComponent)
                    BoundedSpatialComponent.Builder().WithBounds(bounds).WithPosition(new DxVector2(200, 400)).Build();
            physics_ = (MapCollidablePhysicsComponent)MapCollidablePhysicsComponent.Builder().WithSpatialComponent(spatial_).Build();
            gameObject_ = GameObject.Builder().WithComponents(spatial_, physics_).Build();
        }

        [Test]
        public void SimpleSerialization()
        {
            TestSerializable serializable = new TestSerializable
            {
                Components = new List<Component> {spatial_, physics_},
                GameTime = new DxGameTime(TimeSpan.FromSeconds(23), TimeSpan.FromSeconds(1.2)),
                GameObjects = new List<GameObject> {gameObject_}
            };

            var binaryOutput = Serializer<TestSerializable>.BinarySerialize(serializable);
            var convertedSerializable = Serializer<TestSerializable>.BinaryDeserialize(binaryOutput);

            // For now just do a shallow check - see if we have matching ids
            foreach (var component in convertedSerializable.Components)
            {
                Assert.IsTrue(
                    serializable.Components.Any(
                        innerComponent => component.Id.Equals(innerComponent.Id)));
            }
            foreach (GameObject gameObject in convertedSerializable.GameObjects)
            {
                Assert.IsTrue(
                    serializable.GameObjects.Any(
                        innerGameObject => gameObject.Id.Equals(innerGameObject.Id)));
            }

            var spatial = serializable.Components.OfType<SpatialComponent>().First();
            Assert.NotNull(spatial);

            var oldPosition = spatial.Position;
            spatial.Position += new DxVector2(300, 3000);
            Assert.AreNotEqual(oldPosition, spatial.Position);

            var physics = serializable.Components.OfType<PhysicsComponent>().First();
            Assert.NotNull(physics);
            // Make sure the object references have been properly propogated
            Assert.AreEqual(physics.Position, spatial.Position);
        }

        [Serializable]
        [DataContract]
        private class TestSerializable
        {
            [DataMember]
            public DxGameTime GameTime { get; set; }

            [DataMember]
            public List<Component> Components { get; set; }

            [DataMember]
            public List<GameObject> GameObjects { get; set; }
        }
    }
}