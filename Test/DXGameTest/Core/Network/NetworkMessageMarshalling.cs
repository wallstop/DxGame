using DXGame.Core;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using NUnit.Framework;

namespace DXGameTest.Core.Network
{
    public class NetworkMessageMarshalling
    {
        [Test]
        public void SerializeRectangle2F()
        {
            var rectangle = new DxRectangle(200, 100, 300, 400);
            byte[] serialized = Serializer<DxRectangle>.BinarySerialize(rectangle);
            var serializedRectangle = Serializer<DxRectangle>.BinaryDeserialize(serialized);
            Assert.AreEqual(rectangle, serializedRectangle);

            byte[] jsonSerialized = Serializer<DxRectangle>.JsonSerialize(rectangle);
            string jsonText = System.Text.Encoding.Default.GetString(jsonSerialized);
            var jsonDeserialized = Serializer<DxRectangle>.JsonDeserialize(jsonSerialized);
            Assert.AreEqual(rectangle, jsonDeserialized);
        }
    }
}