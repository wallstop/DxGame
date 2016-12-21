using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace DxCore.Core.Utils.Cache.Advanced
{
    internal sealed class FatTimer : IDisposable
    {
        public Timer ReadTimer;
        public Timer WriteTimer;

        public void Dispose()
        {
            try
            {
                ReadTimer?.Dispose();
            }
            catch
            {
                // This page left blank
            }
            try
            {
                WriteTimer?.Dispose();
            }
            catch
            {
                // This one too
            }
        }
    }

    internal enum CacheAction
    {
        Read,
        Write
    }

    [Serializable]
    [DataContract]
    internal sealed class Node<K, V>
    {
        [DataMember] public V Data;
        [DataMember] public K Key;
        [DataMember] public Node<K, V> Next;
        [DataMember] public Node<K, V> Previous;

        public override string ToString() => this.ToJson();
    }

    public class LocalCache<K, V> : ICache<K, V>
    {
        public long ExpireAfterAccessMilliseconds { get; }
        public long ExpireAfterWriteMilliseconds { get; }

        public bool ExpiresAfterAccess => ExpireAfterAccessMilliseconds > 0;
        public bool ExpiresAfterWrite => ExpireAfterWriteMilliseconds > 0;

        public bool HasRemovalListener => !ReferenceEquals(RemovalListener, null);
        public bool IsCapped => MaxElements > 0;
        public long MaxElements { get; }
        private Dictionary<K, V> Cache { get; }

        private Node<K, V> Head { get; }
        private ReaderWriterLockSlim Lock { get; }

        private Dictionary<K, Node<K, V>> LruCache { get; }
        private Action<RemovalNotification<K, V>> RemovalListener { get; }
        private Node<K, V> Tail { get; }
        private Dictionary<K, FatTimer> Timers { get; }

        public LocalCache(CacheBuilder<K, V> cacheBuilder)
        {
            Validate.Validate.Hard.IsNotNull(cacheBuilder,
                () => this.GetFormattedNullOrDefaultMessage(nameof(cacheBuilder)));

            const int invalid = -1;

            ExpireAfterAccessMilliseconds = cacheBuilder.ExpireAfterAccessMilliseconds.GetValueOrDefault(invalid);
            ExpireAfterWriteMilliseconds = cacheBuilder.ExpireAfterWriteMilliseconds.GetValueOrDefault(invalid);
            MaxElements = cacheBuilder.MaxElements.GetValueOrDefault(invalid);

            Lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            if(IsCapped)
            {
                LruCache = new Dictionary<K, Node<K, V>>();
            }
            else
            {
                Cache = new Dictionary<K, V>();
            }

            Timers = new Dictionary<K, FatTimer>();

            Head = new Node<K, V>();
            Tail = new Node<K, V>();
            Head.Next = Tail;
            Tail.Previous = Head;

            RemovalListener = cacheBuilder.RemovalListener;
        }

        public bool GetIfPresent(K key, out V value)
        {
            using(new CriticalRegion(Lock, CriticalRegion.LockType.Read))
            {
                CheckTimer(key, CacheAction.Read);
                if(IsCapped)
                {
                    Node<K, V> valueWrapper;
                    bool present = LruCache.TryGetValue(key, out valueWrapper);
                    ExtractValue(valueWrapper, present, out value);
                    if(present)
                    {
                        AddNode(valueWrapper);
                    }
                    return present;
                }
                return Cache.TryGetValue(key, out value);
            }
        }

        public V Get(K key, Func<K, V> valueLoader)
        {
            /* Get it if its there */
            using(new CriticalRegion(Lock, CriticalRegion.LockType.Read))
            {
                CheckTimer(key, CacheAction.Read);
                if(IsCapped)
                {
                    Node<K, V> valueWrapper;
                    if(LruCache.TryGetValue(key, out valueWrapper))
                    {
                        V existingValue;
                        ExtractValue(valueWrapper, true, out existingValue);
                        AddNode(valueWrapper);
                        return existingValue;
                    }
                }
                else
                {
                    V value;
                    if(Cache.TryGetValue(key, out value))
                    {
                        return value;
                    }
                }
            }

            /* 
                Get exclusive access. Due to some potential missed atomicity, we need to 
                check if someone else has grabbed the write lock first. If so,
                we don't want to invoke the value creator, and simply return the existing value
            */
            using(new CriticalRegion(Lock, CriticalRegion.LockType.Write))
            {
                CheckTimer(key, CacheAction.Write, true);
                CheckTimer(key, CacheAction.Read);

                if(IsCapped)
                {
                    Node<K, V> valueWrapper;
                    if(LruCache.TryGetValue(key, out valueWrapper))
                    {
                        V existingValue;
                        ExtractValue(valueWrapper, true, out existingValue);
                        return existingValue;
                    }
                    V createdValue = valueLoader(key);
                    valueWrapper = new Node<K, V> {Key = key, Data = createdValue};
                    AddNode(valueWrapper);
                    LruCache.Add(key, valueWrapper);
                    LruEvict();
                    return createdValue;
                }
                else
                {
                    V existingValue;
                    if(Cache.TryGetValue(key, out existingValue))
                    {
                        return existingValue;
                    }
                    V createdValue = valueLoader(key);
                    Cache.Add(key, createdValue);
                    return createdValue;
                }
            }
        }

        public void Put(K key, V value)
        {
            V existingValue;
            bool replaced = false;
            using(new CriticalRegion(Lock, CriticalRegion.LockType.Write))
            {
                CheckTimer(key, CacheAction.Write, true);
                if(IsCapped)
                {
                    Node<K, V> valueWrapper;
                    bool existed = false;
                    replaced = LruCache.TryGetValue(key, out valueWrapper) &&
                               !(existed = Objects.Equals(valueWrapper.Data, value));
                    if(replaced)
                    {
                        existingValue = valueWrapper.Data;
                        valueWrapper.Data = value;
                        AddNode(valueWrapper);
                    }
                    else
                    {
                        existingValue = default(V);
                        valueWrapper = new Node<K, V> {Key = key, Data = value};
                        if(!existed)
                        {
                            LruCache.Add(key, valueWrapper);
                        }
                        AddNode(valueWrapper);
                        LruEvict();
                    }
                }
                else
                {
                    replaced = Cache.TryGetValue(key, out existingValue) && !Objects.Equals(existingValue, value);
                    Cache[key] = value;
                }
            }
            if(replaced)
            {
                InternalNotifyOfRemoval(ref key, ref existingValue, RemovalCause.Replaced);
            }
        }

        public void Invalidate(K key)
        {
            Removal(key, () => false, RemovalCause.Explicit);
        }

        public void InvalidateAll()
        {
            K[] cacheKeys;
            using(new CriticalRegion(Lock, CriticalRegion.LockType.Read))
            {
                cacheKeys = IsCapped ? LruCache.Keys.ToArray() : Cache.Keys.ToArray();
            }
            foreach(K cacheKey in cacheKeys)
            {
                Removal(cacheKey, () => false, RemovalCause.Explicit);
            }
        }

        public long Count
        {
            get
            {
                using(new CriticalRegion(Lock, CriticalRegion.LockType.Read))
                {
                    return IsCapped ? LruCache.Count : Cache.Count;
                }
            }
        }

        private void AddNode(Node<K, V> valueWrapper)
        {
            if(valueWrapper == Tail.Previous)
            {
                return;
            }

            /* Already a member of our list? We need some maintenance! */
            if(!ReferenceEquals(valueWrapper.Next, null) && !ReferenceEquals(valueWrapper.Previous, null))
            {
                valueWrapper.Previous.Next = valueWrapper.Next;
                valueWrapper.Next.Previous = valueWrapper.Previous;
                /* Sanity's sake */
                valueWrapper.Next = null;
                valueWrapper.Previous = null;
            }

            Tail.Previous.Next = valueWrapper;
            valueWrapper.Previous = Tail.Previous;
            Tail.Previous = valueWrapper;
            valueWrapper.Next = Tail;
        }

        private void CheckTimer(K key, CacheAction cacheAction, bool forceCreation = false)
        {
            FatTimer existingTimer;
            if(!Timers.TryGetValue(key, out existingTimer))
            {
                if(forceCreation)
                {
                    existingTimer = NewFatTimer(key);
                    Timers.Add(key, existingTimer);
                }
                else
                {
                    return;
                }
            }
            switch(cacheAction)
            {
                case CacheAction.Read:
                {
                    if(!ExpiresAfterAccess)
                    {
                        return;
                    }
                    existingTimer.ReadTimer.Change(ExpireAfterAccessMilliseconds, Timeout.Infinite);
                    break;
                }
                case CacheAction.Write:
                {
                    if(!ExpiresAfterWrite)
                    {
                        return;
                    }
                    existingTimer.WriteTimer.Change(ExpireAfterWriteMilliseconds, Timeout.Infinite);
                    break;
                }
                default:
                {
                    throw new InvalidEnumArgumentException($"Unexpected {typeof(CacheAction)} {cacheAction}");
                }
            }
        }

        private void ExtractValue(Node<K, V> valueNode, bool present, out V value)
        {
            value = present ? valueNode.Data : default(V);
        }

        private void InternalNotifyOfRemoval(ref K key, ref V value, RemovalCause cause)
        {
            if(!HasRemovalListener)
            {
                return;
            }

            RemovalNotification<K, V> removalNotification = new RemovalNotification<K, V>(key, value, cause);
            RemovalListener(removalNotification);
        }

        private void LruEvict()
        {
            Validate.Validate.Hard.IsTrue(IsCapped);
            if(LruCache.Count <= MaxElements)
            {
                return;
            }

            Node<K, V> temp = Head.Next;
            Head.Next = temp.Next;
            temp.Next.Previous = Head;

            K key = temp.Key;
            LruCache.Remove(key);
            FatTimer existingTimer;
            if(Timers.TryGetValue(key, out existingTimer))
            {
                existingTimer.Dispose();
                Timers.Remove(key);
            }

            InternalNotifyOfRemoval(ref key, ref temp.Data, RemovalCause.Evicted);
        }

        private FatTimer NewFatTimer(K key)
        {
            FatTimer fatTimer = new FatTimer();
            int alreadyExpired = 0;

            Func<bool> alreadyExpiredCheck = () => Interlocked.Add(ref alreadyExpired, 1) != 1;
            if(ExpiresAfterWrite)
            {
                fatTimer.WriteTimer = new Timer(_ => { Removal(key, alreadyExpiredCheck, RemovalCause.Expired); });
            }
            if(ExpiresAfterAccess)
            {
                fatTimer.ReadTimer = new Timer(_ => { Removal(key, alreadyExpiredCheck, RemovalCause.Expired); });
            }
            return fatTimer;
        }

        private void Removal(K key, Func<bool> alreadyExpired, RemovalCause removalCause)
        {
            if(alreadyExpired())
            {
                /* Someone else already cleaned up after us :( */
                return;
            }

            using(new CriticalRegion(Lock, CriticalRegion.LockType.Write))
            {
                FatTimer existingTimer;
                if(Timers.TryGetValue(key, out existingTimer))
                {
                    existingTimer.Dispose();
                    Timers.Remove(key);
                }
                if(!HasRemovalListener)
                {
                    if(IsCapped)
                    {
                        LruCache.Remove(key);
                    }
                    else
                    {
                        Cache.Remove(key);
                    }
                    return;
                }

                bool removed = false;
                V removedValue;
                if(IsCapped)
                {
                    Node<K, V> removedValueWrapper;
                    if(removed = LruCache.TryGetValue(key, out removedValueWrapper))
                    {
                        removedValue = removedValueWrapper.Data;
                        removedValueWrapper.Previous.Next = removedValueWrapper.Next;
                        removedValueWrapper.Next.Previous = removedValueWrapper.Previous;
                        LruCache.Remove(key);
                    }
                    else
                    {
                        removedValue = default(V);
                    }
                }
                else
                {
                    if(removed = Cache.TryGetValue(key, out removedValue))
                    {
                        Cache.Remove(key);
                    }
                }

                if(removed)
                {
                    InternalNotifyOfRemoval(ref key, ref removedValue, removalCause);
                }
            }
        }
    }
}