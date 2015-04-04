using System;
using System.Collections.Generic;
using System.IO;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Wrappers;
using DXGame.Main;
using log4net;

namespace DXGame.Core.Generators
{
    public class MapGenerator : Generator<GameObject>
    {
        private const char RED_BLOCK = 'R';
        private const char GREEN_BLOCK = 'G';
        private const char BLUE_BLOCK = 'B';
        private const char PURPLE_BLOCK = 'P';
        private const char YELLOW_BLOCK = 'Y';
        private const char ORANGE_BLOCK = 'O';
        private const char PLAYER_CHARACTER = 'Z';
        private const string BASE_FOLDER = "Map/Blocks/";

        private const char LINE_FEED = (char) 10;
        private const char CARRIAGE_RETURN = (char) 13;
        private const int BLOCK_WIDTH = 50;

        private static readonly ILog LOG = LogManager.GetLogger(typeof (MapGenerator));

        private readonly List<GameObject> map_;
        private DxVector2 playerPosition_;
        private readonly DxGame game_;

        public MapGenerator(DxGame game, string mapPath)
        {
            game_ = game;
            map_ = InitMap(mapPath);
        }

        public DxVector2 PlayerPosition
        {
            get { return playerPosition_; }
        }

        public DxRectangle MapBounds { get; private set; }

        public static int BlockSize
        {
            get { return BLOCK_WIDTH; }
        }

        private List<GameObject> InitMap(string path)
        {
            LOG.Info(String.Format("Attempting to load map {0}", path));

            var blocks = new List<GameObject>();
            using (TextReader fileReader = File.OpenText(path))
            {
                int row = 0;
                int column = 0;
                int maxColumn = 0;
                int readValue;
                int numBlocks = 0;
                while ((readValue = fileReader.Read()) != -1)
                {
                    if (Char.MaxValue < readValue || Char.MinValue > readValue)
                    {
                        LOG.Warn(
                            String.Format(
                                "Read {0} from file, expected to be within bounds [{1}, {2}] at position ({3}, {4})",
                                readValue, Char.MinValue, Char.MaxValue, column, row));
                    }
                    else
                    {
                        var currentChar = (char) readValue;
                        if (CanCreateBlockFrom(currentChar))
                        {
                            string asset = ResolveCharacterToAsset(currentChar);
                            PositionalComponent position =
                                new SpatialComponent(game_).WithDimensions(new DxVector2(BLOCK_WIDTH, BLOCK_WIDTH))
                                    .WithPosition(column * BLOCK_WIDTH,
                                        row * BLOCK_WIDTH);
                            SimpleSpriteComponent sprite =
                                new SimpleSpriteComponent(game_).WithAsset(asset).WithPosition(position);
                            GameObject block = new GameObject().WithComponents(position, sprite);
                            blocks.Add(block);
                            ++numBlocks;
                        }
                        else if (CanCreatePlayerFrom(currentChar))
                        {
                            playerPosition_.X = column * BLOCK_WIDTH;
                            playerPosition_.Y = row * BLOCK_WIDTH;
                        }

                        switch (currentChar)
                        {
                        case LINE_FEED:
                            ++row;
                            maxColumn = Math.Max(column, maxColumn);
                            column = 0;
                            break;
                        case CARRIAGE_RETURN:
                            break;
                        default:
                            ++column;
                            break;
                        }
                    }
                }

                LOG.Info(String.Format("Loaded {0} map blocks.", numBlocks));

                MapBounds = new DxRectangle(0, 0, BLOCK_WIDTH * maxColumn, BLOCK_WIDTH * row);
            }

            return blocks;
        }

        private static string ResolveCharacterToAsset(char character)
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
            case ORANGE_BLOCK:
                assetString += "OrangeBlock";
                break;
            default:
                return "";
            }
            return assetString;
        }

        private static bool CanCreateBlockFrom(char character)
        {
            return ResolveCharacterToAsset(character) != "";
        }

        private static bool CanCreatePlayerFrom(char character)
        {
            return character == PLAYER_CHARACTER;
        }

        public override List<GameObject> Generate()
        {
            return map_;
        }
    }
}