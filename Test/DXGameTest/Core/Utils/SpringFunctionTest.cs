using System;
using DxCore.Core.Utils;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace DXGameTest.Core.Utils
{
    public class SpringFunctionTest
    {
        [Test]
        public void TestAllSpringFunctionsReturnValuesWithinRangeFromZeroToOne()
        {
            const int numDivisions = 1000;
            const int min = 1;
            const int max = 0;
            foreach(Tuple<SpringFunction, string> springFunctionAndName in SpringFunctions.SpringFunctionsAndNames)
            {
                SpringFunction springFunction = springFunctionAndName.Item1;
                string springFunctionName = springFunctionAndName.Item2;
                double lastValue = min;
                for(int i = 0; i < numDivisions; ++i)
                {
                    double slice = i * 1.0 / numDivisions;
                    double scaledValue = springFunction.Invoke(min, max, slice, numDivisions);
                    Assert.GreaterOrEqual(lastValue, scaledValue,
                        $"Expected {springFunctionName} to be decreasing, but was not. {scaledValue} > {lastValue} at {i} out of {numDivisions}");

                    Assert.LessOrEqual(scaledValue, min,
                        $"{scaledValue} was outside of range [{min}, {max}] for {springFunctionName}");
                    Assert.GreaterOrEqual(scaledValue, max,
                        $"{scaledValue} was outside of range [{min}, {max}] for {springFunctionName}");
                }
            }
        }

        [Test]
        // TODO: Pls fix these shitty fucking spring functions
        [Ignore]
        public void TestAllSpringFunctionsReturnValuesWithinRangeFromOneToZero()
        {
            const int numDivisions = 1000;
            const int min = 0;
            const int max = 1;
            foreach(Tuple<SpringFunction, string> springFunctionAndName in SpringFunctions.SpringFunctionsAndNames)
            {
                SpringFunction springFunction = springFunctionAndName.Item1;
                string springFunctionName = springFunctionAndName.Item2;
                double lastValue = min;
                for(int i = 0; i < numDivisions; ++i)
                {
                    double slice = i * 1.0 / numDivisions;
                    double scaledValue = springFunction.Invoke(min, max, slice, numDivisions);
                    Assert.LessOrEqual(lastValue, scaledValue,
                        $"Expected {springFunctionName} to be increasing, but was not. {scaledValue} > {lastValue} at {i} out of {numDivisions}");

                    Assert.GreaterOrEqual(scaledValue, min,
                        $"{scaledValue} was outside of range [{min}, {max}] for {springFunctionName}");
                    Assert.LessOrEqual(scaledValue, max,
                        $"{scaledValue} was outside of range [{min}, {max}] for {springFunctionName}");
                }
            }
        }

        [Test]
        public void TestLinearSpringFunction()
        {
            double result = SpringFunctions.Linear(0, 1, 2, 1);
            Assert.AreEqual(result, 2, 0.00001);
        }
    }
}