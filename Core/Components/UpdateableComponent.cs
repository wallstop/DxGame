using System;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components
{
    public abstract class UpdateableComponent : Component, IComparable<UpdateableComponent>
    {
        public enum UpdatePriority
        {
            NONE = 100,
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
            if (priority_ == rhs.Priority)
            {
                return 0;
            }

            return priority_ > rhs.Priority ? 1 : -1;
        }

        public abstract bool Update(GameTime gameTime);
    }
}