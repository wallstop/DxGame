using System;

namespace DXGame.Core.Generators
{
    public class MapGenerator : ObjectGenerator<GameObject>
    {
        private const string BASE_FOLDER = "Map/Blocks/";

        public MapGenerator(string mapPath)
        {
        }

        public override GameObject Generate()
        {
            throw new NotImplementedException();
        }
    }
}