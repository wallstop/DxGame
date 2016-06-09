using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Utils
{
    public static class ProducerConsumerCollectionExtensions
    {

        public static void Clear<T>(this IProducerConsumerCollection<T> queue)
        {
            T ignored;
            while(queue.TryTake(out ignored))
            {
                // This page intentionally left blank
            }
        }
    }
}
