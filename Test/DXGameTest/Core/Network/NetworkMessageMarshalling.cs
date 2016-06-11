using DxCore.Core;
using DxCore.Core.Primitives;
using NUnit.Framework;

namespace DXGameTest.Core.Network
{
    public class NetworkMessageMarshalling
    {
        [Test]
        public void SerializeDxRectangle()
        {
            var rectangle = new DxRectangle(200, 100, 300, 400);
            byte[] serialized = Serializer<DxRectangle>.BinarySerialize(rectangle);
            var serializedRectangle = Serializer<DxRectangle>.BinaryDeserialize(serialized);
            Assert.AreEqual(rectangle, serializedRectangle);

            byte[] jsonSerialized = Serializer<DxRectangle>.JsonSerialize(rectangle);
            var jsonDeserialized = Serializer<DxRectangle>.JsonDeserialize(jsonSerialized);
            Assert.AreEqual(rectangle, jsonDeserialized);
        }
    }
}