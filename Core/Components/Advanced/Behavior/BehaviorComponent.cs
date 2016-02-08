using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Behavior
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    public class BehaviorComponent : Component
    {
        [DataMember]
        [ProtoMember(1)]
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
                Validate.IsNotNull(team_, StringUtils.GetFormattedNullOrDefaultMessage(this, team_));

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