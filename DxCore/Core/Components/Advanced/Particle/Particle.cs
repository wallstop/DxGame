﻿using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace DxCore.Core.Components.Advanced.Particle
{
    /**
        Particles are simply colored orbs, nothing more, nothing less. They can shrink or grow in regards to time, have a TTL, radius, position, velocity, and acceleration.
        <summary> 
            Simple "Particle" class. Operates on its own Physics system (for now) to avoid Physics components picking up / interacting with them. 
        </summary>
    */

    // TODO: Get rid of this crap and turn it into an emitter

    [Serializable]
    [DataContract]
    public class Particle : DrawableComponent
    {
        protected DxVector2 Acceleration { get; }
        /* The "current" radius. */
        protected float AccumulatedRadius { get; set; }
        protected Color Color { get; }
        protected float GrowRate { get; }
        protected float MaxDistance { get; set; }
        protected DxVector2 Position { get; set; }
        protected float Radius { get; }
        protected TimeSpan TimeToLive { get; set; }
        protected float TransparencyWeight { get; }
        protected DxVector2 Velocity { get; set; }

        protected Particle(Color color, float radius, float growRate, DxVector2 position, DxVector2 velocity,
            DxVector2 acceleration, TimeSpan timeToLive, float maxDistance, float transparencyWeight)
        {
            // TODO: Only draw / make particles if like "hefty" graphics options are specified.
            Color = color;
            Radius = radius;
            GrowRate = growRate;
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            TimeToLive = timeToLive;
            AccumulatedRadius = Radius;
            MaxDistance = maxDistance;
            TransparencyWeight = transparencyWeight;
        }

        public static ParticleBuilder Builder()
        {
            return new ParticleBuilder();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            TimeToLive -= gameTime.ElapsedGameTime;
            if(TimeToLive <= TimeSpan.Zero)
            {
                /* Is our TTL up? If so, die */
                Remove();
                return;
            }

            /* 
                We want to scale our radius with regards to time. 
                The scale factor returned from DetermineScaleFactor on DxGameTime operates in the concept of FPS (should 
                be something like a target 60FPS for the game) instead of straight wallclock time 
            */
            var growthScaleFactor = (float) gameTime.ElapsedGameTime.TotalMilliseconds /
                                    DateTimeConstants.MILLISECONDS_PER_SECOND;
            AccumulatedRadius += Radius * growthScaleFactor * GrowRate;
            if((AccumulatedRadius < 0) || (MaxDistance <= 0))
            {
                /* Have we shrunk to 0 or traveled farther than we should have? If so, die */
                Remove();
                return;
            }
            /* Othwerise, do our own physics computations (scale to FPS like normal physics components) */
            var scaleFactor = gameTime.ScaleFactor;

            Velocity += Acceleration * scaleFactor;
            var scaledVelocity = Velocity * scaleFactor;
            Position += scaledVelocity;
            MaxDistance -= scaledVelocity.Magnitude;
            var destination = new DxRectangle(Position.X - AccumulatedRadius, Position.Y - AccumulatedRadius,
                AccumulatedRadius * 2, AccumulatedRadius * 2);
            spriteBatch.DrawCircle(destination.ToRectangle(), Color, TransparencyWeight);
        }

        public class ParticleBuilder : IBuilder<Particle>
        {
            private DxVector2 acceleration_;
            private Color color_ = Color.Gray;
            private DxGame game_;
            private float growRate_;
            private float maxDistance_ = float.MaxValue;
            private DxVector2 position_;
            private float radius_ = 1.0f;
            private TimeSpan timeToLive_ = TimeSpan.FromSeconds(1.0);
            private float transparencyWeight_ = 1.0f;
            private DxVector2 velocity_;

            public Particle Build()
            {
                if(ReferenceEquals(game_, null))
                {
                    game_ = DxGame.Instance;
                }
                Validate.Hard.IsTrue(radius_ > 0,
                    $"Cannot create {typeof(Particle)}s with a negative radius ({radius_})");
                Validate.Hard.IsTrue(maxDistance_ >= 0,
                    $"Cannot create {typeof(Particle)}s with a negative maxDistance ({maxDistance_})");
                Validate.Hard.IsTrue((transparencyWeight_ >= 0) && (transparencyWeight_ <= 1.0f),
                    $"Cannot create {typeof(Particle)}s with an transparencyWeight that is not [0, 1] (was {transparencyWeight_})");
                return new Particle(color_, radius_, growRate_, position_, velocity_, acceleration_, timeToLive_,
                    maxDistance_, transparencyWeight_);
            }

            public ParticleBuilder WithAcceleration(DxVector2 acceleration)
            {
                acceleration_ = acceleration;
                return this;
            }

            public ParticleBuilder WithColor(Color color)
            {
                color_ = color;
                return this;
            }

            public ParticleBuilder WithGame(DxGame game)
            {
                game_ = game;
                return this;
            }

            /* 
                GrowRate > 0 : Enlarge radius
                GrowRate < 0 : Shrink radius
                GrowRate == 0: Maintain Radius
            */

            public ParticleBuilder WithGrowRate(float growRate)
            {
                growRate_ = growRate;
                return this;
            }

            public ParticleBuilder WithMaxDistance(float maxDistance)
            {
                maxDistance_ = maxDistance;
                return this;
            }

            public ParticleBuilder WithPosition(DxVector2 position)
            {
                position_ = position;
                return this;
            }

            public ParticleBuilder WithRadius(float radius)
            {
                radius_ = radius;
                return this;
            }

            public ParticleBuilder WithTimeToLive(TimeSpan timeToLive)
            {
                timeToLive_ = timeToLive;
                return this;
            }

            public ParticleBuilder WithTransparencyWeight(float transparencyWeight)
            {
                transparencyWeight_ = transparencyWeight;
                return this;
            }

            public ParticleBuilder WithVelocity(DxVector2 velocity)
            {
                velocity_ = velocity;
                return this;
            }
        }
    }
}