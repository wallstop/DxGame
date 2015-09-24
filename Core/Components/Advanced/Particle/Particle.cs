using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog.Time;

namespace DXGame.Core.Components.Advanced.Particle
{
    [Serializable]
    [DataContract]
    public class Particle : DrawableComponent
    {
        protected Color Color { get; }
        protected float Radius { get; set; }
        protected float AccumulatedRadius { get; set; }
        protected float GrowRate { get; }
        protected DxVector2 Position { get; set; }
        protected DxVector2 Velocity { get; set; }
        protected DxVector2 Acceleration { get; }
        protected TimeSpan TimeToLive { get; set; }

        protected Particle(DxGame game, Color color, float radius, float growRate, DxVector2 position, DxVector2 velocity, DxVector2 acceleration, TimeSpan timeToLive) 
            : base(game)
        {
            Color = color;
            Radius = radius;
            GrowRate = growRate;
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            TimeToLive = timeToLive;
            AccumulatedRadius = Radius;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            TimeToLive -= gameTime.ElapsedGameTime;
            if (TimeToLive <= TimeSpan.Zero)
            {
                Remove();
                return;
            }

            var scaleFactor = (float)gameTime.ElapsedGameTime.TotalMilliseconds / DateTimeConstants.MILLISECONDS_PER_SECOND;
            AccumulatedRadius += (Radius * scaleFactor* GrowRate);
            if (AccumulatedRadius < 0)
            {
                Remove();
                return;
            }
            Velocity += (Acceleration * scaleFactor);
            Position += (Velocity * scaleFactor);
            
            var destination = new DxRectangle(Position.X - AccumulatedRadius, Position.Y - AccumulatedRadius, AccumulatedRadius * 2, AccumulatedRadius * 2);
            spriteBatch.DrawCircle(destination.ToRectangle(), Color);
        }

        public static ParticleBuilder Builder()
        {
            return new ParticleBuilder();
        }

        public class ParticleBuilder : IBuilder<Particle>
        {
            private Color color_ = Color.Gray;
            private float radius_ = 1.0f;
            private float growRate_;
            private DxVector2 velocity_;
            private DxVector2 position_;
            private DxVector2 acceleration_;
            private DxGame game_;
            private TimeSpan timeToLive_ = TimeSpan.FromSeconds(1.0);

            public ParticleBuilder WithGrowRate(float growRate)
            {
                growRate_ = growRate;
                return this;
            }

            public ParticleBuilder WithTimeToLive(TimeSpan timeToLive)
            {
                timeToLive_ = timeToLive;
                return this;
            }

            public ParticleBuilder WithGame(DxGame game)
            {
                game_ = game;
                return this;
            }

            public ParticleBuilder WithPosition(DxVector2 position)
            {
                position_ = position;
                return this;
            }

            public ParticleBuilder WithAcceleration(DxVector2 acceleration)
            {
                acceleration_ = acceleration;
                return this;
            }

            public ParticleBuilder WithVelocity(DxVector2 velocity)
            {
                velocity_ = velocity;
                return this;
            }

            public ParticleBuilder WithColor(Color color)
            {
                color_ = color;
                return this;
            }

            public ParticleBuilder WithRadius(float radius)
            {
                radius_ = radius;
                return this;
            }

            public Particle Build()
            {
                if (ReferenceEquals(game_, null))
                {
                    game_ = DxGame.Instance;
                }
                Validate.IsTrue(radius_ > 0, $"Cannot create {typeof(Particle)}s with a negative radius ({radius_})");
                return new Particle(game_, color_, radius_, growRate_, position_, velocity_, acceleration_, timeToLive_);
            }
        }
    }
}
