using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
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
        public StandardMovementComponent Movement { get; }

        protected BehaviorComponent(DxGame game, StandardActionComponent actionComponent, StandardMovementComponent movementComponent, Team team) 
            : base(game)
        {
            Team = team;
            Action = actionComponent;
            Movement = movementComponent;
        }

        public static BehaviorComponentBuilder Builder()
        {
            return new BehaviorComponentBuilder();
        }

        public class BehaviorComponentBuilder : IBuilder<BehaviorComponent>
        {
            private Team team_;
            private StandardActionComponent actionComponent_;
            private StandardMovementComponent movementComponent_;

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

            public BehaviorComponentBuilder WithStandardMovementComponent(StandardMovementComponent movementComponent)
            {
                movementComponent_ = movementComponent;
                return this;
            }

            public BehaviorComponent Build()
            {
                Validate.IsNotNull(team_, StringUtils.GetFormattedNullOrDefaultMessage(this, team_));
                Validate.IsNotNull(actionComponent_, StringUtils.GetFormattedNullOrDefaultMessage(this, actionComponent_));
                Validate.IsNotNull(movementComponent_, StringUtils.GetFormattedNullOrDefaultMessage(this, movementComponent_));

                var game = DxGame.Instance;
                return new BehaviorComponent(game, actionComponent_, movementComponent_, team_);
            }
        }
    }
}
