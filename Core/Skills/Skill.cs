using System;
using System.Runtime.Serialization;
using DXGame.Core.GraphicsWidgets.HUD;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.Skills
{
    public delegate void SkillFunction(GameObject parent, DxGameTime gameTime);

    /**
        <summary> 
            Skills can be thought of as a sub-module of the SkillComponent. 
            Skills can be activated (to some effect). 
            They operate on a cooldown system, so you can only activate skills every so often 
        </summary>
    */

    [Serializable]
    [DataContract]
    public class Skill
    {
        [DataMember]
        private readonly SkillFunction skillFunction_;
        [DataMember]
        private TimeSpan lastActivated_;
        [DataMember]
        public TimeSpan Cooldown { get; private set; }

        [DataMember]
        public Commandment Ability { get; }

        private Skill(SkillFunction skillFunction, TimeSpan cooldown, Commandment ability)
        {
            skillFunction_ = skillFunction;
            Cooldown = cooldown;
            Ability = ability;
        }

        /**
            <summary> How much time is remaining until the cooldown has expired. </summary>
        */

        public TimeSpan RemainingCooldown(DxGameTime gameTime)
        {
            return MathUtils.Max(Cooldown - (gameTime.TotalGameTime - lastActivated_), TimeSpan.Zero);
        }

        /**
            <summary> Attempts to activate the skill. This will silently do nothing if the skill is on cooldown. </summary>
        */
        public void Activate(GameObject parent, DxGameTime gameTime)
        {
            var totalGameTime = gameTime.TotalGameTime;
            if (lastActivated_ + Cooldown < totalGameTime)
            {
                lastActivated_ = totalGameTime;
                skillFunction_(parent, gameTime);
            }
        }

        public static SkillBuilder Builder()
        {
            return new SkillBuilder();
        }

        public class SkillBuilder : IBuilder<Skill>
        {
            private TimeSpan cooldown_;
            private SkillFunction skillFunction_;
            private Commandment ability_;

            public Skill Build()
            {
                Validate.IsNotNullOrDefault(cooldown_, StringUtils.GetFormattedNullOrDefaultMessage(this, "Cooldown"));
                Validate.IsNotNull(skillFunction_, StringUtils.GetFormattedNullOrDefaultMessage(this, skillFunction_));
                Validate.IsTrue(Commandments.ABILITY_COMMANDMENTS.Contains(ability_), $"{ability_ } was expected to be an ability-type {typeof(Commandment)}, but was not.");
                return new Skill(skillFunction_, cooldown_, ability_);
            }

            public SkillBuilder WithCommandment(Commandment ability)
            {
                ability_ = ability;
                return this;
            }

            public SkillBuilder WithSkillFunction(SkillFunction skillFunction)
            {
                skillFunction_ = skillFunction;
                return this;
            }

            public SkillBuilder WithCooldown(TimeSpan cooldown)
            {
                cooldown_ = cooldown;
                return this;
            }
        }
    }
}