using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Babel.Generators;
using Babel.Level;
using DxCore;
using DxCore.Core;
using DxCore.Core.Level;
using DxCore.Core.Services;

namespace Babel.Services
{
    public enum GameState
    {
        Playing,
        Paused
    }

    [Serializable]
    [DataContract]
    [Obsolete("Pls find a better way of injecting behavior into the game")]
    public class GameService : DxService
    {
        [DataMember]
        public float GameSpeed { get; set; }

        private MapService mapService_;

        protected override void OnCreate()
        {
            /* Since we're throwing this away, don't make it clean. Make it suck. */

            /* ... this sucks */
            ILevelProgressionStrategy levelProgression = new SimpleRotatingLevelProgression();
            levelProgression.Init();
            new PathfindingService().Create();
            mapService_ = new MapService(levelProgression);
            mapService_.Create();
            new EnvironmentService().Create();

            new ExperienceService().Create();
            new PlayerService().Create();

            if(DxGame.Instance.UpdateMode == UpdateMode.Active)
            {
                InitializePlayer();
            }
        }

        public void InitializePlayer()
        {
            BabelPlayerGenerator playerGenerator = new BabelPlayerGenerator(mapService_.PlayerSpawn);
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