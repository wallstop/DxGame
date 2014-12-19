using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DXGame.Core.Models;
using log4net;
using Microsoft.Xna.Framework;

namespace DXGame.Core
{
    public class GameState : GameComponentCollection
    {
        public float GameSpeed { get; set; }

        public GameState(Game game) : base(game)
        {
        }
    }


    //// TODO: Make this class thredsafe
    //public static class GameState
    //{
    //    private static readonly ILog LOG = LogManager.GetLogger(typeof(GameState));

    //    private static readonly List<Model> models_ = new List<Model>();

    //    public static T Model<T>() where T : Model
    //    {
    //        return models_.OfType<T>().FirstOrDefault();
    //    }

    //    public static bool AttachModel(Model model)
    //    {
    //        bool alreadyContainsModel = models_.Contains(model);
    //        Debug.Assert(!alreadyContainsModel, String.Format("GameState already contains {0}", model));
    //        if (!alreadyContainsModel)
    //        {
    //            models_.Add(model);
    //        }

    //        return !alreadyContainsModel;
    //    }

    //    public static bool RemoveModel(Model model)
    //    {
    //        bool alreadyContainsModel = models_.Remove(model);
    //        Debug.Assert(alreadyContainsModel, String.Format("GameState doesn't already contain {0}", model));
    //        return alreadyContainsModel;
    //    }
    //}
}
