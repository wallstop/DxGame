using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Simple
{
    public class SimpleMap
    {
        private static readonly log4net.ILog LOG =
            log4net.LogManager.GetLogger(typeof(SimpleMap));

        private readonly List<SimpleBlock> blocks_ = new List<SimpleBlock>();

        private const char LINE_FEED = (char) 10;
        private const char CARRIAGE_RETURN = (char) 13;

        private Vector2 playerPosition_;
        private Vector2 mapSize_;

        public SimpleMap(string path)
        {
            InitMap(path);
        }

        public void InitMap(string path)
        {
            LOG.Info(String.Format("Attempting to load map {0}", path));

            using (TextReader fileReader = File.OpenText(path))
            {
                int row = 0;
                int column = 0;
                int maxColumn = 0;
                int readValue;
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
                        char currentChar = (char) readValue;
                        if (SimpleBlock.CanCreateFrom(currentChar))
                        {
                            SimpleBlock block = new SimpleBlock(column, row, currentChar);
                            blocks_.Add(block);
                        }
                        else if (SimplePlayer.CanCreateFrom((currentChar)))
                        {
                            playerPosition_.X = column;
                            playerPosition_.Y = row;
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

                mapSize_.X = SimpleSprite.GetBlockWidth()*maxColumn;
                mapSize_.Y = SimpleSprite.GetBlockWidth()*row;
            }
        }

        public Vector2 GetMapSize()
        {
            return mapSize_;
        }

        public Vector2 GetPlayerPosition()
        {
            return playerPosition_;
        }

        public void Load(ContentManager contentManager)
        {
            foreach (SimpleBlock block in blocks_)
            {
                block.LoadContent(contentManager);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (SimpleBlock block in blocks_)
            {
                block.Draw(spriteBatch);
            }
        }
    }
}