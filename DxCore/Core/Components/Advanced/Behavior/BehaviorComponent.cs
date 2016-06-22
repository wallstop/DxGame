using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Components.Advanced.Behavior
{
    [Serializable]
    [DataContract]
    public class BehaviorComponent : Component
    {
        [DataMember]
        public Team Team { get; }

        protected BehaviorComponent(Team team)
        {
            Team = team;
        }

        public static BehaviorComponentBuilder Builder()
        {
            return new BehaviorComponentBuilder();
        }

        public class BehaviorComponentBuilder : IBuilder<BehaviorComponent>
        {
            private Team team_;

            public BehaviorComponent Build()
            {
                Validate.Hard.IsNotNull(team_, this.GetFormattedNullOrDefaultMessage(team_));

                return new BehaviorComponent(team_);
            }

            public BehaviorComponentBuilder WithTeam(Team team)
            {
                team_ = team;
                return this;
            }
        }
    }
}