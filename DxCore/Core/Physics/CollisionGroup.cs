using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using DxCore.Core.Utils.Validate;
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
        public static CollisionGroup Entities = new CollisionGroup(Category.Cat2);

        public static class Alias<T>
        {
            private static ConcurrentDictionary<T, CollisionGroup> Aliases { get; } =
                new ConcurrentDictionary<T, CollisionGroup>();

            public static CollisionGroup Register(T key, Category value)
            {
                Validate.Hard.IsNotNull(key,
                    () => $"Cannot alias a null key to a {typeof(CollisionGroup)} of type {typeof(T)}");
                CollisionGroup foundGroup = Aliases.GetOrAdd(key, providedKey => new CollisionGroup(value));
                Validate.Hard.AreEqual(foundGroup.CollisionCategory, value,
                    () => $"Cannot re-alias {key} from {foundGroup.CollisionCategory} to {value}");
                return foundGroup;
            }

            public static CollisionGroup GroupFor(T key)
            {
                /* Throw if not found */
                return Aliases[key];
            }
        }

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
