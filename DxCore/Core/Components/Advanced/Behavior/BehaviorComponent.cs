using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DXGame.Core;
using DXGame.Core.Utils;

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
                Validate.IsNotNull(team_, this.GetFormattedNullOrDefaultMessage(team_));

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