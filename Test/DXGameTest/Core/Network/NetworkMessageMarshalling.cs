using DXGame.Core;
using DXGame.Core.Utils;
using NUnit.Framework;

namespace DXGameTest.Core.Network
{
    public class NetworkMessageMarshalling
    {
        [Test]
        public void SerializeRectangle2F()
        {
            var rectangle = new Rectangle2F(200, 100, 300, 400);
            byte[] serialized = Serializer<Rectangle2F>.BinarySerialize(rectangle);
            var serializedRectangle = Serializer<Rectangle2F>.BinaryDeserialize(serialized);
            Assert.AreEqual(rectangle, serializedRectangle);

            byte[] jsonSerialized = Serializer<Rectangle2F>.JsonSerialize(rectangle);
            string jsonText = System.Text.Encoding.Default.GetString(jsonSerialized);
            var jsonDeserialized = Serializer<Rectangle2F>.JsonDeserialize(jsonSerialized);
            Assert.AreEqual(rectangle, jsonDeserialized);
        }
    }
}