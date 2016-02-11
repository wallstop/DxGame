using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Messaging.Entity;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Damage
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
    [ProtoContract]
    public class DeathEffectComponent : Component
    {
        [DataMember] [ProtoMember(1)] protected DeathEffect deathEffect_;

        public DeathEffectComponent(DeathEffect deathEffect)
        {
            Validate.IsNotNull(deathEffect, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(DeathEffect)));
            deathEffect_ = deathEffect;
            MessageHandler.RegisterMessageHandler<EntityDeathMessage>(HandleEntityDeath);
        }

        protected virtual void HandleEntityDeath(EntityDeathMessage deathMessage)
        {
            var spatial = deathMessage.Entity?.ComponentOfType<SpatialComponent>();
            /* 
                If there is a spatial, trigger the death effect. 
                If there isn't one, we can't reliably determine the space that the entity died at, so don't trigger it. 
            */
            if(!ReferenceEquals(spatial, null))
            {
                deathEffect_.Invoke(spatial.Space);
            }
            /* But regardless of whether or not a spatial exists, we want to kill this guy - remove everything from the game */
            DxGame.Instance.RemoveGameObject(Parent);
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

                DxGame.Instance.AddAndInitializeComponents(particle);
            }
        }
    }
}