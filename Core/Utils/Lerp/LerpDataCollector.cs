using System;
using System.Collections.Concurrent;
using System.Threading;

namespace DXGame.Core.Utils.Lerp
{
    public struct LerpData<T>
    {
        public T OldValue { get; set; }
        public T NewValue { get; set; }
        public TimeSpan OldTime { get; set; }
        public TimeSpan NewTime { get; set; }
    }

    public class LerpDataCollector<T>
    {
        /* It only takes 2 to lerp */
        private const int BUFFER_SIZE = 2;

        private const int OLD_INDEX = 0;
        private const int NEW_INDEX = 1;

        private readonly
            ConcurrentDictionary<UniqueId, Tuple<ReaderWriterLockSlim, FixedSizedBuffer<Tuple<T, TimeSpan>>>>
            lerpBuffersById_;

        public LerpDataCollector()
        {
            lerpBuffersById_ =
                new ConcurrentDictionary<UniqueId, Tuple<ReaderWriterLockSlim, FixedSizedBuffer<Tuple<T, TimeSpan>>>>();
        }

        public bool Remove(UniqueId entityId)
        {
            Tuple<ReaderWriterLockSlim, FixedSizedBuffer<Tuple<T, TimeSpan>>> value;
            return lerpBuffersById_.TryRemove(entityId, out value);
        }

        public bool TryGetLerpData(UniqueId entityId, out LerpData<T> lerpData)
        {
            Tuple<ReaderWriterLockSlim, FixedSizedBuffer<Tuple<T, TimeSpan>>> value;
            if(lerpBuffersById_.TryGetValue(entityId, out value))
            {
                using(new CriticalRegion(value.Item1, CriticalRegion.LockType.Read))
                {
                    if(value.Item2.Count != BUFFER_SIZE)
                    {
                        lerpData = new LerpData<T>();
                        return false;
                    }
                    lerpData = new LerpData<T>
                    {
                        NewTime = value.Item2[NEW_INDEX].Item2,
                        NewValue = value.Item2[NEW_INDEX].Item1,
                        OldTime = value.Item2[OLD_INDEX].Item2,
                        OldValue = value.Item2[OLD_INDEX].Item1
                    };
                    return true;
                }
            }
            lerpData = new LerpData<T>();
            return false;
        }

        public void UpdateLerpData(UniqueId entityId, T dataPoint, TimeSpan snapshot)
        {
            Tuple<T, TimeSpan> newLerpDataPoint = new Tuple<T, TimeSpan>(dataPoint, snapshot);
            lerpBuffersById_.AddOrUpdate(entityId, key =>
            {
                FixedSizedBuffer<Tuple<T, TimeSpan>> newFixedSizedBuffer =
                    new FixedSizedBuffer<Tuple<T, TimeSpan>>(BUFFER_SIZE);
                newFixedSizedBuffer.Buffer(newLerpDataPoint);
                return
                    new Tuple<ReaderWriterLockSlim, FixedSizedBuffer<Tuple<T, TimeSpan>>>(
                        new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion), newFixedSizedBuffer);
            }, (key, existing) =>
            {
                using(new CriticalRegion(existing.Item1, CriticalRegion.LockType.Write))
                {
                    if(existing.Item2.Count < BUFFER_SIZE)
                    {
                        existing.Item2.Buffer(newLerpDataPoint);
                        return existing;
                    }

                    Tuple<T, TimeSpan> latestTimestampedLerpValue = existing.Item2[NEW_INDEX];
                    if(latestTimestampedLerpValue.Item2 < snapshot)
                    {
                        existing.Item2.Buffer(newLerpDataPoint);
                        return existing;
                    }

                    Tuple<T, TimeSpan> oldestTimeStampedLerpValue = existing.Item2[OLD_INDEX];
                    if(oldestTimeStampedLerpValue.Item2 < snapshot)
                    {
                        existing.Item2[OLD_INDEX] = newLerpDataPoint;
                        return existing;
                    }
                    return existing;
                }
            });
        }
    }
}
