using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DXGame.Core.Components.Basic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Advanced
{
    class AnimationComponent : DrawableComponent
    {
        private readonly Dictionary<String, Animation> stateMap_ = new Dictionary<string, Animation>();
        private String lastState_;
        protected StateComponent state_;
        protected PositionalComponent position_;



        public AnimationComponent(Game game)
            : base(game)
        {
            (SpriteBatch)game.Services.GetService(typeof(SpriteBatch))
        }

        public AnimationComponent WithPosition(PositionalComponent position)
        {
            Debug.Assert(position != null, "Sprite position cannot be null on assignment");
            position_ = position;
            return this;
        }

        public AnimationComponent WithState(StateComponent state)
        {
            Debug.Assert(state != null, "Sprite position cannot be null on assignment");
            state_ = state;
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
