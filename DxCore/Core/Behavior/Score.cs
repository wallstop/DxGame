﻿using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Behavior
{
    /**
        Simple "Score" class for encapsulating (but not exposing!) score in terms of numbers and stuff.

        Uses natural numeric ordering to do comparisons.

        <summary>
            Encapsualtes utilitarian score concepts
        </summary>
    */

    [Serializable]
    [DataContract]
    public class Score : IComparable<Score>, IEquatable<Score>
    {
        public static Score Min { get; } = new Score(float.MinValue);

        public static Score Max { get; } = new Score(float.MaxValue);

        [DataMember]
        private float Value { get; }

        public Score(float score)
        {
            Value = score;
        }

        public Score(Score other)
        {
            Validate.Hard.IsNotNull(other, this.GetFormattedNullOrDefaultMessage(other));
            Value = other.Value;
        }

        /* TODO: Weighting & math operations of scores */

        public int CompareTo(Score other)
        {
            return Value.CompareTo(other?.Value);
        }

        public bool Equals(Score other)
        {
            /* Score may be roughly computed, so fuzzy equality is probably best */
            return !ReferenceEquals(null, other) && Value.FuzzyCompare(other.Value) == 0;
        }

        public override bool Equals(object other)
        {
            var score = other as Score;
            return !ReferenceEquals(null, score) && Equals(score);
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(Value);
        }

        public override string ToString()
        {
            return Value.ToString("N2");
        }
    }
}