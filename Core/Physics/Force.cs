using System;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.Physics
{

    /**
        <summary>
            Returns a tuple [isDissipated, newAcceleration] given the current acceleration value & the gameTime
        </summary>
    */
    public delegate Tuple<bool, DxVector2> DissipationFunction(DxVector2 externalVelocity, DxVector2 currentAcceleration, DxGameTime gameTime);

    /**
        <summary>
            Represents some kind of external force that can be applied to physics-type objects. An easy example is Gravity.
        </summary>
    */
    [Serializable]
    [DataContract]
    public class Force
    {
        /**
            Summary provides a quick and simple gist of what a Force is attempting to do. Jump provides an upwards vector 
            (of some magnitude). Moving right provides a right-wards vector (of some magnitude), for example 
        */
        [DataMember]
        public DxVector2 Summary { get; }
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

        public Force(DxVector2 initialVelocity, DxVector2 acceleration, DxVector2 summary, DissipationFunction dissipationFunction, string name, bool dissipated = false)
        {
            Validate.IsNotNull(dissipationFunction,
                StringUtils.GetFormattedNullOrDefaultMessage(this, dissipationFunction));
            Summary = summary;
            Dissipation = dissipationFunction;
            Dissipated = dissipated;
            InitialVelocity = initialVelocity;
            Acceleration = acceleration;
            Name = name;
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
            if (Objects.Equals(force, null))
            {
                return false;
            }
            return InitialVelocity == force.InitialVelocity && Acceleration == force.Acceleration &&
                   Dissipated == force.Dissipated;
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(InitialVelocity, Acceleration, Dissipated, Summary);
        }
    }
}