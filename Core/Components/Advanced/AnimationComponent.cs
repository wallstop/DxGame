using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Behavior;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class AnimationComponent : DrawableComponent
    {
        [DataMember] private readonly Dictionary<State, Animation> states_ = new Dictionary<State, Animation>();
        [DataMember] protected Behavior.Behavior behavior_;
        [DataMember] protected PositionalComponent position_;

        public AnimationComponent(DxGame game)
            : base(game)
        {
        }

        public AnimationComponent WithPosition(PositionalComponent position)
        {
            Validate.IsNotNullOrDefault(position, StringUtils.GetFormattedNullDefaultMessage(this, position));
            position_ = position;
            return this;
        }

        public AnimationComponent WithBehavior(Behavior.Behavior behavior)
        {
            Validate.IsNotNullOrDefault(behavior, StringUtils.GetFormattedNullDefaultMessage(this, behavior));
            behavior_ = behavior;
            return this;
        }

        public void AddAnimation(string state, string assetName)
        {
            var animation = new Animation(assetName).WithPosition(position_);
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

        public override void Draw(DxGameTime gameTime)
        {
            var currentState = state_.State;
            if (lastState_ != currentState && states_.ContainsKey(currentState))
            {
                states_[lastState_].Reset();
                lastState_ = currentState;
            }

            states_[lastState_].Draw(spriteBatch_);

            // TODO: Do timing based on gameTime
            if (lastState_ != state_.State)
            {
                states_[lastState_].Reset();
            }
            states_[state_.State].Draw(spriteBatch_);
            lastState_ = state_.State;
        }
    }
}