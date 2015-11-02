using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Advanced.Map
{
    [Serializable]
    [DataContract]
    public class MapTransition : DrawableComponent, IEnvironmentComponent
    {
        private static readonly TimeSpan PARTICLE_EMISSION_DELAY = TimeSpan.FromSeconds(0.01);

        public DxVector2 Position => PositionalComponent.Position;

        [DataMember]
        private TimeSpan LastParticleEmission { get; set; }

        [DataMember]
        private PositionalComponent PositionalComponent { get; }

        [DataMember]
        public bool Active { get; private set; }

        public MapTransition(PositionalComponent position)
        {
            Validate.IsNotNull(position);
            PositionalComponent = position;
            MessageHandler.RegisterMessageHandler<EnvironmentInteractionMessage>(HandleEnvironmentInteraction);
            MessageHandler.RegisterMessageHandler<LevelEndRequest>(HandleLevelEndRequest);
            Active = false;
        }

        private void HandleLevelEndRequest(LevelEndRequest message)
        {
            Active = true;
        }

        private void CheckForLevelEndRequest(DxGameTime gameTime)
        {
            if (Active)
            {
                return;
            }
            EventModel eventModel = DxGame.Instance.Model<EventModel>();
            if (ReferenceEquals(eventModel, null))
            {
                return;
            }
            EventRequest levelEndRequestRequest = EventRequest.Builder().WithType<LevelEndRequest>().Build();
            List<Event> levelEndEvents = eventModel.EventsFor(levelEndRequestRequest, gameTime);
            List<LevelEndRequest> levelEndRequests =
                levelEndEvents.Select(endEvent => endEvent.Message as LevelEndRequest).ToList();
            foreach (LevelEndRequest levelEndRequest in levelEndRequests)
            {
                HandleLevelEndRequest(levelEndRequest);
            }
        }

        protected override void Update(DxGameTime gameTime)
        {
            CheckForLevelEndRequest(gameTime);
        }

        private void HandleEnvironmentInteraction(EnvironmentInteractionMessage message)
        {
            if(!Active)
            {
                return;
            }
            MapRotationRequest mapRotationRequest = new MapRotationRequest();
            DxGame.Instance.BroadcastMessage<MapRotationRequest>(mapRotationRequest);
            Active = false; // Disable once we're done so we don't accidentally double-trigger
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            if(!Active)
            {
                return;
            }
            if (LastParticleEmission + PARTICLE_EMISSION_DELAY < gameTime.TotalGameTime)
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
