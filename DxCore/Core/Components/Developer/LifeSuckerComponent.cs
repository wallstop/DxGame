﻿using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;

namespace DxCore.Core.Components.Developer
{
    /**
        <summary> 
            Slowly drains the life from the game object that it is attached to. 
            Primarily for testing.
        </summary>
    */

    [Serializable]
    [DataContract]
    public class LifeSuckerComponent : Component
    {
        private const double LIFE_SUCK_DAMAGE = 1.0; // It hurtsss
        private static readonly TimeSpan LIFE_SUCK_TICK_FREQUENCY = TimeSpan.FromSeconds(1);
        private TimeSpan lastSucked_ = TimeSpan.Zero;

        protected override void Update(DxGameTime gameTime)
        {
            if(lastSucked_ + LIFE_SUCK_TICK_FREQUENCY <= gameTime.TotalGameTime)
            {
                SuckLife();
                lastSucked_ = gameTime.TotalGameTime;
            }
        }

        private void SuckLife()
        {
            DamageMessage lifeSuckMessage = new DamageMessage
            {
                Source = Parent,
                DamageCheck = LifeSuckDamage,
                Target = Parent?.Id
            };
            lifeSuckMessage.Emit();
        }

        private static Tuple<bool, double> LifeSuckDamage(GameObject source, GameObject destination)
        {
            /* 
                We don't really care who sent it or who is on the receiving end, 
                we suck the life out of everyone indescriminately 
            */
            return Tuple.Create(true, LIFE_SUCK_DAMAGE);
        }
    }
}