using System;
using System.Runtime.Serialization;
using FarseerPhysics.Dynamics;

namespace DxCore.Core.Physics
{
    [Serializable]
    [DataContract]
    public sealed class CollisionGroup
    {
        public static CollisionGroup All = new CollisionGroup(Category.All);
        public static CollisionGroup None = new CollisionGroup(Category.None);
        public static CollisionGroup Map = new CollisionGroup(Category.Cat1);

        // TODO: Expand for teams, etc, more general use

        [DataMember]
        public Category CollisionCategory { get; private set; }

        private CollisionGroup(Category collisionCategory)
        {
            CollisionCategory = collisionCategory;
        }

        public CollisionGroup And(CollisionGroup collisionGroup)
        {
            return new CollisionGroup(CollisionCategory | collisionGroup.CollisionCategory);
        }

        public CollisionGroup Not(CollisionGroup collisionGroup)
        {
            return new CollisionGroup(CollisionCategory & ~collisionGroup.CollisionCategory);
        }
    }
}
