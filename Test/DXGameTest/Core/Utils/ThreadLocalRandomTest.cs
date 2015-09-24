using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Utils;
using NUnit.Framework;

namespace DXGameTest.Core.Utils
{
    public class ThreadLocalRandomTest
    {
        [Test]
        public void TestThreadLocalRandomSpeed()
        {
            int numRounds = 1000000;
            for (int i = 0; i < numRounds; ++i)
            {
                ThreadLocalRandom.Current.Next();
            }

            var list = new List<int>(numRounds);
            var stopWatch = Stopwatch.StartNew();
            for (int i = 0; i < numRounds; ++i)
            {
                list.Add(ThreadLocalRandom.Current.Next());
            }
            stopWatch.Stop();
            Console.WriteLine($"ThreadLocalRandom took: {stopWatch.ElapsedMilliseconds}ms");
        }
    }
}
