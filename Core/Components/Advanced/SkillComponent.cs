using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Skills;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced
{

    /**
        <summary>
            Provides an answer to the question "Should this skill be activated?"
        </summary>
    */
    public delegate bool SkillActivater(DxGame game, DxGameTime gameTime, TimeSpan remainingCooldown);

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
        public Skill Skill { get; private set; }
        [DataMember]
        private readonly SkillActivater skillActivator_;

        public SkillComponent(DxGame game, Skill skill, SkillActivater skillActivator) 
            : base(game)
        {
            Validate.IsNotNull(skill, StringUtils.GetFormattedNullOrDefaultMessage(this, skill));
            Validate.IsNotNull(skillActivator, StringUtils.GetFormattedNullOrDefaultMessage(this, skillActivator));
            Skill = skill;
            skillActivator_ = skillActivator;
        }

        protected override void Update(DxGameTime gameTime)
        {
            TimeSpan remainingCooldown = Skill.RemainingCooldown(gameTime);
            bool shouldActivate = skillActivator_(DxGame, gameTime, remainingCooldown);
            if (shouldActivate)
            {
                Skill.Activate(Parent, gameTime);
            }
            base.Update(gameTime);
        }
    }
}