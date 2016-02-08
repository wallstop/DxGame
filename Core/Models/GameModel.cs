﻿using System;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Generators;
using DXGame.Main;
using DXGame.TowerGame.Level;
using NLog;
using ProtoBuf;

namespace DXGame.Core.Models
{
    public enum GameState
    {
        Playing,
        Paused
    }

    [Serializable]
    [DataContract]
    [ProtoContract]
    public class GameModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [ProtoMember(1)]
        [DataMember]
        public float GameSpeed { get; set; }

        [ProtoMember(2)]
        [DataMember]
        public SpatialComponent FocalPoint { get; protected set; }

        protected override void DeSerialize()
        {
            // Do nothing with this, this Initialize will do all kinds of bad things (re-trigger map model spawning, for example)
        }

        public override void Initialize()
        {
            EventModel eventModel = new EventModel();
            DxGame.Instance.AttachModel(eventModel);

            SimpleRotatingLevelProgression levelProgression = new SimpleRotatingLevelProgression();
            levelProgression.LoadContent();
            levelProgression.Initialize();

            var mapModel = new MapModel(levelProgression);
            DxGame.Instance.AttachModel(mapModel);
            var developerModel = new DeveloperModel();
            DxGame.Instance.AttachModel(developerModel);
            var collisionModel = new CollisionModel();
            DxGame.Instance.AttachModel(collisionModel);
            EnvironmentModel environmentModel = new EnvironmentModel();
            DxGame.Instance.AttachModel(environmentModel);
            var pathfindingModel = new PathfindingModel();
            DxGame.Instance.AttachModel(pathfindingModel);
            ExperienceModel experienceModel = new ExperienceModel();
            DxGame.Instance.AttachModel(experienceModel);

            PlayerGenerator playerGenerator = new PlayerGenerator(mapModel.PlayerSpawn, mapModel.MapBounds);
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