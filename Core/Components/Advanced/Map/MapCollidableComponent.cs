using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Map;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Map
{
    /**
        <summary>
            This is a CollidableComponent of type Map. Not a Component for Colliding with the Map
        </summary>
        TODO: Rename this, it is garbage
    */
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