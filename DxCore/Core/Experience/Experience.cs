using System;
using System.Runtime.Serialization;
using WallNetCore.Validate;

namespace DxCore.Core.Experience
{
    /**
        <summary>
            Simple data wrapper around experience values
        </summary>
    */

    [DataContract]
    [Serializable]
    public struct Experience
    {
        [DataMember]
        public int Value { get; set; }

        public Experience(int experience)
        {
            Validate.Hard.IsTrue(experience >= 0,
                () => $"Cannot create an {typeof(Experience)} with a value of {experience}");
            Value = experience;
        }
    }
}