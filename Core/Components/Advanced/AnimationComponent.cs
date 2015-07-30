using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.State;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class AnimationComponent : DrawableComponent
    {
        [DataMember] private readonly Dictionary<State.State, Animation> states_ =
            new Dictionary<State.State, Animation>();

        [DataMember]
        public PositionalComponent Position { get; protected set; }

        [DataMember]
        public StateMachine StateMachine { get; protected set; }

        private AnimationComponent(DxGame game, StateMachine stateMachine, PositionalComponent position) : base(game)
        {
            Validate.IsNotNullOrDefault(stateMachine, StringUtils.GetFormattedNullOrDefaultMessage(this, stateMachine));
            Validate.IsNotNullOrDefault(position, StringUtils.GetFormattedNullOrDefaultMessage(this, position));
            StateMachine = stateMachine;
            Position = position;
        }

        public static AnimationComponentBuilder Builder()
        {
            return new AnimationComponentBuilder();
        }

        public void AddAnimation(State.State state, string assetName)
        {
            Validate.IsNotNullOrDefault(state, $"Cannot add a null {nameof(state)} to a {nameof(AnimationComponent)}");
            Validate.IsFalse(states_.ContainsKey(state),
                StringUtils.GetFormattedAlreadyContainsMessage(this, state, states_.Keys));
            var animation = new Animation(assetName).WithPosition(Position);
            states_.Add(state, animation);
        }

        public override void Initialize()
        {
            foreach (var pair in states_)
            {
                pair.Value.LoadContent(DxGame.Content);
            }
        }

        public override void LoadContent()
        {
            foreach (var pair in states_)
            {
                pair.Value.LoadContent(DxGame.Content);
            }
        }

        public override void Draw(SpriteBatch spritebatch, DxGameTime gameTime)
        {
            var currentState = StateMachine.CurrentState;
            var animation = states_[currentState];
            animation.Draw(spritebatch, gameTime);
        }

        public class AnimationComponentBuilder : IBuilder<AnimationComponent>
        {
            private DxGame game_;
            private PositionalComponent position_;
            private StateMachine stateMachine_;

            public AnimationComponent Build()
            {
                return new AnimationComponent(game_, stateMachine_, position_);
            }

            public AnimationComponentBuilder WithDxGame(DxGame game)
            {
                game_ = game;
                return this;
            }

            public AnimationComponentBuilder WithPosition(PositionalComponent position)
            {
                position_ = position;
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