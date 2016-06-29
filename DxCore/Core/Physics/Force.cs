using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Physics
{
    /**
        <summary>
            Returns a tuple [isDissipated, newAcceleration] given the current acceleration value & the gameTime
            TODO: Invert this. This should return (isValid, newAccelerationIfSo), not what it currently is :(. Bad API
        </summary>
    */

    public delegate bool DissipationFunction(DxVector2 externalVelocity, DxVector2 currentAcceleration, DxGameTime gameTime, out DxVector2 newAcceleration);

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
            Validate.Hard.IsNotNull(dissipationFunction, () => this.GetFormattedNullOrDefaultMessage(dissipationFunction));
            Dissipation = dissipationFunction;
            Dissipated = dissipated;
            InitialVelocity = initialVelocity;
            Acceleration = acceleration;
            Name = name;
        }

        public static DissipationFunction OneFrameDissipation(DxVector2 nextFrameAcceleration)
        {
            return new SerializableDissipation(nextFrameAcceleration).Dissipation;
        }

        public static bool InstantDisipation(DxVector2 velocity, DxVector2 acceleration,
            DxGameTime gameTime, out DxVector2 newAcceleration)
        {
            newAcceleration = DxVector2.EmptyVector;
            return true;
        }

        public void Update(DxVector2 velocity, DxVector2 acceleration, DxGameTime gameTime)
        {
            if (!Dissipated)
            {
                DxVector2 newAcceleration;
                Dissipated = Dissipation.Invoke(velocity, acceleration, gameTime, out newAcceleration);
                Acceleration = newAcceleration;
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

        [Serializable]
        [DataContract]
        internal sealed class SerializableDissipation
        {
            [DataMember]
            private bool Dissipated { get; set; }

            [DataMember]
            private DxVector2 NextFrameAcceleration { get; set; }

            public SerializableDissipation(DxVector2 nextFrameAcceleration)
            {
                NextFrameAcceleration = nextFrameAcceleration;
                Dissipated = false;
            }

            public bool Dissipation(DxVector2 externalVelocity, DxVector2 currentAcceleration,
                DxGameTime gameTime, out DxVector2 newAcceleration)
            {
                if(Dissipated)
                {
                    newAcceleration = DxVector2.EmptyVector;
                    return true;
                }
                newAcceleration = NextFrameAcceleration;
                Dissipated = true;
                return false;
            }
        }
    }
}