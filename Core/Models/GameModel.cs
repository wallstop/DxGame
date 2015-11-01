using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Entities;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Components.Developer;
using DXGame.Core.Generators;
using DXGame.Core.GraphicsWidgets.HUD;
using DXGame.Core.Utils;
using DXGame.Main;
using DXGame.Core.Messaging;
using NLog;
using NLog.Fluent;

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
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [DataMember]
        public float GameSpeed { get; set; }
        [DataMember]
        public SpatialComponent FocalPoint { get; protected set; }

        protected override void DeSerialize()
        {
            // Do nothing with this, this Initialize will do all kinds of bad things (re-trigger map model spawning, for example)
        }

        public GameModel()
        {
            MessageHandler.RegisterMessageHandler<MapRotationRequest>(HandleMapRotationRequest);
        }

        private void HandleMapRotationRequest(MapRotationRequest mapRotationRequest)
        {
            PlayerModel playerModel = DxGame.Instance.Model<PlayerModel>();
            if (ReferenceEquals(playerModel, null))
            {
                LOG.Info(
                    $"Received a {typeof (MapRotationRequest)}, but the {typeof (PlayerModel)} was not found, ignoring.");
                return;
            }
            List<Player> players = playerModel.Players.ToList();
            HashSet<object> thingsToKeep = new HashSet<object>();
            foreach (Component component in players.Select(player => player.Object).SelectMany(playerObject => playerObject.Components))
            {
                thingsToKeep.Add(component);
            }
            foreach(Player player in players)
            {
                thingsToKeep.Add(player.Object);
            }

            GameElementCollection gameElements = DxGame.Instance.DxGameElements;
            // TODO: Figure out a better way, this is crap
            foreach (var gameElement in gameElements.Cast<object>().Where(gameElement => !thingsToKeep.Contains(gameElement) && !(gameElement is SpriteBatchInitializer) && !(gameElement is SpriteBatchEnder) && !(gameElement is Model) && !(gameElement is Spawner) && !(gameElement is TimePerFrameGraph)))
            {
                DxGame.Instance.Remove(gameElement);
            }
        }

        public override void Initialize()
        {
            var mapModel = new MapModel();
            DxGame.Instance.AttachModel(mapModel);
            var developerModel = new DeveloperModel();
            DxGame.Instance.AttachModel(developerModel);
            var collisionModel = new CollisionModel();
            DxGame.Instance.AttachModel(collisionModel);
            EnvironmentModel environmentModel = new EnvironmentModel();
            DxGame.Instance.AttachModel(environmentModel);
            var pathfindingModel = new PathfindingModel();
            DxGame.Instance.AttachModel(pathfindingModel);
            EventModel eventModel = new EventModel();
            DxGame.Instance.AttachModel(eventModel);
            PlayerGenerator playerGenerator = new PlayerGenerator(mapModel.PlayerSpawn,
                mapModel.MapBounds);
            FocalPoint = playerGenerator.PlayerSpace;
            var generatedObjects = playerGenerator.Generate();
            var player = generatedObjects.First();

            var activePlayer = Player.PlayerFrom(player, "Player1");
            // TODO
            var playerModel = new PlayerModel().WithActivePlayer(activePlayer);
            DxGame.Instance.AttachModel(playerModel);

            // TODO: Split these out into some kind of unified loading... thing
            DxGame.Instance.AddAndInitializeGameObjects(generatedObjects);
            base.Initialize();
        }
    }
}