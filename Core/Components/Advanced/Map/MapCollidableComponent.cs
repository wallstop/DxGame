using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Map;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Map
{
    [Serializable]
    [DataContract]
    public class MapCollidableComponent : CollidableComponent
    {
        [DataMember]
        public PlatformType PlatformType { get; }

        protected MapCollidableComponent(DxGame game, IList<CollidableDirection> collidableDirections,
            SpatialComponent spatial, PlatformType type)
            : base(game, collidableDirections, spatial)
        {
            PlatformType = type;
        }

        public new static MapCollidableComponentBuilder Builder()
        {
            return new MapCollidableComponentBuilder();
        }

        public class MapCollidableComponentBuilder : CollidableComponentBuilder
        {
            protected PlatformType type_;

            public MapCollidableComponentBuilder WithPlatformType(PlatformType type)
            {
                type_ = type;
                return this;
            }

            public override CollidableComponent Build()
            {
                Validate.IsNotNull(spatial_, StringUtils.GetFormattedNullOrDefaultMessage(this, spatial_));
                var game = DxGame.Instance;
                return new MapCollidableComponent(game, collidableDirections_, spatial_, type_);
            }
        }
    }
}