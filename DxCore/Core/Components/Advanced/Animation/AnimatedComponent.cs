using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Animation;
using DxCore.Core.Components.Advanced.Animated;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.State;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace DxCore.Core.Components.Advanced.Animation
{
    [Serializable]
    [DataContract]
    public class AnimatedComponent : DrawableComponent
    {
        [DataMember] private readonly Dictionary<State.State, Core.Animation.Animation> animationsForStates_;

        [DataMember] private State.State lastState_;

        [DataMember]
        public PhysicsComponent Position { get; }

        [DataMember]
        public StateMachine StateMachine { get; }

        private AnimatedComponent(StateMachine stateMachine,
            Dictionary<State.State, Core.Animation.Animation> animationsForStates, PhysicsComponent position)
        {
            StateMachine = stateMachine;
            Position = position;
            animationsForStates_ = animationsForStates;
        }

        public static AnimationComponentBuilder Builder()
        {
            return new AnimationComponentBuilder();
        }

        public override void Draw(SpriteBatch spritebatch, DxGameTime gameTime)
        {
            State.State currentState = StateMachine.CurrentState;
            if(lastState_ == null)
            {
                lastState_ = currentState;
            }
            else if(!ReferenceEquals(lastState_, currentState))
            {
                animationsForStates_[lastState_].Reset();
                lastState_ = currentState;
            }
            Core.Animation.Animation animation = animationsForStates_[currentState];

            // TODO: Emit & Consume "Facing changed" messages
            FacingComponent facing = Parent.ComponentOfType<FacingComponent>();
            Direction orientation = Direction.East;
            if(!ReferenceEquals(facing, null))
            {
                orientation = facing.Facing;
            }
            animation.Draw(spritebatch, gameTime, Position.Position, orientation);
        }

        public override void Initialize()
        {
            foreach(Core.Animation.Animation animation in animationsForStates_.Values)
            {
                animation.LoadContent(DxGame.Instance.Content);
            }
        }

        public override void LoadContent()
        {
            foreach(Core.Animation.Animation animation in animationsForStates_.Values)
            {
                animation.LoadContent(DxGame.Instance.Content);
            }
        }

        public class AnimationComponentBuilder : IBuilder<AnimatedComponent>
        {
            private readonly Dictionary<State.State, Core.Animation.Animation> animationsForStates_ =
                new Dictionary<State.State, Core.Animation.Animation>();

            private PhysicsComponent position_;
            private StateMachine stateMachine_;

            public AnimatedComponent Build()
            {
                Validate.Hard.IsNotNull(position_, this.GetFormattedNullOrDefaultMessage(position_));
                Validate.Hard.IsNotNull(stateMachine_, this.GetFormattedNullOrDefaultMessage(stateMachine_));
                return new AnimatedComponent(stateMachine_, animationsForStates_, position_);
            }

            public AnimationComponentBuilder WithPosition(PhysicsComponent position)
            {
                position_ = position;
                return this;
            }

            public AnimationComponentBuilder WithStateAndAsset(State.State state, AnimationDescriptor descriptor)
            {
                Validate.Hard.IsNotNullOrDefault(state, () => this.GetFormattedNullOrDefaultMessage(nameof(descriptor)));
                Validate.Hard.IsFalse(animationsForStates_.ContainsKey(state),
                    () => this.GetFormattedAlreadyContainsMessage(state, animationsForStates_.Keys));
                Core.Animation.Animation animation = new Core.Animation.Animation(descriptor);
                animationsForStates_.Add(state, animation);
                return this;
            }

            public AnimationComponentBuilder WithStateMachine(StateMachine stateMachine)
            {
                stateMachine_ = stateMachine;
                return this;
            }
        }
    }
}