using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Entities;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Advanced.Properties;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core
{
    /**
        <summary>
            Simple Player-specific abstraction around GameObjects. 
            Useful for getting known components out of Players in a standardized fashion.
        </summary>
    */

    [Serializable]
    [DataContract]
    public class Player
    {
        [DataMember]
        public GameObject Object { get; }

        public SpatialComponent Position => Object.ComponentOfType<SpatialComponent>();
        public EntityPropertiesComponent Properties => Object.ComponentOfType<EntityPropertiesComponent>();
        public PhysicsComponent Physics => Object.ComponentOfType<PhysicsComponent>();
        public AnimationComponent Animation => Object.ComponentOfType<AnimationComponent>();
        public SkillComponent Abilities => Object.ComponentOfType<SkillComponent>();
        public LevelComponent Level => Object.ComponentOfType<LevelComponent>();

        [DataMember]
        public string Name { get; private set; }

        private Player(GameObject gameObject)
        {
            Object = gameObject;
        }

        public static Player PlayerFrom(GameObject existingPlayer, string name)
        {
            Validate.IsNotNull(existingPlayer, StringUtils.GetFormattedNullOrDefaultMessage(typeof(Player), name));
            return new Player(existingPlayer) {Name = name};
        }
    }
}