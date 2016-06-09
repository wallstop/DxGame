using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;

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
            Validate.IsTrue(experience >= 0, $"Cannot create an {typeof(Experience)} with a value of {experience}");
            Value = experience;
        }
    }
}