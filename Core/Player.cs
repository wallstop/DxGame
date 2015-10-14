using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Utils;

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
    public class Player
    {
        [DataMember] private readonly GameObject player_;
        public SpatialComponent Position => player_.ComponentOfType<SpatialComponent>();
        public PlayerPropertiesComponent Properties => player_.ComponentOfType<PlayerPropertiesComponent>();
        public PhysicsComponent Physics => player_.ComponentOfType<PhysicsComponent>();
        public AnimationComponent Animation => player_.ComponentOfType<AnimationComponent>();
        public SkillComponent Abilities => player_.ComponentOfType<SkillComponent>();

        [DataMember]
        public string Name { get; private set; }

        private Player(GameObject gameObject)
        {
            player_ = gameObject;
        }

        public static Player PlayerFrom(GameObject existingPlayer, string name)
        {
            Validate.IsNotNull(existingPlayer, StringUtils.GetFormattedNullOrDefaultMessage(typeof (Player), name));
            return new Player(existingPlayer) {Name = name};
        }
    }
}