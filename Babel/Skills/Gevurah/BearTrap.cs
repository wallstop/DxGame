using System;
using System.Runtime.Serialization;
using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Particle;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Primitives;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Babel.Skills.Gevurah
{
    [Serializable]
    [DataContract]
    public class BearTrap : DrawableComponent
    {
        private static readonly string BEAR_TRAP_ASSET = "Player/Archer/BearTrap";

        [DataMember] private readonly SpatialComponent spatial_;

        [DataMember]
        private bool Triggered { get; set; } = false;

        [DataMember] private Texture2D bearTrap_;

        public BearTrap(SpatialComponent spatial)
        {
            Validate.IsNotNull(spatial, this.GetFormattedNullOrDefaultMessage(spatial));
            spatial_ = spatial;
        }

        public override void LoadContent()
        {
            bearTrap_ = DxGame.Instance.Content.Load<Texture2D>(BEAR_TRAP_ASSET);
        }

        protected override void Update(DxGameTime gameTime)
        {
            if (Triggered)
            {
                Remove();
                Parent.Remove();
                Particle particle =
                    Particle.Builder()
                            .WithColor(Color.Gray)
                            .WithPosition(spatial_.Position)
                            .WithGrowRate(1.0f)
                            .WithTimeToLive(TimeSpan.FromSeconds(1))
                            .WithRadius(10)
                            .Build();
                EntityCreatedMessage entityCreated = new EntityCreatedMessage(particle);
                entityCreated.Emit();
                return;
            }

            PhysicsMessage stepOnBearTrap = new PhysicsMessage();
            stepOnBearTrap.AffectedAreas.Add(spatial_.Space);
            stepOnBearTrap.Source = Parent;
            stepOnBearTrap.Interaction = Trigger;
            stepOnBearTrap.Emit();
        }

        private void Trigger(GameObject source, PhysicsComponent destination)
        {
            DamageMessage damage = new DamageMessage
            {
                DamageCheck = DamageCheck,
                Source = Parent,
                Target = destination.Parent?.Id
            };
            damage.Emit();
        }

        private Tuple<bool, double> DamageCheck(GameObject source, GameObject destination)
        {
            if (source == destination)
            {
                return Tuple.Create(false, 0.0);
            }
            Triggered = true;
            return Tuple.Create(true, 10.0);
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.Draw(bearTrap_, destinationRectangle: spatial_.Space.ToRectangle());
        }
    }
}
