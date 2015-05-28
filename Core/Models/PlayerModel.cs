using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Skills;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;

namespace DXGame.Core.Models
{
    [DataContract]
    [Serializable]
    public class PlayerModel : Model
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (PlayerModel));

        [DataMember]
        public string PlayerName { get; private set; }

        [DataMember]
        public IEnumerable<ISkill> Skills { get; private set; }

        public PlayerModel(DxGame game)
            : base(game)
        {
        }

        public PlayerModel WithName(string name)
        {
            Validate.IsNotNullOrDefault(name, $"Cannot initialize {GetType()} with a null/default name ({name})");
            PlayerName = name;
            return this;
        }

        public PlayerModel WithSkills(IEnumerable<ISkill> skills)
        {
            var skillsAsArray = skills as ISkill[] ?? skills.ToArray();
            Validate.IsNotEmpty(skillsAsArray, $"Cannot initialize {GetType()} with a null/default skill collection");
            Skills = skillsAsArray;
            return this;
        }
    }
}