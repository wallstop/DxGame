using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;

namespace DxCore.Core.Components.Advanced.Damage
{
    /**
        <summary>
            What happens when we die? The world hungers for answers.
            This is a simple function that, given a space on which an entity died, ...does something. 
            Probably something cool, but maybe not (you can never tell)
        </summary>
    */

    public delegate void DeathEffect(DxRectangle space);

    /**
        <summary>
            Handles removing the owning Object & components from the gamestate on death, as well as applying a death effect
        </summary>
    */

    [Serializable]
    [DataContract]
    public class DeathEffectComponent : Component
    {
        [DataMember] protected DeathEffect deathEffect_;

        public DeathEffectComponent(DeathEffect deathEffect)
        {
            Validate.IsNotNull(deathEffect, this.GetFormattedNullOrDefaultMessage(nameof(DeathEffect)));
            deathEffect_ = deathEffect;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<EntityDeathMessage>(HandleEntityDeath);
        }

        protected virtual void HandleEntityDeath(EntityDeathMessage deathMessage)
        {
            if(!ReferenceEquals(deathMessage.Entity, Parent))
            {
                /* Not us? Don't care */
                return;
            }

            SpatialComponent spatial = deathMessage.Entity?.ComponentOfType<SpatialComponent>();
            /* 
                If there is a spatial, trigger the death effect. 
                If there isn't one, we can't reliably determine the space that the entity died at, so don't trigger it. 
            */
            if(!ReferenceEquals(spatial, null))
            {
                deathEffect_.Invoke(spatial.Space);
            }
            Parent?.Remove();
        }

        public static void SimpleEnemyBloodParticles(DxRectangle deathSpace)
        {
            /* Make sure we didn't accidentally get a bogus space */
            if(Check.IsNullOrDefault(deathSpace))
            {
                return;
            }

            /* TODO: These are all SWAGs for good values, tweak as necessary */
            const float minGrowRate = -0.9f;
            const float maxGrowRate = 0.3f;
            const float minTimeToLive = 1.0f;
            const float maxTimeToLive = 2.5f;
            const float minAcceleration = -0.5f;
            const float maxAcceleration = 0.7f;
            const float minForce = 0.3f;
            const float maxForce = 1.0f;
            const int minNumParticles = 10;
            const int maxNumParticles = 20;
            const float minRadius = 3;
            const float maxRadius = 8;
            const float minDistance = 40.0f;
            const float maxDistance = 110.0f;
            DxRadian maxSpread = new DxDegree(90).DxRadian;

            Color deathColor = Color.DarkRed;

            /* 
                Randomly generate a bunch of particles that fly in any direction away from the origin 
                space at varying speeds, accelerations, TTLs, all that good stuff 
            */
            var numParticles = ThreadLocalRandom.Current.Next(minNumParticles, maxNumParticles);
            for(int i = 0; i < numParticles; ++i)
            {
                var position =
                    new DxVector2(ThreadLocalRandom.Current.NextFloat(deathSpace.X, deathSpace.X + deathSpace.Width),
                        ThreadLocalRandom.Current.NextFloat(deathSpace.Y, deathSpace.Y + deathSpace.Height));
                var particleAngle = ThreadLocalRandom.Current.NextBool() ? DxRadian.West : DxRadian.East;
                var particleVelocity =
                    new DxRadian(ThreadLocalRandom.Current.NextFloat(particleAngle.Value - maxSpread.Value / 2,
                        particleAngle.Value + maxSpread.Value / 2)).UnitVector;

                var force = ThreadLocalRandom.Current.NextFloat(minForce, maxForce);
                particleVelocity *= force;

                var acceleration = particleVelocity.UnitVector *
                                   ThreadLocalRandom.Current.NextFloat(minAcceleration, maxAcceleration);
                var timeToLive = TimeSpan.FromSeconds(ThreadLocalRandom.Current.NextFloat(minTimeToLive, maxTimeToLive));
                var growRate = ThreadLocalRandom.Current.NextFloat(minGrowRate, maxGrowRate);
                var radius = ThreadLocalRandom.Current.NextFloat(minRadius, maxRadius);
                var maximumDistance = ThreadLocalRandom.Current.NextFloat(minDistance, maxDistance);
                var transparencyWeight = ThreadLocalRandom.Current.NextFloat(0.1f, 1.0f);
                var particle =
                    Particle.Particle.Builder()
                        .WithPosition(position)
                        .WithVelocity(particleVelocity)
                        .WithAcceleration(acceleration)
                        .WithColor(deathColor)
                        .WithTimeToLive(timeToLive)
                        .WithGrowRate(growRate)
                        .WithRadius(radius)
                        .WithMaxDistance(maximumDistance)
                        .WithTransparencyWeight(transparencyWeight)
                        .Build();
                
                EntityCreatedMessage particlesCreated = new EntityCreatedMessage(particle);
                particlesCreated.Emit();
            }
        }
    }
}