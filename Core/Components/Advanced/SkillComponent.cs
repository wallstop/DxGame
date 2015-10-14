using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Skills;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced
{
    /**
        <summary>
            Simple component wrapper around skills.
        </summary>
    */

    [Serializable]
    [DataContract]
    public class SkillComponent : Component
    {
        [DataMember]
        public ReadOnlyDictionary<Commandment, Skill> Skills { get; }

        [DataMember]
        private DxGameTime LatestGameTime { get; set; }

        public SkillComponent(DxGame game, params Skill[] skills)
            : this(game, skills.ToList())
        {
        }

        public SkillComponent(DxGame game, List<Skill> skills)
            : base(game)
        {
            Validate.IsNotNull(skills, StringUtils.GetFormattedNullOrDefaultMessage(this, skills));
            Validate.NoNullElements(skills, StringUtils.GetFormattedNullOrDefaultMessage(this, typeof (Skill)));
            Skills =
                new ReadOnlyDictionary<Commandment, Skill>(skills.ToDictionary(skill => skill.Ability, skill => skill));
            MessageHandler.RegisterMessageHandler<CommandMessage>(HandleSkillMessage);
        }

        private void HandleSkillMessage(CommandMessage abilityCommand)
        {
            var commandment = abilityCommand.Commandment;
            if(!Commandments.ABILITY_COMMANDMENTS.Contains(commandment))
            {
                return;
            }

            if (Skills.ContainsKey(commandment))
            {
                Skills[commandment].Activate(Parent, LatestGameTime);
            }
        }

        protected override void Update(DxGameTime gameTime)
        {
            LatestGameTime = gameTime;
        }
    }
}