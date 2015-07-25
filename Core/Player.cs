using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Skills;
using DXGame.Core.Utils;

namespace DXGame.Core
{
    [DataContract]
    [Serializable]
    public class Player : GameObject
    {
        /*
            We're making the executive design decision to limit players to 4 skills
        */
        private static readonly int NUM_SKILLS = 4;
        public SpatialComponent Position => ComponentOfType<SpatialComponent>();
        public PlayerPropertiesComponent PlayerProperties => ComponentOfType<PlayerPropertiesComponent>();

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public IEnumerable<ISkill> Skills { get; private set; }

        public static Player PlayerFrom(GameObject existingPlayer)
        {
            Validate.IsNotNull(existingPlayer, "Cannot create a Player from a null existing player");

            return (Player) new Player().WithComponents(existingPlayer.Components);
        }

        public Player WithName(string name)
        {
            Validate.IsNotNullOrDefault(name, $"Cannot initialize {GetType()} with a null/default name ({name})");
            Name = name;
            return this;
        }

        public Player WithSkills(ISkill[] skills)
        {
            var skillsAsArray = skills ?? skills.ToArray();
            Validate.IsNotEmpty(skillsAsArray, $"Cannot initialize {GetType()} with a null/default skill collection");
            Validate.IsTrue(skillsAsArray.Length == NUM_SKILLS,
                $"Cannot initialize {GetType()} with {skillsAsArray.Length} skills)");
            Skills = skillsAsArray;
            return this;
        }
    }
}