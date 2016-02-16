using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Map
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    public class MapTransition : DrawableComponent, IEnvironmentComponent
    {
        private static readonly TimeSpan PARTICLE_EMISSION_DELAY = TimeSpan.FromSeconds(0.01);

        [DataMember]
        [ProtoMember(1)]
        private TimeSpan LastParticleEmission { get; set; }

        [DataMember]
        [ProtoMember(2)]
        private PositionalComponent PositionalComponent { get; }

        [DataMember]
        [ProtoMember(3)]
        public bool Active { get; private set; }

        public MapTransition(PositionalComponent position)
        {
            Validate.IsNotNull(position);
            PositionalComponent = position;
            MessageHandler.RegisterMessageHandler<EnvironmentInteractionMessage>(HandleEnvironmentInteraction);
            Active = true;
        }

        public DxVector2 Position => PositionalComponent.Position;

        private void HandleEnvironmentInteraction(EnvironmentInteractionMessage message)
        {
            if(!Active)
            {
                return;
            }
            MapRotationRequest mapRotationRequest = new MapRotationRequest();
            DxGame.Instance.BroadcastTypedMessage(mapRotationRequest);
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
            DxGame.Instance.AddAndInitializeComponent(mapTransitionParticle);
        }
    }
}