using System;
using DXGame.Core;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace DXGameTest.Core.Network
{
    public class NetworkMessageMarshalling
    {
        [Test]
        public void SerializeVector2()
        {
            var vector2 = new Vector2(300.4f, 102.33f);
            byte[] serialized = Serializer<Vector2>.BinarySerialize(vector2);
            var serializedVector2 = Serializer<Vector2>.BinaryDeserialize(serialized);
            Assert.AreEqual(vector2, serializedVector2);

            byte[] jsonSerialized = Serializer<Vector2>.JsonSerialize(vector2);
            string jsonText = jsonSerialized.ToString();
            Console.WriteLine(jsonText);
        }
    }
}