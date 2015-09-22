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

        private AnimationComponent(DxGame game, StateMachine stateMachine,
            Dictionary<State.State, Animation.Animation> animationsForStates, PositionalComponent position)
            : base(game)
        {
            Validate.IsNotNullOrDefault(stateMachine, StringUtils.GetFormattedNullOrDefaultMessage(this, stateMachine));
            Validate.IsNotNullOrDefault(position, StringUtils.GetFormattedNullOrDefaultMessage(this, position));
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
            foreach (var pair in animationsForStates_)
            {
                pair.Value.LoadContent(DxGame.Content);
            }
        }

        public override void LoadContent()
        {
            foreach (var pair in animationsForStates_)
            {
                pair.Value.LoadContent(DxGame.Content);
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

            private DxGame game_;
            private PositionalComponent position_;
            private StateMachine stateMachine_;

            public AnimationComponent Build()
            {
                return new AnimationComponent(game_, stateMachine_, animationsForStates_, position_);
            }

            public AnimationComponentBuilder WithDxGame(DxGame game)
            {
                game_ = game;
                return this;
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