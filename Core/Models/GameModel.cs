using System;
using System.Collections.Generic;
using System.Drawing;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Basic;
using DXGame.Core.Generators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace DXGame.Core.Models
{
    public enum GameState
    {
        Playing,
        Paused
    }

    public class GameModel : GameComponentCollection
    {
        public float GameSpeed { get; set; }

        public MapModel MapModel { get; set; }

        protected SpatialComponent FocalPoint { get; set; }

        protected Rectangle Screen { get; set; }

        protected SpriteBatch SpriteBatch { get; set; }

        public GameModel(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            MapModel = MapModel.InitializeFromGenerator(Game, new MapGenerator(Game, "Content/Map/SimpleMap.txt"));

            PlayerGenerator playerGenerator = new PlayerGenerator(Game, MapModel.PlayerPosition, MapModel.MapBounds);
            FocalPoint = playerGenerator.PlayerSpace;
            AddGameObjects(MapModel.MapObjects);
            AddGameObjects(playerGenerator.Generate());

            Screen = new Rectangle(0, 0, 1280, 720);
            var graphics = new GraphicsDeviceManager(Game);
            graphics.PreferredBackBufferHeight = Screen.Height;
            graphics.PreferredBackBufferWidth = Screen.Width;

            Game.Content.RootDirectory = "Content";

            foreach (GameComponent component in Components)
            {
                Game.Components.Add(component);
            }
        }

        /*
            Since we can't properly control how we add/remove each component from the gamestate,
            we entrust the runtime to call dispose, which is where we remove all
        */
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            foreach (GameComponent component in Components)
            {
                Game.Components.Remove(component);
            }
        }

        public bool AddComponent(GameComponent component)
        {
            bool alreadyExists = Components.Contains(component);
            if (!alreadyExists)
            {
                Components.Add(component);
            }
            return alreadyExists;
        }

        public bool RemoveComponent(GameComponent component)
        {
            return Components.Remove(component);
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
