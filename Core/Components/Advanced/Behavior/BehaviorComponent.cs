using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Impulse;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Behavior
{
    [Serializable]
    [DataContract]
    public class BehaviorComponent : Component
    {
        public Team Team { get; }
        public StandardActionComponent Action { get; }

        protected BehaviorComponent(StandardActionComponent actionComponent, Team team)
        {
            Team = team;
            Action = actionComponent;
        }

        public static BehaviorComponentBuilder Builder()
        {
            return new BehaviorComponentBuilder();
        }

        public class BehaviorComponentBuilder : IBuilder<BehaviorComponent>
        {
            private StandardActionComponent actionComponent_;
            private Team team_;

            public BehaviorComponent Build()
            {
                Validate.IsNotNull(team_, StringUtils.GetFormattedNullOrDefaultMessage(this, team_));
                Validate.IsNotNull(actionComponent_,
                    StringUtils.GetFormattedNullOrDefaultMessage(this, actionComponent_));
                
                return new BehaviorComponent(actionComponent_, team_);
            }

            public BehaviorComponentBuilder WithTeam(Team team)
            {
                team_ = team;
                return this;
            }

            public BehaviorComponentBuilder WithStandardActionComponent(StandardActionComponent actionComponent)
            {
                actionComponent_ = actionComponent;
                return this;
            }
        }
    }
}