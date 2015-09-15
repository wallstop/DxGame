using System;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Generators;
using DXGame.Core.GraphicsWidgets.HUD;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Models
{
    public enum GameState
    {
        Playing,
        Paused
    }

    [Serializable]
    [DataContract]
    public class GameModel : Model
    {
        [DataMember]
        public float GameSpeed { get; set; }
        [DataMember]
        public SpatialComponent FocalPoint { get; protected set; }

        public GameModel(DxGame game) : base(game)
        {
        }

        protected override void DeSerialize()
        {
            // Do nothing with this, this Initialize will do all kinds of bad things (re-trigger map model spawning, for example)
        }

        public override void Initialize()
        {
            var mapModel = new MapModel(DxGame);
            DxGame.AttachModel(mapModel);
            PlayerGenerator playerGenerator = new PlayerGenerator(DxGame, mapModel.PlayerSpawn,
                mapModel.MapBounds);
            FocalPoint = playerGenerator.PlayerSpace;
            var generatedObjects = playerGenerator.Generate();
            var player = generatedObjects.First();

            var activePlayer = Player.PlayerFrom(player, "Player1");
            // TODO
            var playerModel = new PlayerModel(DxGame).WithActivePlayer(activePlayer);
            DxGame.AttachModel(playerModel);

            var fpsTracker = new FpsTracker(DxGame);
            DxGame.AddAndInitializeComponent(fpsTracker);
            var physicsComponents = player.ComponentsOfType<PhysicsComponent>();


            foreach (var physicsComponent in physicsComponents)
            {
                GravityApplier.ApplyGravityToPhysics(physicsComponent);
            }

            // TODO: Split these out into some kind of unified loading... thing
            DxGame.AddAndInitializeGameObjects(generatedObjects);
            base.Initialize();
        }
    }
}