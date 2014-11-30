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

namespace DXGame.Core
{
    public class SimpleMap
    {
        private static readonly log4net.ILog LOG =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<SimpleBlock> blocks_ = new List<SimpleBlock>();

        private const char LINE_FEED = (char) 10;
        private const char CARRIAGE_RETURN = (char) 13;

        private Vector2 playerPosition_ = new Vector2();

        public SimpleMap(string path)
        {
            InitMap(path);
        }

        public void InitMap(string path)
        {
            LOG.Info(String.Format("Attempting to load map %s", path));

            using (TextReader fileReader = File.OpenText(path))
            {
                int row = 0;
                int column = 0;
                int readValue = -1;
                while ((readValue = fileReader.Read()) != -1)
                {
                    if (Char.MaxValue < readValue || Char.MinValue > readValue)
                    {
                        LOG.Warn(
                            String.Format(
                                "Read %d from file, expected to be within bounds [%d, %d] at position (%d, %d)",
                                readValue, Char.MinValue, Char.MaxValue, column, row));
                    }
                    else
                    {
                        var currentChar = (char) readValue;
                        if (SimpleBlock.CanCreateFrom(currentChar))
                        {
                            var block = new SimpleBlock(column, row, currentChar);
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
            }
        }

        public Vector2 GetPlayerPosition()
        {
            return playerPosition_;
        }

        public void Load(ContentManager contentManager)
        {
            foreach (var block in blocks_)
            {
                block.LoadContent(contentManager);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var block in blocks_)
            {
                block.Draw(spriteBatch);
            }
        }
    }
}