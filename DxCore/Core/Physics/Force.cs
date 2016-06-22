using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using DXGame.Core.Utils;

namespace DxCore.Core.Physics
{
    /**
        <summary>
            Returns a tuple [isDissipated, newAcceleration] given the current acceleration value & the gameTime
        </summary>
    */

    public delegate Tuple<bool, DxVector2> DissipationFunction(
        DxVector2 externalVelocity, DxVector2 currentAcceleration, DxGameTime gameTime);

    /**
        <summary>
            Represents some kind of external force that can be applied to physics-type objects. An easy example is Gravity.
        </summary>
    */

    [Serializable]
    [DataContract]
    public class Force
    {
        [DataMember]
        public DxVector2 InitialVelocity { get; }

        [DataMember]
        public DxVector2 Acceleration { get; private set; }

        [DataMember]
        public bool Dissipated { get; private set; }

        [DataMember]
        protected DissipationFunction Dissipation { get; }

        [DataMember]
        public string Name { get; }

        /** 
            <summary>
                This force does absolutely nothing and dissipates immediately.
            </summary>
        */
        public static Force NullForce
            =>
                new Force(DxVector2.EmptyVector, DxVector2.EmptyVector, InstantDisipation, "NullForce", true);

        public Force(DxVector2 initialVelocity, DxVector2 acceleration, DissipationFunction dissipationFunction,
            string name, bool dissipated = false)
        {
            Validate.Hard.IsNotNull(dissipationFunction,
                StringUtils.GetFormattedNullOrDefaultMessage(this, dissipationFunction));
            Dissipation = dissipationFunction;
            Dissipated = dissipated;
            InitialVelocity = initialVelocity;
            Acceleration = acceleration;
            Name = name;
        }

        public static Tuple<bool, DxVector2> InstantDisipation(DxVector2 velocity, DxVector2 acceleration,
            DxGameTime gameTime)
        {
            return Tuple.Create(true, DxVector2.EmptyVector);
        }

        public void Update(DxVector2 velocity, DxVector2 acceleration, DxGameTime gameTime)
        {
            if (!Dissipated)
            {
                var dissipation = Dissipation.Invoke(velocity, Acceleration, gameTime);
                Dissipated = dissipation.Item1;
                Acceleration = dissipation.Item2;
            }
        }

        public override bool Equals(object other)
        {
            var force = other as Force;
            if (force == null)
            {
                return false;
            }
            return InitialVelocity == force.InitialVelocity && Acceleration == force.Acceleration &&
                   Dissipated == force.Dissipated && Objects.Equals(Dissipation, force.Dissipation);
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(Name, Dissipation, InitialVelocity);
        }
    }
}