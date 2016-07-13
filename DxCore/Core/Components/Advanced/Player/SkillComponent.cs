using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Skills;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Components.Advanced.Player
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

        [DataMember] private readonly List<CommandMessage> commandMessages_;

        public SkillComponent(params Skill[] skills) : this(skills.ToList()) {}

        public SkillComponent(List<Skill> skills)
        {
            Validate.Hard.IsNotNull(skills, () => this.GetFormattedNullOrDefaultMessage(skills));
            Validate.Hard.NoNullElements(skills, () => this.GetFormattedNullOrDefaultMessage(typeof(Skill)));
            Skills =
                new ReadOnlyDictionary<Commandment, Skill>(skills.ToDictionary(skill => skill.Ability, skill => skill));
            commandMessages_ = new List<CommandMessage>();
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<CommandMessage>(HandleCommandMessage);
        }

        private void HandleCommandMessage(CommandMessage commandMessage)
        {
            if(!Equals(commandMessage.Target, Parent.Id))
            {
                return;
            }
            commandMessages_.Add(commandMessage);
        }

        /**
            <summary>
                Use a one-frame-delayed approach in order to properly accumulate all CommandMessages that may have been relayed so we can properly trigger Activate/NoActivate
            </summary>
        */

        protected override void Update(DxGameTime gameTime)
        {
            HashSet<CommandMessage> commandMessages = commandMessages_.ToHashSet();
            commandMessages_.Clear();

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