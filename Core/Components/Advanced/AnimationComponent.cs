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

        public AnimationComponent(StateComponent state = null, GameObject parent = null) 
            : base(parent)
        {
            state_ = state;
            lastState_ = "";
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

        public void AddAnimation(String state = "", String assetName = "", int totalFrames = 1)
        {
            var animation = new Animation(assetName, totalFrames).WithPosition(position_);
            stateMap_.Add(state, animation);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (lastState_ != state_.State)
            {
                stateMap_[lastState_].Reset();
            }
            Debug.Assert(stateMap_.ContainsKey(state_.State), "AnimationComponent has no corresponding state animation");
            stateMap_[state_.State].Draw(spriteBatch);
            lastState_ = state_.State;
        }

        public override bool LoadContent(ContentManager contentManager)
        {
            Debug.Assert(contentManager != null, "ContentManager cannot be null during LoadContent");
            foreach (var pair in stateMap_)
            {
                pair.Value.LoadContent(contentManager);
            }
            return true;
        }
    }
}
