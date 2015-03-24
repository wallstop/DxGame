using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class WeaponComponent : Component
    {
        [DataMember]
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

        public virtual void Attack(DxGameTime gameTime)
        {
            // No op in base
        }
    }
}