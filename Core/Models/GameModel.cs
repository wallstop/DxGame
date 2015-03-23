using System.Linq;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Generators;
using DXGame.Main;

namespace DXGame.Core.Models
{
    public enum GameState
    {
        Playing,
        Paused
    }

    public class GameModel : Model
    {
        public float GameSpeed { get; set; }

        public SpatialComponent FocalPoint { get; protected set; }

        public GameModel(DxGame game) : base(game)
        {
        }

        public override void Initialize()
        {
            var mapGenerator = new MapGenerator(DxGame, "Content/Map/SimpleMap.txt");
            var mapModel = MapModel.InitializeFromGenerator(DxGame, mapGenerator);
            var worldGravity = new WorldGravityModel(DxGame);
            PlayerGenerator playerGenerator = new PlayerGenerator(DxGame, mapModel.PlayerPosition, mapModel.MapBounds);
            FocalPoint = playerGenerator.PlayerSpace;
            var player = playerGenerator.Generate().First();
            var physicsComponents = player.ComponentsOfType<PhysicsComponent>();
            foreach (var physicsComponent in physicsComponents)
            {
                worldGravity.AttachPhysicsComponent(physicsComponent);
            }

            // TODO: Split these out into some kind of unified loading... thing
            DxGame.AddAndInitializeGameObjects(playerGenerator.Generate());
            DxGame.AttachModel(mapModel);
            DxGame.AttachModel(worldGravity);
            base.Initialize();
        }
    }
}