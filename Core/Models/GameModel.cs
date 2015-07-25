using System.Linq;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Generators;
using DXGame.Core.GraphicsWidgets.HUD;
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
            PlayerGenerator playerGenerator = new PlayerGenerator(DxGame, mapModel.PlayerPosition,
                mapModel.MapBounds);
            FocalPoint = playerGenerator.PlayerSpace;
            var player = playerGenerator.Generate().First();

            var activePlayer = Player.PlayerFrom(player);
            // TODO
            //var playerModel = new PlayerModel(DxGame).WithActivePlayer(activePlayer);
            //DxGame.AttachModel(playerModel);

            var fpsTracker = new FpsTracker(DxGame);
            DxGame.AddAndInitializeComponent(fpsTracker);
            var physicsComponents = player.ComponentsOfType<PhysicsComponent>();


            foreach (var physicsComponent in physicsComponents)
            {
                var component = physicsComponent;
                physicsComponent.AddPostUpdater(
                    gameTime => WorldGravity.ApplyGravityToPhysics(component));
            }

            // TODO: Split these out into some kind of unified loading... thing
            DxGame.AddAndInitializeGameObjects(playerGenerator.Generate());
            DxGame.AttachModel(mapModel);
            base.Initialize();
        }
    }
}