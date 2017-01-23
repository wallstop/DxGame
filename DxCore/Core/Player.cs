using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Animation;
using DxCore.Core.Components.Advanced.Entities;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Player;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Utils;
using WallNetCore.Validate;

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
        public SkillComponent Abilities => Object.ComponentOfType<SkillComponent>();
        public AnimatedComponent Animation => Object.ComponentOfType<AnimatedComponent>();
        public EntityLevelComponent EntityLevel => Object.ComponentOfType<EntityLevelComponent>();

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public GameObject Object { get; }

        public PhysicsComponent Physics => Object.ComponentOfType<PhysicsComponent>();

        public PhysicsComponent Position => Object.ComponentOfType<PhysicsComponent>();
        public EntityPropertiesComponent Properties => Object.ComponentOfType<EntityPropertiesComponent>();

        private Player(GameObject gameObject)
        {
            Object = gameObject;
        }

        public static Player PlayerFrom(GameObject existingPlayer, string name)
        {
            Validate.Hard.IsNotNull(existingPlayer, StringUtils.GetFormattedNullOrDefaultMessage(typeof(Player), name));
            return new Player(existingPlayer) {Name = name};
        }
    }
}