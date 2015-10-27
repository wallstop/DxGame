using System.Collections.Generic;
using System.IO;
using System.Linq;
using DXGame.Core.Map;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DXGame.Core.Models
{
    public class MapModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        private static readonly string MAP_PATH = "Content/Map/";
        private readonly List<Map.Map> maps_ = new List<Map.Map>();
        public DxRectangle RandomSpawnLocation => Map.RandomSpawnLocation;
        public DxVector2 PlayerSpawn => Map.PlayerSpawn;
        public DxRectangle MapBounds => Map.MapDescriptor.Size * Map.MapDescriptor.Scale;
        public Map.Map Map { get; private set; }
        public override bool ShouldSerialize => false;

        public MapModel()
        {
            DrawPriority = DrawPriority.MAP;
        }

        public override void LoadContent()
        {
            maps_.Clear();
            var maps =
                Directory.EnumerateFiles(MAP_PATH)
                    .Where(
                        path =>
                            System.IO.Path.HasExtension(path) &&
                            (System.IO.Path.GetExtension(path)?.Equals(MapDescriptor.MapExtension) ?? false))
                    .Select(MapDescriptor.StaticLoad)
                    .Select(mapDescriptor => new Map.Map(mapDescriptor));
            maps_.AddRange(maps);
            Validate.IsNotEmpty(maps_, $"Failed to find maps! Check {MAP_PATH} for valid descriptors");
            base.LoadContent();
        }

        public override void Initialize()
        {
            // Figure out a better way of choosing the map
            Map = maps_[2]; // Always grab sample map (testing yay)
            Map.LoadContent();
            Map.Initialize();
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            Map.Draw(spriteBatch, gameTime);
            base.Draw(spriteBatch, gameTime);
        }
    }
}