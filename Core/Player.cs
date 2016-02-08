using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Entities;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Utils;
using ProtoBuf;

namespace DXGame.Core
{
    /**
        <summary>
            Simple Player-specific abstraction around GameObjects. 
            Useful for getting known components out of Players in a standardized fashion.
        </summary>
    */

    [Serializable]
    [DataContract]
    [ProtoContract]
    public class Player
    {
        [DataMember]
        [ProtoMember(1)]
        public GameObject Object { get; }

        public SpatialComponent Position => Object.ComponentOfType<SpatialComponent>();
        public EntityPropertiesComponent Properties => Object.ComponentOfType<EntityPropertiesComponent>();
        public PhysicsComponent Physics => Object.ComponentOfType<PhysicsComponent>();
        public AnimationComponent Animation => Object.ComponentOfType<AnimationComponent>();
        public SkillComponent Abilities => Object.ComponentOfType<SkillComponent>();
        public LevelComponent Level => Object.ComponentOfType<LevelComponent>();

        [DataMember]
        [ProtoMember(2)]
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