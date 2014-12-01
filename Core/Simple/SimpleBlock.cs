using System;
using log4net;

namespace DXGame.Core.Simple
{
    public class SimpleBlock : SimpleSprite
    {
        private const char RED_BLOCK = 'R';
        private const char GREEN_BLOCK = 'G';
        private const char BLUE_BLOCK = 'B';
        private const char PURPLE_BLOCK = 'P';
        private const char YELLOW_BLOCK = 'Y';
        private const string BASE_FOLDER = "Map/Blocks/";
        private static readonly ILog LOG = LogManager.GetLogger(typeof (SimpleBlock));

        public SimpleBlock(int column, int row, char colorId)
            : base(ResolveCharacterToString(colorId))
        {
            position_.X = column * BLOCK_WIDTH;
            position_.Y = row * BLOCK_WIDTH;
        }

        public static bool CanCreateFrom(char character)
        {
            return ResolveCharacterToString(character) != "";
        }

        private static string ResolveCharacterToString(char character)
        {
            string assetString = BASE_FOLDER;
            switch (character)
            {
            case RED_BLOCK:
                assetString += "RedBlock";
                break;
            case GREEN_BLOCK:
                assetString += "GreenBlock";
                break;
            case BLUE_BLOCK:
                assetString += "BlueBlock";
                break;
            case PURPLE_BLOCK:
                assetString += "PurpleBlock";
                break;
            case YELLOW_BLOCK:
                assetString += "YellowBlock";
                break;
            default:
                return "";
            }
            return assetString;
        }
    }
}