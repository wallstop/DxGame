using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Babel.Generators;
using Babel.Level;
using DxCore;
using DxCore.Core;
using DxCore.Core.Level;
using DxCore.Core.Models;

namespace Babel.Models
{
    public enum GameState
    {
        Playing,
        Paused
    }

    [Serializable]
    [DataContract]
    [Obsolete("Pls find a better way of injecting behavior into the game")]
    public class GameModel : Model
    {
        [DataMember]
        public float GameSpeed { get; set; }

        private MapModel mapModel_;

        public override void DeSerialize()
        {
            // Do nothing with this, this Initialize will do all kinds of bad things (re-trigger map model spawning, for example)
        }

        public override void Initialize()
        {

            new EventModel().Create();

            ILevelProgressionStrategy levelProgression = new SimpleRotatingLevelProgression();
            levelProgression.Init();

            mapModel_ = new MapModel(levelProgression);
            mapModel_.Create();
            new EnvironmentModel().Create();
            new PathfindingModel().Create();
            new ExperienceModel().Create();
            new PlayerModel().Create();

            if(DxGame.Instance.UpdateMode == UpdateMode.Active)
            {
                InitializePlayer();
            }
            base.Initialize();
        }

        public void InitializePlayer()
        {
            BabelPlayerGenerator playerGenerator = new BabelPlayerGenerator(mapModel_.PlayerSpawn);
            List<GameObject> generatedObjects = playerGenerator.Generate();

            // TODO: We need to throw this away if we're doing a network game
            // TODO: Split these out into some kind of unified loading... thing
            foreach(GameObject generatedObject in generatedObjects)
            {
                generatedObject.Create();
            }
        }
    }
}