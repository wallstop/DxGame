using System;
using System.Runtime.Serialization;
using DxCore.Core.Generators;
using NLog;

namespace DxCore.Core.Models
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

        private MapModel mapModel_;

        public override void DeSerialize()
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

            mapModel_ = new MapModel(levelProgression);
            DxGame.Instance.AttachModel(mapModel_);
            DeveloperModel developerModel = new DeveloperModel();
            DxGame.Instance.AttachModel(developerModel);
            CollisionModel collisionModel = new CollisionModel();
            DxGame.Instance.AttachModel(collisionModel);
            EnvironmentModel environmentModel = new EnvironmentModel();
            DxGame.Instance.AttachModel(environmentModel);
            PathfindingModel pathfindingModel = new PathfindingModel();
            DxGame.Instance.AttachModel(pathfindingModel);
            ExperienceModel experienceModel = new ExperienceModel();
            DxGame.Instance.AttachModel(experienceModel);
            PlayerModel playerModel = new PlayerModel();
            DxGame.Instance.AttachModel(playerModel);

            if(DxGame.Instance.UpdateMode == UpdateMode.Active)
            {
                InitializePlayer();
            }
            base.Initialize();
        }

        public void InitializePlayer()
        {
            PlayerGenerator playerGenerator = new PlayerGenerator(mapModel_.PlayerSpawn);
            var generatedObjects = playerGenerator.Generate();

            // TODO: We need to throw this away if we're doing a network game
            // TODO: Split these out into some kind of unified loading... thing
            foreach(GameObject generatedObject in generatedObjects)
            {
                generatedObject.Create();
            }
        }
    }
}