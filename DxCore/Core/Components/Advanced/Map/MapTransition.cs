using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Components.Advanced.Map
{
    [Serializable]
    [DataContract]
    public class MapTransition : DrawableComponent, IEnvironmentComponent
    {
        private static readonly TimeSpan PARTICLE_EMISSION_DELAY = TimeSpan.FromSeconds(0.01);

        [DataMember]
        private TimeSpan LastParticleEmission { get; set; }

        [DataMember]
        private PhysicsComponent PositionalComponent { get; }

        [DataMember]
        public bool Active { get; private set; }

        public MapTransition(PhysicsComponent position)
        {
            Validate.Hard.IsNotNull(position);
            PositionalComponent = position;
            Active = true;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<EnvironmentInteractionMessage>(HandleEnvironmentInteraction);
            base.OnAttach();
        }

        public DxVector2 Position => PositionalComponent.Position;

        private void HandleEnvironmentInteraction(EnvironmentInteractionMessage message)
        {
            if(!Active)
            {
                return;
            }
            MapRotationRequest mapRotationRequest = new MapRotationRequest();
            mapRotationRequest.Emit();
            Active = false; // Disable once we're done so we don't accidentally double-trigger
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            if(!Active)
            {
                return;
            }
            if(LastParticleEmission + PARTICLE_EMISSION_DELAY < gameTime.TotalGameTime)
            {
                EmitParticle();
                LastParticleEmission = gameTime.TotalGameTime;
            }
        }

        private void EmitParticle()
        {
            Color mapTransitionColor = Color.Blue;

            float transparencyWeight = ThreadLocalRandom.Current.NextFloat(0.1f, 0.85f);
            TimeSpan timeToLive = TimeSpan.FromSeconds(ThreadLocalRandom.Current.NextDouble(0.5, 1.2));
            DxRadian direction = new DxRadian(ThreadLocalRandom.Current.NextFloat(DxRadian.MinValue, DxRadian.MaxValue));
            DxVector2 velocity = direction.UnitVector * ThreadLocalRandom.Current.NextFloat(0.01f, 1.0f);
            float radius = ThreadLocalRandom.Current.NextFloat(3.0f, 7.0f);
            Particle.Particle mapTransitionParticle =
                Particle.Particle.Builder()
                    .WithColor(mapTransitionColor)
                    .WithTransparencyWeight(transparencyWeight)
                    .WithTimeToLive(timeToLive)
                    .WithVelocity(velocity)
                    .WithRadius(radius)
                    .WithPosition(Position)
                    .Build();

            EntityCreatedMessage entityCreated = new EntityCreatedMessage(mapTransitionParticle);
            entityCreated.Emit();
        }
    }
}