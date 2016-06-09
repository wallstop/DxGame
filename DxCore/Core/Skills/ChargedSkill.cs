using System;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;

namespace DxCore.Core.Skills
{
    /**
        <summary>
            A skill that needs charging. Lazers go here.
        </summary>
    */

    [Serializable]
    [DataContract]
    public class ChargedSkill : Skill
    {
        /**
            <summary>
                Whether or not the skill is "currently being charged" 
            </summary>
        */

        [DataMember]
        private bool Charging { get; set; }

        /**
            <summary>
                If charging, this will mark the Time that the ability had begun to charge
            </summary>
        */

        [DataMember]
        private DxGameTime InitialChargeTime { get; set; }

        public ChargedSkill(SkillFunction skillFunction, TimeSpan cooldown, Commandment ability)
            : base(skillFunction, cooldown, ability)
        {
            Charging = false;
        }

        public override void Activate(GameObject parent, DxGameTime gameTime)
        {
            TimeSpan totalGameTime = gameTime.TotalGameTime;
            if(lastActivated_ + Cooldown < totalGameTime && !Charging)
            {
                Charging = true;
                InitialChargeTime = gameTime;
            }
        }

        public override void NoActivation(GameObject parent, DxGameTime gameTime)
        {
            if(!Charging)
            {
                return;
            }
            Charging = false;
            lastActivated_ = gameTime.TotalGameTime;
            skillFunction_(parent, InitialChargeTime, gameTime);
        }

        public new static ChargedSkillBuilder Builder()
        {
            return new ChargedSkillBuilder();
        }

        public class ChargedSkillBuilder : SkillBuilder
        {
            public override Skill Build()
            {
                ValidateArguments();
                return new ChargedSkill(skillFunction_, cooldown_, ability_);
            }
        }
    }
}
