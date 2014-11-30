using System;
using log4net;

namespace DXGame.Core
{
    public class SimpleBlock : SimpleSprite
    {
        private const int width_ = 50;
        private const int height_ = 50;

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
            position_.X = column * width_;
            position_.Y = row * height_;
            LOG.Debug(String.Format("Creating block at ({0}, {1}), width: {2}, height: {3}, asset: {4}, colorId: {5}", column,
                row, width_, height_, assetName_, colorId));
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
                assetString += "RedBlock";
                break;
            case BLUE_BLOCK:
                assetString += "RedBlock";
                break;
            case PURPLE_BLOCK:
                assetString += "RedBlock";
                break;
            case YELLOW_BLOCK:
                assetString += "RedBlock";
                break;
            default:
                return "";
            }
            return assetString;
        }
    }
}