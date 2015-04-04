using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
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
        [DataMember]
        private readonly Dictionary<string, Animation> states_ = new Dictionary<string, Animation>();
        [DataMember]
        private string lastState_;
        [DataMember]
        protected StateComponent state_;
        [DataMember]
        protected PositionalComponent position_;

        public AnimationComponent(DxGame game)
            : base(game)
        {
        }

        public AnimationComponent WithPosition(PositionalComponent position)
        {
            Debug.Assert(!GenericUtils.IsNullOrDefault(position), "Sprite position cannot be null on assignment");
            position_ = position;
            return this;
        }

        public AnimationComponent WithState(StateComponent state)
        {
            Debug.Assert(!GenericUtils.IsNullOrDefault(state) || !state.States.Any(),
                "Sprite position cannot be null on assignment");
            state_ = state;
            lastState_ = state.States.First();
            return this;
        }

        public void AddAnimation(String state, String assetName)
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