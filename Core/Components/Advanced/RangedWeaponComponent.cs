using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class RangedWeaponComponent : WeaponComponent
    {
        private const float velocity_ = 7.0f;

        public Vector2 Direction { get; set; }

        public TimeSpan Cooldown { get; private set; }

        private TimeSpan LastAttacked { get; set; }

        private PhysicsComponent Owner { get; set; }

        public RangedWeaponComponent(DxGame game)
            : base(game)
        {
            Cooldown = TimeSpan.FromSeconds(0);
        }

        public RangedWeaponComponent WithDirection(Vector2 direction)
        {
            Direction = direction;
            return this;
        }

        public RangedWeaponComponent WithPhysicsComponent(PhysicsComponent physics)
        {
            Debug.Assert(!GenericUtils.IsNullOrDefault(physics),
                "Ranged Weapons cannot be initialized with a null physics owner");
            Owner = physics;
            return this;
        }

        public override void Update(DxGameTime gameTime)
        {
            var velocity = Owner.Velocity;
            // If we're moving in the +/- X direction, update our Direction
            if (0 != MathUtils.FuzzyCompare(velocity.X, 0.0))
            {
                Direction = velocity;
            }

            base.Update(gameTime);
        }

        public override void Attack(DxGameTime gameTime)
        {
            // Check if we can fire the weapon again (
            var currentTime = gameTime.TotalGameTime;
            if (LastAttacked.Add(Cooldown) > currentTime)
            {
                return;
            }
            LastAttacked = currentTime;
            DoAttack();
        }

        private void DoAttack()
        {
            GameObject projectile = new GameObject();
            var mapModel = DxGame.Model<MapModel>();
            var bounds = mapModel.MapBounds;
            // TODO: Unhard code the dimensions
            SpatialComponent space =
                (SpatialComponent)
                    new BoundedSpatialComponent(DxGame).WithBounds(bounds)
                        .WithDimensions(new Vector2(30, 30))
                        .WithPosition(Owner.Position);
            var shootLeft = Direction.X < 0;
            Vector2 velocity = new Vector2(shootLeft ? -velocity_ : velocity_, 0);
            PhysicsComponent physics =
                new MapCollideablePhysicsComponent(DxGame).WithVelocity(velocity).WithPositionalComponent(space);
            SimpleSpriteComponent sprite = new SimpleSpriteComponent(DxGame).WithPosition(space).WithAsset("Orb");
            CollisionDestructibleComponent destructible = new CollisionDestructibleComponent(DxGame);
            projectile.WithComponents(space, physics, sprite, destructible);

            DxGame.AddAndInitializeGameObject(projectile);
        }
    }
}