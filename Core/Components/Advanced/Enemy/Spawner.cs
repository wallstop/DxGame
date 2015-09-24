using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
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
        protected DxVector2 Position { get; }
        protected DxRectangle SpawnArea { get; }
        protected SpawnTrigger SpawnTrigger { get; }

        protected Spawner(DxGame game, DxVector2 position, DxRectangle spawnArea, SpawnTrigger spawnTrigger) 
            : base(game)
        {
            Position = position;
            SpawnArea = spawnArea;
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
            var minX = SpawnArea.X;
            var maxX = SpawnArea.X + SpawnArea.Width;
            var minY = SpawnArea.Y;
            var maxY = SpawnArea.Y + SpawnArea.Height;

            var xCoordinate = ThreadLocalRandom.Current.Next((int) minX, (int) maxX);
            var yCoordinate = ThreadLocalRandom.Current.Next((int) minY, (int) maxY);
            return new DxVector2(xCoordinate, yCoordinate);
        }

        public static SpawnerBuilder Builder()
        {
            return new SpawnerBuilder();
        }

        public class SpawnerBuilder : IBuilder<Spawner>
        {
            private DxVector2 position_;
            private DxRectangle spawnArea_;
            private SpawnTrigger spawnTrigger_;
            private DxGame game_;

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

            public Spawner Build()
            {
                Validate.IsNotNullOrDefault(spawnTrigger_, StringUtils.GetFormattedNullOrDefaultMessage(this, "SpawnTrigger"));
                return new Spawner(game_, position_, spawnArea_, spawnTrigger_);
            }
        }
    }
}
