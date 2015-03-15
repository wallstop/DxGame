using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
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
    }
}