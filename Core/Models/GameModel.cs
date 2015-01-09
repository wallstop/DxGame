using DXGame.Core.Components.Advanced;
using DXGame.Core.Generators;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public GameModel(DxGame game) : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            MapModel = MapModel.InitializeFromGenerator(Game, new MapGenerator(DxGame, "Content/Map/SimpleMap.txt"));

            PlayerGenerator playerGenerator = new PlayerGenerator(DxGame, MapModel.PlayerPosition, MapModel.MapBounds);
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
            we entrust the runtime to call dispose, which is where we remove all of our added components
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
}