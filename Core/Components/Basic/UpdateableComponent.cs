using System;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Basic
{
    public abstract class UpdateableComponent : Component, IComparable<UpdateableComponent>
    {
        public enum UpdatePriority
        {
            NONE = 100,
            IMPERATIVE = -10,
            HIGH = 0,
            NORMAL = 50,
            LOW = 80
        }

        protected UpdatePriority priority_;

        protected UpdateableComponent(GameObject parent = null)
            : base(parent)
        {
        }

        public UpdatePriority Priority
        {
            get { return priority_; }
            set { priority_ = value; }
        }

        public int CompareTo(UpdateableComponent rhs)
        {
            return MathUtils.Compare(priority_, rhs.Priority);
        }

        public abstract bool Update(GameTime gameTime);
    }
}