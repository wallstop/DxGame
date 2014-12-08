using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXGame.Core.Models;
using log4net;

namespace DXGame.Core
{
    public class GameState
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(GameState));

        private readonly List<Model> models_;

        public GameState()
        {
            models_ = new List<Model>();
        }

        public T Model<T>() where T : Model
        {
            return models_.OfType<T>().FirstOrDefault();
        }
    }
}
