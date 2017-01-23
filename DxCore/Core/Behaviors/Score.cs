using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using WallNetCore.Validate;

namespace DXGame.Core.Behaviors
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
        private static readonly float MIN = 0f;
        private static readonly float MAX = 1f;
        public static Score Max => new Score(MAX);

        public static Score Min => new Score(MIN);

        [DataMember]
        private float Value { get; set; }

        public Score(float score)
        {
            Validate.Hard.IsInClosedInterval(score, MIN, MAX, $"Scores must be normalized to {MIN}, {MAX}]!");
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
            return !ReferenceEquals(null, other) && (Value.FuzzyCompare(other.Value) == 0);
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