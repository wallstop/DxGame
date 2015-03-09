using System.Diagnostics;
using DXGame.Core.Components.Basic;
using DXGame.Main;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    public class WeaponComponent : Component
    {
        public int Damage { get; private set; }

        public WeaponComponent(DxGame game)
            : base(game)
        {
        }

        public WeaponComponent WithDamage(int damage)
        {
            Debug.Assert(damage >= 0, "Weapons can't have negative damage!");
            Damage = damage;
            return this;
        }

        public virtual void Attack(GameTime gameTime)
        {
            // No op in base
        }

        public override void Write(NetOutgoingMessage message)
        {
            throw new System.NotImplementedException();
        }

        public override void Read(NetIncomingMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}