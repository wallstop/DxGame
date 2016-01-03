using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Entities;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using NLog.LayoutRenderers;

namespace DXGame.Core.Level
{
    [DataContract]
    [Serializable]
    public class Level : Component
    {
        public Map.Map Map { get; }

        public TimeSpan LevelTime { get; private set; }

        public List<Spawner> Spawners { get; } 

        private readonly ManagerComponent entityManager_ = new ManagerComponent();

        public void OnLoad() {}

        private Level(Map.Map map, IEnumerable<Spawner> spawners)
        {
            Map = map;
            Spawners = spawners.ToList();
            LevelTime = TimeSpan.Zero;
        }

        protected override void Update(DxGameTime gameTime)
        {
            LevelTime += gameTime.ElapsedGameTime;
        }

        public override void Dispose()
        {
            entityManager_.Dispose();
            Spawners.ForEach(spawner => spawner.Dispose());
        }

        public class Builder : IBuilder<Level>
        {
            private Map.Map map_;
            private readonly HashSet<Spawner> spawners_ = new HashSet<Spawner>(); 

            public Builder WithMap(Map.Map map)
            {
                map_ = map;
                return this;
            }

            public Builder WithSpawner(Spawner spawner)
            {
                Validate.IsNotNull(spawner);
                spawners_.Add(spawner);
                return this;
            }

            public Level Build()
            {
                Validate.IsNotNull(map_, StringUtils.GetFormattedNullOrDefaultMessage(this, map_));
                return new Level(map_, spawners_);
            }
        }
    }
}
