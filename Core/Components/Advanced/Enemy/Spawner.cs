using System;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Enemy
{
    /*
        Simple function that, given a GameTime, returns true if a GameObject should be spawned there
        along with the GameObject that should be spawned. If the first element in the tuple is false,
        the GameObject reference is undefined.
    */

    public delegate Tuple<bool, GameObject> SpawnTrigger(DxGameTime gameTime);

    [Serializable]
    public class Spawner : Component
    {
        protected SpawnTrigger SpawnTrigger { get; }

        protected Spawner(DxGame game, DxVector2 position, DxRectangle spawnArea, SpawnTrigger spawnTrigger)
            : base(game)
        {
            SpawnTrigger = spawnTrigger;
        }

        protected override void Update(DxGameTime gameTime)
        {
            var spawnCheck = SpawnTrigger(gameTime);
            if (spawnCheck.Item1)
            {
                var spawnedObject = spawnCheck.Item2;
                var position = spawnedObject.ComponentOfType<PositionalComponent>();
                position.Position = RandomPositionInSpawnArea();
                DxGame.AddAndInitializeGameObject(spawnedObject);
            }
            base.Update(gameTime);
        }

        private DxVector2 RandomPositionInSpawnArea()
        {
            var map = DxGame.Model<MapModel>();
            var spawnArea = map.RandomSpawnLocation;
            var minX = spawnArea.X;
            var maxX = spawnArea.X + spawnArea.Width;
            var minY = spawnArea.Y;
            var maxY = spawnArea.Y + spawnArea.Height;

            var xCoordinate = ThreadLocalRandom.Current.NextFloat(minX, maxX);
            var yCoordinate = ThreadLocalRandom.Current.NextFloat( minY, maxY);
            return new DxVector2(xCoordinate, yCoordinate);
        }

        public static SpawnerBuilder Builder()
        {
            return new SpawnerBuilder();
        }

        public class SpawnerBuilder : IBuilder<Spawner>
        {
            private DxGame game_;
            private DxVector2 position_;
            private DxRectangle spawnArea_;
            private SpawnTrigger spawnTrigger_;

            public Spawner Build()
            {
                Validate.IsNotNullOrDefault(spawnTrigger_,
                    StringUtils.GetFormattedNullOrDefaultMessage(this, "SpawnTrigger"));
                return new Spawner(game_, position_, spawnArea_, spawnTrigger_);
            }

            public SpawnerBuilder WithPosition(DxVector2 position)
            {
                position_ = position;
                return this;
            }

            public SpawnerBuilder WithSpawnArea(DxRectangle spawnArea)
            {
                spawnArea_ = spawnArea;
                return this;
            }

            public SpawnerBuilder WithSpawnTrigger(SpawnTrigger trigger)
            {
                spawnTrigger_ = trigger;
                return this;
            }

            public SpawnerBuilder WithGame(DxGame game)
            {
                Validate.IsNotNullOrDefault(game, StringUtils.GetFormattedNullOrDefaultMessage(this, game));
                game_ = game;
                return this;
            }
        }
    }
}