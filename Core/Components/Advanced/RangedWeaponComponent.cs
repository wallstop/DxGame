using System;
using System.Collections.Generic;
using System.Diagnostics;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    public class RangedWeaponComponent : WeaponComponent
    {
        private const float velocity_ = 2.0f;

        public Vector2 Direction { get; set; }

        public TimeSpan Cooldown { get; private set; }

        private TimeSpan LastAttacked { get; set; }

        private PhysicsComponent Owner { get; set; }

        private Dictionary<GameObject, Vector2> projectiles_ = new Dictionary<GameObject, Vector2>();

        public RangedWeaponComponent(DxGame game)
            : base(game)
        {
            Cooldown = new TimeSpan(0);
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

        public override void Update(GameTime gameTime)
        {
            var velocity = Owner.Velocity;
            // If we're moving in the +/- X direction, update our Direction
            if (0 != MathUtils.FuzzyCompare(velocity.X, 0.0))
            {
                Direction = velocity;
            }

            // Iterate over all shot projectiles, find the ones that have "stopped"
            List<GameObject> projectilesToRemove = new List<GameObject>();
            Dictionary<GameObject, Vector2> updatedProjectiles = new Dictionary<GameObject, Vector2>();
            foreach (var projectile in projectiles_)
            {
                GameObject gameObject = projectile.Key;
                Vector2 projectilePosition = projectile.Value;

                PhysicsComponent physics = gameObject.ComponentOfType<PhysicsComponent>();
                var updatedPosition = physics.Position;
                // Check to see if we've moved
                if (updatedPosition == projectilePosition)
                {
                    projectilesToRemove.Add(gameObject);
                }
                else
                {
                    updatedProjectiles.Add(gameObject, updatedPosition);
                }
            }

            projectiles_ = updatedProjectiles;

            // Remove each one from our map, and the gamestate
            foreach (var gameObject in projectilesToRemove)
            {
                DxGame.RemoveGameObject(gameObject);
            }

            base.Update(gameTime);
        }

        public override void Attack(GameTime gameTime)
        {
            // Check if we can fire the weapon again (
            var currentTime = gameTime.ElapsedGameTime;
            if (LastAttacked.Add(Cooldown) <= currentTime)
            {
                LastAttacked = currentTime;
                DoAttack();
            }
        }

        private void DoAttack()
        {
            GameObject projectile = new GameObject();
            var mapModel = DxGame.Model<MapModel>();
            var bounds = mapModel.MapBounds;
            // TODO: Unhard code the dimensions
            SpatialComponent space =
                (SpatialComponent) new BoundedSpatialComponent(DxGame).WithBounds(bounds).WithDimensions(new Vector2(30, 30)).WithPosition(Owner.Position);
            var shootLeft = Direction.X < 0;
            Vector2 velocity = new Vector2(shootLeft ? -velocity_ : velocity_, 0);
            PhysicsComponent physics =
                new MapCollideablePhysicsComponent(DxGame).WithVelocity(velocity).WithPositionalComponent(space);
            SimpleSpriteComponent sprite = new SimpleSpriteComponent(DxGame).WithPosition(space).WithAsset("Orb");
            projectile.WithComponents(space, physics, sprite);

            DxGame.AddAndInitializeGameObject(projectile);
            // Set initial position to be the "previous" position so we don't immediately remove ourselves from the game
            projectiles_.Add(projectile, space.Position - velocity);
        }
    }
}