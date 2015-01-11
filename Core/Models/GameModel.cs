using System.Linq;
using System.Runtime.InteropServices;
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

        public SpatialComponent FocalPoint { get; protected set; }

        protected SpriteBatch SpriteBatch { get; set; }

        public GameModel(DxGame game) : base(game)
        {
        }

        public override void Initialize()
        {
            MapModel = MapModel.InitializeFromGenerator(DxGame, new MapGenerator(DxGame, "Content/Map/SimpleMap.txt"));
            var worldGravity = new WorldGravityModel(DxGame);
            PlayerGenerator playerGenerator = new PlayerGenerator(DxGame, MapModel.PlayerPosition, MapModel.MapBounds);
            FocalPoint = playerGenerator.PlayerSpace;
            var player = playerGenerator.Generate().First();
            var physicsComponents = player.ComponentsOfType<PhysicsComponent>();
            foreach (var physicsComponent in physicsComponents)
            {
                worldGravity.AttachPhysicsComponent(physicsComponent);
            }

            DxGame.AddAndInitializeGameObjects(MapModel.MapObjects);
            DxGame.AddAndInitializeGameObjects(playerGenerator.Generate());
            DxGame.AddAndInitializeComponent(worldGravity);
            DxGame.AttachModel(this);
            DxGame.AttachModel(MapModel);
            DxGame.AttachModel(worldGravity);
            base.Initialize();
        }

        /*
            Since we can't properly control how we add/remove each component from the gamestate,
            we entrust the runtime to call dispose, which is where we remove all of our added components
        */
        // TODO:
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
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