using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core;
using DXGame.Core.Components.Advanced.Particle;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.TowerGame.Skills.Gevurah
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
            Validate.IsNotNull(spatial, StringUtils.GetFormattedNullOrDefaultMessage(this, spatial));
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
                Dispose();
                DxGame.Instance.RemoveGameObject(Parent);
                Particle particle =
                    Particle.Builder()
                            .WithColor(Color.Gray)
                            .WithPosition(spatial_.Position)
                            .WithGrowRate(1.0f)
                            .WithTimeToLive(TimeSpan.FromSeconds(1))
                            .WithRadius(10)
                            .Build();
                DxGame.Instance.AddAndInitializeComponent(particle);
                return;
            }

            PhysicsMessage stepOnBearTrap = new PhysicsMessage();
            stepOnBearTrap.AffectedAreas.Add(spatial_.Space);
            stepOnBearTrap.Source = Parent;
            stepOnBearTrap.Interaction = Trigger;
            DxGame.Instance.BroadcastTypedMessage<PhysicsMessage>(stepOnBearTrap);
        }

        private void Trigger(GameObject source, PhysicsComponent destination)
        {
            DamageMessage damage = new DamageMessage
            {
                DamageCheck = DamageCheck,
                Source = Parent
            };
            destination.Parent?.BroadcastTypedMessage<DamageMessage>(damage);
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
