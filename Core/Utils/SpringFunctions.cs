using System;
using System.Collections.Generic;
using System.Reflection;

namespace DXGame.Core.Utils
{
    public delegate double SpringFunction(double start, double end, double instant, double duration);

    public static class SpringFunctions
    {
        public static List<Tuple<SpringFunction, string>> SpringFunctionsAndNames
        {
            get
            {
                /* Rip every function that is a SpringFunction off of the class via Reflection (what could go wrong?) */
                /* TODO: Cache */
                var staticPublicMethods = typeof(SpringFunctions).GetMethods(BindingFlags.Static | BindingFlags.Public);
                var springFunctions = new List<Tuple<SpringFunction, string>>(staticPublicMethods.Length);
                foreach(var staticPublicMethod in staticPublicMethods)
                {
                    try
                    {
                        var springFunction =
                            (SpringFunction) Delegate.CreateDelegate(typeof(SpringFunction), staticPublicMethod);
                        springFunctions.Add(Tuple.Create(springFunction, staticPublicMethod.Name));
                    }
                    catch(Exception)
                    {
                        // Lol woops, not a SpringFunction, pls ignore
                    }
                }
                return springFunctions;
            }
        }

        /* Spring Functions borrowed from http://robertpenner.com/easing/ */

        public static double Linear(double start, double end, double instant, double duration)
        {
            return end * instant / duration + start;
        }

        public static double ExponentialEaseOut(double start, double end, double instant, double duration)
        {
            return (instant.FuzzyCompare(end) == 0) ? (start + end) : end * (-Math.Pow(2, -10 * instant / duration) + 1) + start;
        }

        public static double ExponentialEaseIn(double start, double end, double instant, double duration)
        {
            return (instant.FuzzyCompare(0) == 0) ? start : end * Math.Pow(2, 10 * (instant / duration - 1)) + start;
        }

        public static double ExponentialEaseInOut(double start, double end, double instant, double duration)
        {
            if(instant.FuzzyCompare(0) == 0)
            {
                return start;
            }
            if(instant.FuzzyCompare(duration) == 0)
            {
                return start + end;
            }
            if((instant /= 2) < 1)
            {
                return end / 2 * Math.Pow(2, 10 * (instant - 1)) + start;
            }

            return end / 2 * (-Math.Pow(2, -10 * --instant) + 2) + start;
        }

        public static double ExponentialEaseOutIn(double start, double end, double instant, double duration)
        {
            return InternalEaseOutIn(ExponentialEaseOut, ExponentialEaseIn, start, end, instant, duration);
        }

        public static double CircularEaseOut(double start, double end, double instant, double duration)
        {
            return end * Math.Sqrt(1 - (instant = instant / duration - 1) * instant) + start;
        }

        public static double CircularEaseIn(double start, double end, double instant, double duration)
        {
            return -end * (Math.Sqrt(1 - (instant /= duration) * instant) - 1) + start;
        }

        public static double CircularEaseInOut(double start, double end, double instant, double duration)
        {
            if((instant /= duration / 2) < 1)
            {
                return -end / 2 * (Math.Sqrt(1 - instant * instant) - 1) + start;
            }
            return end / 2 * (Math.Sqrt(1 - (instant -= 2) * instant) + 1) + start;
        }

        public static double CircularEaseOutIn(double start, double end, double instant, double duration)
        {
            return InternalEaseOutIn(CircularEaseOut, CircularEaseIn, start, end, instant, duration);
        }

        public static double QuadraticEaseOut(double start, double end, double instant, double duration)
        {
            return -end * (instant /= duration) * (instant - 2) + start;
        }

        public static double QuadraticEaseIn(double start, double end, double instant, double duration)
        {
            return end * (instant /= duration) * instant + start;
        }

        public static double QuadraticEaseInOut(double start, double end, double instant, double duration)
        {
            if((instant /= duration / 2) < 1)
            {
                return end / 2 * instant * instant + start;
            }
            return -end / 2 * ((--instant) * (instant - 2) - 1) + start;
        }

        public static double QuadraticEaseOutIn(double start, double end, double instant, double duration)
        {
            return InternalEaseOutIn(QuadraticEaseOut, QuadraticEaseIn, start, end, instant, duration);
        }

        public static double SineEaseOut(double start, double end, double instant, double duration)
        {
            return end * Math.Sin(instant / duration * (Math.PI / 2)) + start;
        }

        public static double SineEaseIn(double start, double end, double instant, double duration)
        {
            return -end * Math.Cos(instant / duration * (Math.PI / 2)) + end + start;
        }

        public static double SineEaseInOut(double start, double end, double instant, double duration)
        {
            if((instant /= duration / 2) < 1)
            {
                return end / 2 * (Math.Sin(Math.PI * instant / 2)) + start;
            }
            return -end / 2 * (Math.Cos(Math.PI * --instant) - 2) + start;
        }

        public static double SineEaseOutIn(double start, double end, double instant, double duration)
        {
            return InternalEaseOutIn(SineEaseOut, SineEaseIn, start, end, instant, duration);
        }

        public static double CubicEaseOut(double start, double end, double instant, double duration)
        {
            return end * ((instant = instant / duration - 1) * instant * instant + 1) + start;
        }

