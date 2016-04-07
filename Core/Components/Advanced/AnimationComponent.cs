using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Animation;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.State;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class AnimationComponent : DrawableComponent
    {
        [DataMember] private readonly Dictionary<State.State, Animation.Animation> animationsForStates_;
        
        [DataMember]
        public PositionalComponent Position { get; }
        
        [DataMember]
        public StateMachine StateMachine { get; }

        [DataMember] private State.State lastState_ = null;

        private AnimationComponent(StateMachine stateMachine,
            Dictionary<State.State, Animation.Animation> animationsForStates, PositionalComponent position)
        {
            StateMachine = stateMachine;
            Position = position;
            animationsForStates_ = animationsForStates;
        }

        public static AnimationComponentBuilder Builder()
        {
            return new AnimationComponentBuilder();
        }

        public override void Initialize()
        {
            foreach(Animation.Animation animation in animationsForStates_.Values)
            {
                animation.LoadContent(DxGame.Instance.Content);
            }
        }

        public override void LoadContent()
        {
            foreach(Animation.Animation animation in animationsForStates_.Values)
            {
                animation.LoadContent(DxGame.Instance.Content);
            }
        }

        public override void Draw(SpriteBatch spritebatch, DxGameTime gameTime)
        {
            State.State currentState = StateMachine.CurrentState;
            if(lastState_ == null)
            {
                lastState_ = currentState;
            }
            else if(!ReferenceEquals(lastState_,currentState))
            {
                animationsForStates_[lastState_].Reset();
                lastState_ = currentState;
            }
            Animation.Animation animation = animationsForStates_[currentState];

            // TODO: Emit & Consume "Facing changed" messages
            FacingComponent facing = Parent.ComponentOfType<FacingComponent>();
            Direction orientation = Direction.East;
            if(!ReferenceEquals(facing, null))
            {
                orientation = facing.Facing;
            }
            animation.Draw(spritebatch, gameTime, Position.Position, orientation);
        }

        public class AnimationComponentBuilder : IBuilder<AnimationComponent>
        {
            private readonly Dictionary<State.State, Animation.Animation> animationsForStates_ =
                new Dictionary<State.State, Animation.Animation>();

            private PositionalComponent position_;
            private StateMachine stateMachine_;

            public AnimationComponent Build()
            {
                Validate.IsNotNull(position_, StringUtils.GetFormattedNullOrDefaultMessage(this, position_));
                Validate.IsNotNull(stateMachine_, StringUtils.GetFormattedNullOrDefaultMessage(this, stateMachine_));
                return new AnimationComponent(stateMachine_, animationsForStates_, position_);
            }

            public AnimationComponentBuilder WithPosition(PositionalComponent position)
            {
                Validate.IsNull(position_, $"Cannot double-assign a {typeof(PositionalComponent)} to a {GetType()}");
                position_ = position;
                return this;
            }

            public AnimationComponentBuilder WithStateMachine(StateMachine stateMachine)
            {
                stateMachine_ = stateMachine;
                return this;
            }

            public AnimationComponentBuilder WithStateAndAsset(State.State state, AnimationDescriptor descriptor)
            {
                Validate.IsNotNullOrDefault(state,
                    StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(descriptor)));
                Validate.IsFalse(animationsForStates_.ContainsKey(state),
                    StringUtils.GetFormattedAlreadyContainsMessage(this, state, animationsForStates_.Keys));
                var animation = new Animation.Animation(descriptor);
                animationsForStates_.Add(state, animation);
                return this;
            }
        }
    }
}