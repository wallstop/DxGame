using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Animation;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.State;
using DXGame.Core.Utils;
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
            foreach (Animation.Animation animation in animationsForStates_.Values)
            {
                animation.LoadContent(DxGame.Instance.Content);
            }
        }

        public override void LoadContent()
        {
            foreach (Animation.Animation animation in animationsForStates_.Values)
            {
                animation.LoadContent(DxGame.Instance.Content);
            }
        }

        public override void Draw(SpriteBatch spritebatch, DxGameTime gameTime)
        {
            var currentState = StateMachine.CurrentState;
            var animation = animationsForStates_[currentState];
            animation.Draw(spritebatch, gameTime);
        }

        protected override void Update(DxGameTime gameTime)
        {
            var currentState = StateMachine.CurrentState;
            var animation = animationsForStates_[currentState];
            animation.Process(gameTime);
            base.Update(gameTime);
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
                Validate.IsNull(position_, $"Cannot double-assign a {typeof (PositionalComponent)} to a {GetType()}");
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
                Validate.IsNotNull(position_,
                    $"Creating {typeof (Animation.Animation)} requires that the {typeof (PositionalComponent)} be set first");
                Validate.IsNotNullOrDefault(state,
                    StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(descriptor)));
                Validate.IsFalse(animationsForStates_.ContainsKey(state),
                    StringUtils.GetFormattedAlreadyContainsMessage(this, state, animationsForStates_.Keys));
                var animation = new Animation.Animation(descriptor).WithPosition(position_);
                animationsForStates_.Add(state, animation);
                return this;
            }
        }
    }
}