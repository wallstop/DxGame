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

        public SkillComponent(params Skill[] skills)
            : this(skills.ToList())
        {
        }

        public SkillComponent(List<Skill> skills)
        {
            Validate.IsNotNull(skills, StringUtils.GetFormattedNullOrDefaultMessage(this, skills));
            Validate.NoNullElements(skills, StringUtils.GetFormattedNullOrDefaultMessage(this, typeof (Skill)));
            Skills =
                new ReadOnlyDictionary<Commandment, Skill>(skills.ToDictionary(skill => skill.Ability, skill => skill));
        }

        /**
            <summary>
                Use a one-frame-delayed approach in order to properly accumulate all CommandMessages that may have been relayed so we can properly trigger Activate/NoActivate
            </summary>
        */
        protected override void Update(DxGameTime gameTime)
        {
            HashSet<CommandMessage> commandMessages = Parent.CurrentMessages.OfType<CommandMessage>().ToHashSet();
            foreach(var skillEntry in Skills)
            {
                // No skill message? Signal that we didn't get one
                if(commandMessages.All(message => message.Commandment != skillEntry.Key))
                {
                    skillEntry.Value.NoActivation(Parent, gameTime);
                }
                else
                {
                    // Skill message? Signal that we did :^)
                    skillEntry.Value.Activate(Parent, gameTime);
                }
            }
        }
    }
}