        public static double CubicEaseIn(double start, double end, double instant, double duration)
        {
            return end * (instant /= duration) * instant * instant + start;
        }

        public static double CubicEaseInOut(double start, double end, double instant, double duration)
        {
            if((instant /= duration / 2) < 1)
            {
                return end / 2 * instant * instant * instant + start;
            }
            return end / 2 * ((instant -= 2) * instant * instant + 2) + start;
        }

        public static double CubicEaseOutIn(double start, double end, double instant, double duration)
        {
            return InternalEaseOutIn(CubicEaseOut, CubicEaseIn, start, end, instant, duration);
        }

        public static double QuarticEaseOut(double start, double end, double instant, double duration)
        {
            return -end * ((instant = instant / duration - 1) * instant * instant * instant - 1) + start;
        }

        public static double QuarticEaseIn(double start, double end, double instant, double duration)
        {
            return end * (instant /= duration) * instant * instant * instant + start;
        }

        public static double QuarticEaseInOut(double start, double end, double instant, double duration)
        {
            if((instant /= duration / 2) < 1)
            {
                return end / 2 * instant * instant * instant * instant + start;
            }

            return -end / 2 * ((instant -= 2) * instant * instant * instant - 2) + start;
        }

        public static double QuarticEaseOutIn(double start, double end, double instant, double duration)
        {
            return InternalEaseOutIn(QuarticEaseOut, QuarticEaseIn, start, end, instant, duration);
        }

        public static double ElasticEaseOut(double start, double end, double instant, double duration)
        {
            if((instant /= duration).FuzzyCompare(1) == 0)
            {
                return start + end;
            }
            double p = duration * 0.3;
            double s = p / 4;
            return (end * Math.Pow(2, -10 * instant) * Math.Sin((instant * duration - s) * (2 * Math.PI) / p) + start +
                    end);
        }

        public static double ElasticEaseIn(double start, double end, double instant, double duration)
        {
            if((instant /= duration).FuzzyCompare(1) == 0)
            {
                return start + end;
            }
            double p = duration * 0.3;
            double s = p / 4;

            return -(end * Math.Pow(2, 10 * (instant -= 1)) * Math.Sin((instant * duration - s) * (2 * Math.PI) / p)) +
                   start;
        }

        public static double ElasticEaseInOut(double start, double end, double instant, double duration)
        {
            if((instant /= duration / 2).FuzzyCompare(2) == 0)
            {
                return start + end;
            }
            double p = duration * (0.3 * 1.5);
            double s = p / 4;
            if(instant < 1)
            {
                return -0.5 *
                       (end * Math.Pow(2, 10 * (instant -= 1)) * Math.Sin((instant * duration - s) * (2 * Math.PI) / p)) +
                       start;
            }
            return end * Math.Pow(2, -10 * (instant -= 1)) * Math.Sin((instant * duration - s) * (2 * Math.PI) / p) *
                   0.5 + start + end;
        }

        public static double ElasticEaseOutIn(double start, double end, double instant, double duration)
        {
            return InternalEaseOutIn(ElasticEaseOut, ElasticEaseIn, start, end, instant, duration);
        }

        public static double BounceEaseOut(double start, double end, double instant, double duration)
        {
            const double multiplier = 7.5625;
            const double divisor = 2.75;
            if((instant /= duration) < (1.0 / divisor))
            {
                return end * (multiplier * instant * instant) + start;
            }
            if(instant < (2.0 / divisor))
            {
                return end * (multiplier * (instant -= (1.5 / divisor)) * instant + 0.75) + start;
            }
            if(instant < (2.5 / divisor))
            {
                return end * (multiplier * (instant -= (2.25 / divisor)) * instant + 0.9375) + start;
            }
            return end * (multiplier * (instant -= (2.625 / divisor)) * instant + 0.984375) + start;
        }

        public static double BounceEaseIn(double start, double end, double instant, double duration)
        {
            return end - BounceEaseOut(0, end, duration - instant, duration) + start;
        }

        public static double BounceEaseInOut(double start, double end, double instant, double duration)
        {
            if(instant < duration / 2)
            {
                return BounceEaseIn(0, end, instant * 2, duration) * 0.5 + start;
            }
            return BounceEaseOut(0, end, instant * 2 - duration, instant) * 0.5 + end * 0.5 + start;
        }

        public static double BounceEaseOutIn(double start, double end, double instant, double duration)
        {
            return InternalEaseOutIn(BounceEaseOut, BounceEaseIn, start, end, instant, duration);
        }

        private static double InternalEaseOutIn(SpringFunction easeOut, SpringFunction easeIn, double start, double end,
            double instant, double duration)
        {
            if(instant < duration / 2)
            {
                TransformInputToEeaseIn(ref start, ref end, ref instant, ref duration);
                return easeOut(start, end, instant, duration);
            }
            TransformInputToEaseOut(ref start, ref end, ref instant, ref duration);
            return easeIn(start, end, instant, duration);
        }

        private static void TransformInputToEeaseIn(ref double start, ref double end, ref double instant,
            ref double duration)
        {
            end /= 2;
            instant *= 2;
        }

        private static void TransformInputToEaseOut(ref double start, ref double end, ref double instant,
            ref double duration)
        {
            end /= 2;
            start += end;
            instant = instant * 2 - duration;
        }
    }
}
