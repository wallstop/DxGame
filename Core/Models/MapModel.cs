using System.Collections.Generic;
using System.IO;
using System.Linq;
using DXGame.Core.Map;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
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
        public DxVector2 PlayerSpawn => Map.PlayerSpawn;
        public DxRectangle MapBounds => Map.MapDescriptor.Size;
        public Map.Map Map { get; private set; }

        public MapModel(DxGame game)
            : base(game)
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
                            Path.HasExtension(path) &&
                            (Path.GetExtension(path)?.Equals(MapDescriptor.MapExtension) ?? false))
                    .Select(MapDescriptor.StaticLoad)
                    .Select(mapDescriptor => new Map.Map(DxGame, mapDescriptor));
            maps_.AddRange(maps);
            Validate.IsNotEmpty(maps_, $"Failed to find maps! Check {MAP_PATH} for valid descriptors");
            base.LoadContent();
        }

        public override void Initialize()
        {
            // Figure out a better way of choosing the map
            Map = maps_[1];
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