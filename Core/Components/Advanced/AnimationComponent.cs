using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    internal class AnimationComponent : DrawableComponent
    {
        private readonly Dictionary<String, Animation> stateMap_ = new Dictionary<string, Animation>();
        private String lastState_;
        protected StateComponent state_;
        protected PositionalComponent position_;

        public AnimationComponent(DxGame game)
            : base(game)
        {
        }

        public AnimationComponent WithPosition(PositionalComponent position)
        {
            Debug.Assert(position != null, "Sprite position cannot be null on assignment");
            position_ = position;
            return this;
        }

        public AnimationComponent WithState(StateComponent state)
        {
            Debug.Assert(!GenericUtils.IsNullOrDefault(state) || !state.States.Any(), "Sprite position cannot be null on assignment");
            state_ = state;
            lastState_ = state.States.First();
            return this;
        }

        public void AddAnimation(String state, String assetName)
        {
            var animation = new Animation(assetName).WithPosition(position_);
            stateMap_.Add(state, animation);
        }

        protected override void LoadContent()
        {
            foreach (var pair in stateMap_)
            {
                pair.Value.LoadContent(Game.Content);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (lastState_ != state_.State)
            {
                stateMap_[lastState_].Reset();
            }
            stateMap_[state_.State].Draw(spriteBatch_);
            lastState_ = state_.State;
        }
    }
}