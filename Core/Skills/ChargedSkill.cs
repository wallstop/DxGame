using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.Skills
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
                Maximum amount of time that this skill can charge (and do anything)
            </summary>
        */
        [DataMember]
        public TimeSpan ChargeTime { get; }

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

        public ChargedSkill(SkillFunction skillFunction, TimeSpan cooldown, TimeSpan chargeTime, Commandment ability)
            : base(skillFunction, cooldown, ability)
        {
            ChargeTime = chargeTime;
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

        public static new ChargedSkillBuilder Builder()
        {
            return new ChargedSkillBuilder();
        }

        public class ChargedSkillBuilder : SkillBuilder
        {
            private TimeSpan chargeTime_;

            public ChargedSkillBuilder WithChargeTime(TimeSpan chargeTime)
            {
                chargeTime_ = chargeTime;
                return this;
            }
            public override Skill Build()
            {
                ValidateArguments();
                Validate.IsNotNullOrDefault(chargeTime_,
                    StringUtils.GetFormattedNullOrDefaultMessage(this, "chargeTime"));
                return new ChargedSkill(skillFunction_, cooldown_, chargeTime_, ability_);
            }
        }
    }
}
