using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Wrappers;
using DXGame.Main;
using log4net;

namespace DXGame.Core.Components.Advanced.Position
{
    /**
    <summary>
        PositionalComponent is a component that keeps track of a position in space and time. You can think of it as a 2D point.

        PositionalComponent is meant to be a base class for more complex classes. For example, SpatialComponent (which is a component
        that takes up a Rectangular 2D space) and BoundedSpatialComponent (which is the same as SpatialComponent, but has it's position
        bounded by some larger Rectangle) implement more complex logic inside of their set Position functions.

        Any DrawableComponent should have some reference to a PositionalComponent (or subclass) as a reference of where to call their
        Draw methods at.

        PositionalComponent is meant to be shared between Components. For example, a Component that handles input can update a 
        PositionalComponent's Position, which will then be reflected in the DrawableComponent by having the DrawableComponent be
        drawn at the new Position.

        Classes derived from PositionalComponent should take care to implement their own Position property if they want any special behavior.
    </summary>
    */

    [Serializable]
    [DataContract]
    public class PositionalComponent : Component
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (PositionalComponent));
        [DataMember] protected DxVector2 position_;
        /**
        <summary>
            The Position property of a PositionalComponent is likely to be overriden by derived classes.
            As such, it's virtual, and wraps an internal position_ member, instead of being a simple
            get/set Property.

            Used to modify the PositionalComponent's Position in 2D space.

            For most PositionalComponents, you should be able to get/set Position by doing things like:
            <code>
                // Calls get, updates the value, then calls set
                Positional += new Vector2(2.0, -2.0); 
            </code>
        </summary>
        */

        [IgnoreDataMember]
        public virtual DxVector2 Position
        {
            get { return position_; }
            set { position_ = value; }
        }

        public PositionalComponent(DxGame game)
            : base(game)
        {
        }

        public virtual PositionalComponent WithPosition(float x, float y)
        {
            position_.X = x;
            position_.Y = y;
            return this;
        }

        public virtual PositionalComponent WithPosition(DxVector2 position)
        {
            Debug.Assert(position != null, "PositionalComponent cannot be initialized with a null position");
            position_ = position;
            return this;
        }
    }
}