using System.Collections.Generic;
using System.IO;
using System.Linq;
using DXGame.Core.Map;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using NLog;

namespace DXGame.Core.Models
{
    public class MapModelv2 : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        private static readonly string MAP_PATH = "Content/Map/";
        private readonly List<Map.Map> maps_ = new List<Map.Map>();
        public DxVector2 PlayerSpawn { get; }
        public DxRectangle MapBounds { get; }
        public Map.Map Map { get; private set; }

        public MapModelv2(DxGame game)
            : base(game)
        {
        }

        public override void LoadContent()
        {
            maps_.Clear();
            var descriptors =
                Directory.EnumerateFiles(MAP_PATH)
                    .Where(path => Path.HasExtension(MapDescriptor.Extension))
                    .Select(Serializer<MapDescriptor>.ReadFromJsonFile)
                    .Select(mapDescriptor => new Map.Map(DxGame, mapDescriptor));
            maps_.AddRange(descriptors);
            Validate.IsNotEmpty(maps_, $"Failed to find maps! Check {MAP_PATH} for valid descriptors");
            // Figure out a better way of choosing the map
            Map = maps_[0];
            DxGame.AddAndInitializeComponents(maps_);
            base.LoadContent();
        }
    }
}