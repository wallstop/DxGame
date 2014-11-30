using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXGame.Core
{
    public class SimpleBlock : SimpleSprite
    {
        private static readonly log4net.ILog LOG = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int width_ = 50;
        private const int height_ = 50;

        private const char RED_BLOCK = 'R';
        private const char GREEN_BLOCK = 'G';
        private const char BLUE_BLOCK = 'B';
        private const char PURPLE_BLOCK = 'P';
        private const char YELLOW_BLOCK = 'Y';
        private const string BASE_FOLDER = "Map/";

        public SimpleBlock(int column, int row, char colorId)
            : base(ResolveCharacterToString(colorId))
        {
            position_.X = column * width_;
            position_.Y = row * height_;
            LOG.Debug(String.Format("Creating block at (%d, %d), width: %d, height: %d, asset: %s, colorId: %s", column, row, width_, height_, assetName_, colorId));
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
