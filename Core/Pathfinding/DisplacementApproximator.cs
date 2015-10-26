using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using MathNet.Numerics;
using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Pathfinding
{
    /**
        <summary>
            Utilizes curve-fitting techniques to answer key questions about how forces change position.
        </summary>
    */
    [Serializable]
    [DataContract]
    public class DisplacementApproximator
    {
        [DataMember]
        private double[] YTerms { get; }

        [DataMember]
        private double[] XTerms { get; }

        [DataMember]
        private double[] PositionalTerms { get; }

        [DataMember]
        private double[] XToTimeRegression { get; }

        /**
            <summary>
                Represents the absolute upper bounds that this displacement can have as an effect on an object
            </summary>
        */
        [DataMember]
        public DxRectangle Bounds { get; }

        [NonSerialized]
        [DataMember]
        private int hash_;

        public DisplacementApproximator(double[] xRegression, double[] yRegression, double[] positionalRegression,
            double[] xToTimeRegression, DxRectangle bounds)
        {
            XTerms = xRegression;
            YTerms = yRegression;
            PositionalTerms = positionalRegression;
            XToTimeRegression = xToTimeRegression;
            Bounds = bounds;
        }

        /**
            <summary>
                Given an amount of x displacement, answers the question "How much time will it take me to arrive at the x coordinate?"
            </summary>
        */
        public TimeSpan TimeFor(double x)
        {
            double timeInMillis = Evaluate.Polynomial(x, XToTimeRegression);
            if(timeInMillis < 0)
            {
                return TimeSpan.FromSeconds(int.MaxValue);
            }
            return TimeSpan.FromSeconds(timeInMillis);
        }

        /**
            <summary>
                Given an amount of x displacement, answers the question "If I am displaced by this much horizontally, how much will I be displaced vertically?"
                (Returns the y coordinate of the point on the curve that x belongs to)
            </summary>
        */
        public double PositionalDisplacement(double x)
        {
            return Evaluate.Polynomial(x, PositionalTerms);
        }

        /**
            <summary>
                Given an elapsed amount of time, returns the displacement vector that the entity should be at after the specified amount of time has elapsed
            </summary>
        */
        public DxVector2 TimeDisplacement(TimeSpan offset)
        {
            double time = offset.TotalSeconds;
            float x = (float) Evaluate.Polynomial(time, XTerms);
            if(float.IsNaN(x))
            {
                x = 0;
            }
            float y = (float) Evaluate.Polynomial(time, YTerms);
            if(float.IsNaN(y))
            {
                y = 0;
            }
            return new DxVector2(x, y);
        }

        public override bool Equals(object other)
        {
            DisplacementApproximator approximator = other as DisplacementApproximator;
            if(ReferenceEquals(approximator, null))
            {
                return GetHashCode() == approximator.GetHashCode() 
                    && Objects.Equals(XTerms, approximator.XTerms) 
                    && Objects.Equals(YTerms, approximator.YTerms) 
                    && Objects.Equals(PositionalTerms, approximator.PositionalTerms) 
                    && Objects.Equals(XToTimeRegression, approximator.XToTimeRegression) 
                    && Objects.Equals(Bounds, approximator.Bounds);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if(hash_ == 0)
            {
                hash_ = Objects.HashCode(XTerms, YTerms, PositionalTerms, XToTimeRegression, Bounds);
            }
            return hash_;
        }
    }
}
