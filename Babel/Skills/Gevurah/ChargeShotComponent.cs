using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DXGame.Core;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using Microsoft.Xna.Framework.Graphics;

namespace Babel.Skills.Gevurah
{
    [Serializable]
    [DataContract]
    public class ChargeShotComponent : DrawableComponent
    {
        private static readonly string ARROW_LEFT_ASSET = "Player/Archer/ArrowLeft";
        private static readonly string ARROW_RIGHT_ASSET = "Player/Archer/ArrowRight";

        private static readonly int BASE_DAMAGE = 1;
        private static readonly int MAX_DAMAGE = 200;
        private static readonly TimeSpan MAX_CHARGE_TIME = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan MIN_LIFETIME = TimeSpan.FromSeconds(0.5);
        private static readonly TimeSpan MAX_LIFETIME = TimeSpan.FromSeconds(4);

        [DataMember] private Texture2D arrow_;

        [DataMember]
        private HashSet<GameObject> entitiesHit_ = new HashSet<GameObject>();
        
        [DataMember] private SpatialComponent spatial_;
        [DataMember] private Direction Direction { get; }
        [DataMember] private double Damage { get; }
        [DataMember] private TimeSpan LifeTime { get; }

        [DataMember] private TimeSpan currentDuration_;

        public ChargeShotComponent(SpatialComponent spatial, Direction facing, TimeSpan timeCharged)
        {
            Validate.IsNotNullOrDefault(spatial, this.GetFormattedNullOrDefaultMessage(spatial));
            spatial_ = spatial;
            Direction = facing;
            double maxMillis = MAX_CHARGE_TIME.TotalMilliseconds;
            double millisCharged = Math.Min(timeCharged.TotalMilliseconds, maxMillis);
            Damage = SpringFunctions.ExponentialEaseIn(BASE_DAMAGE, MAX_DAMAGE, millisCharged, maxMillis);
            LifeTime =
                TimeSpan.FromMilliseconds(SpringFunctions.ExponentialEaseIn(MIN_LIFETIME.TotalMilliseconds,
                    MAX_LIFETIME.TotalMilliseconds, millisCharged, maxMillis));
            currentDuration_ = TimeSpan.Zero;
        }

        public override void LoadContent()
        {
            switch(Direction)
            {
                case Direction.West:
                    arrow_ = DxGame.Instance.Content.Load<Texture2D>(ARROW_LEFT_ASSET);
                    break;
                case Direction.East:
                    arrow_ = DxGame.Instance.Content.Load<Texture2D>(ARROW_RIGHT_ASSET);
                    break;
                default:
                    throw new InvalidEnumArgumentException($"Could not determine Arrow asset for ${Direction}");
            }
        }

        public void Interact(GameObject sourceEntity, PhysicsComponent destinationPhysics)
        {
            GameObject hitEntity = destinationPhysics.Parent;
            /* Someone we've already seen? Pls ignore */
            if(!entitiesHit_.Add(hitEntity))
            {
                return;
            }
            DamageMessage damage = new DamageMessage {Source = sourceEntity, DamageCheck = ChargeShotDamageCheck, Target = destinationPhysics.Parent?.Id};
            damage.Emit();
        }

        private Tuple<bool, double> ChargeShotDamageCheck(GameObject source, GameObject destination)
        {
            if(source == destination)
            {
                return Tuple.Create(false, 0.0);
            }
            // Object has team and is a player? No friendly fire!
            TeamComponent team = destination.ComponentOfType<TeamComponent>();
            if(Objects.Equals(Team.PlayerTeam, team?.Team))
            {
                return Tuple.Create(false, 0.0);
            }
            return Tuple.Create(true, Damage);
        }
        
        protected override void Update(DxGameTime gameTime)
        {
            currentDuration_ += gameTime.ElapsedGameTime;
            if(currentDuration_ >= LifeTime)
            {
                // We're done here.
                Remove();
                Parent?.Remove();
                return;
            }
            
            PhysicsMessage interaction = new PhysicsMessage();
            interaction.AffectedAreas.Add(spatial_.Space);
            interaction.Source = Parent;
            interaction.Interaction = Interact;
            interaction.Emit();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.Draw(arrow_, destinationRectangle: spatial_.Space.ToRectangle());
        }
    }
}
