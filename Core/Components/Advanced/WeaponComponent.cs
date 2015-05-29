using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
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
            Validate.IsTrue(damage >= 0, $"{GetType()} should not be able to have negative {nameof(damage)} ({damage})");
            Damage = damage;
            return this;
        }

        public virtual void Attack(DxGameTime gameTime)
        {
            // No op in base
        }
    }
}