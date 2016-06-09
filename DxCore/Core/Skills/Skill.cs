using System;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DxCore.Core.Skills
{
    public delegate void SkillFunction(GameObject parent, DxGameTime startTime, DxGameTime currentTime);

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
        [DataMember] protected readonly SkillFunction skillFunction_;
        [DataMember] protected TimeSpan lastActivated_;

        [DataMember]
        public TimeSpan Cooldown { get; private set; }

        [DataMember]
        public Commandment Ability { get; }

        protected Skill(SkillFunction skillFunction, TimeSpan cooldown, Commandment ability)
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

        public virtual void Activate(GameObject parent, DxGameTime gameTime)
        {
            var totalGameTime = gameTime.TotalGameTime;
            if(lastActivated_ + Cooldown < totalGameTime)
            {
                lastActivated_ = totalGameTime;
                skillFunction_(parent, gameTime, gameTime);
            }
        }

        /**
            <summary>
                We might care about the case where we aren't activated - for example, if the longer we aren't activated, the more stuff we do. 
                Or, conversely, a charged-type ability (that needs to know when the player stops pressing a key)
            </summary>
        */

        public virtual void NoActivation(GameObject parent, DxGameTime gameTime)
        {
            // Normal skill - do nothing
        }

        public static SkillBuilder Builder()
        {
            return new SkillBuilder();
        }

        public class SkillBuilder : IBuilder<Skill>
        {
            protected TimeSpan cooldown_;
            protected SkillFunction skillFunction_;
            protected Commandment ability_;

            public virtual Skill Build()
            {
                ValidateArguments();
                return new Skill(skillFunction_, cooldown_, ability_);
            }

            protected void ValidateArguments()
            {
                Validate.IsNotNullOrDefault(cooldown_, this.GetFormattedNullOrDefaultMessage("Cooldown"));
                Validate.IsNotNull(skillFunction_, this.GetFormattedNullOrDefaultMessage(skillFunction_));
                Validate.IsTrue(Commandments.ABILITY_COMMANDMENTS.Contains(ability_),
                    $"{ability_} was expected to be an ability-type {typeof(Commandment)}, but was not.");
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