using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using NLog;

namespace DXGame.Core.Map
{
    /**
        <summary>
            Intended-to-be serializable description of a Map. 
            This should generally translate 1:1 with loaded Map objects. 
            Stored nice and json-ey.
        </summary>
    */
    [Serializable]
    [DataContract]
    public class MapDescriptor : JsonPersistable<MapDescriptor>
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        public static string MapExtension => ".mdtr";
        [IgnoreDataMember]
        public override string Extension => MapExtension;
        [IgnoreDataMember]
        public override MapDescriptor Item => this;

        [DataMember]
        public string Asset { get; set; }

        [DataMember]
        public List<Platform> Platforms { get; set; } = new List<Platform>();

        [DataMember]
        public DxRectangle Size { get; set; }

        [DataMember]
        public float Scale { get; set; } = 1.0f;

        public static MapDescriptorBuilder Builder()
        {
            return new MapDescriptorBuilder();
        }

        public class MapDescriptorBuilder : IBuilder<MapDescriptor>
        {
            private string asset_;
            private readonly List<Platform> platforms_ = new List<Platform>();
            private DxRectangle size_;
            private float scale_ = 1.0f;

            public MapDescriptorBuilder WithScale(float scale)
            {
                scale_ = scale;
                return this;
            }

            public MapDescriptorBuilder WithSize(DxRectangle size)
            {
                size_ = size;
                return this;
            }

            public MapDescriptorBuilder WithPlatform(params Platform[] platforms)
            {
                platforms_.AddRange(platforms);
                return this;
            }

            public MapDescriptorBuilder WithAsset(string asset)
            {
                asset_ = asset;
                return this;
            }

            public MapDescriptor Build()
            {
                Validate.IsTrue(scale_ > 0, $"Cannot create a {typeof(MapDescriptor)} with a non-positive scale");
                if(Check.IsNullOrDefault(asset_))
                {
                    LOG.Warn($"Creating a {typeof(Map)} without an asset, this will result in an empty map");
                }
                return new MapDescriptor {Asset = asset_, Platforms = platforms_.ToList(), Scale = scale_, Size = size_};
            }
        }
    }
}