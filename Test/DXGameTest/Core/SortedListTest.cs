using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core;
using NUnit.Framework;

namespace DXGameTest.Core
{
    public class SortedListTest
    {
        [Test]
        public void TestDefaultConstructor()
        {
            SortedList<int> intList = new SortedList<int>();
            Assert.NotNull(intList);
            Assert.AreEqual(0, intList.Count);
            Random rGen = new Random();
            for (int i = 0; i < 100; ++i)
            {
                int arbitraryValue = rGen.Next();
                Assert.IsFalse(intList.Contains(arbitraryValue));
            }
            Assert.False(intList.IsReadOnly);
        }

        [Test]
        public void TestInitializeWithCapacity()
        {
            SortedList<int> intList = new SortedList<int>(10);
            Assert.NotNull(intList);
            Assert.AreEqual(0, intList.Count);
            Random rGen = new Random();
            for (int i = 0; i < 100; ++i)
            {
                int arbitraryValue = rGen.Next();
                Assert.IsFalse(intList.Contains(arbitraryValue));
            }
            Assert.False(intList.IsReadOnly);
        }

        [Test]
        public void TestCopyConstructor()
        {
            List<int> originalList = new List<int>();
            Random rGen = new Random();
            int numElements = 2000;
            for (int i = 0; i < numElements; ++i)
            {
                int arbitraryValue = rGen.Next();
                originalList.Add(arbitraryValue);
            }

            SortedList<int> sortedList = new SortedList<int>(originalList);
            Assert.NotNull(sortedList);
            Assert.AreEqual(numElements, sortedList.Count);
            foreach (int value in originalList)
            {
                Assert.IsTrue(sortedList.Contains(value));
            }

            Assert.True(IsSorted(sortedList));
            Assert.False(sortedList.IsReadOnly);
        }

        [Test]
        public void TestClear()
        {
            int numElements = 20000;
            SortedList<int> sortedList = GenerateRandomSortedList(numElements);
            List<int> originalValues = new SortedList<int>(sortedList).ToList();

            Assert.AreEqual(numElements, sortedList.Count);
            Assert.AreEqual(numElements, originalValues.Count);

            sortedList.Clear();
            Assert.AreEqual(0, sortedList.Count);
            foreach (int value in originalValues)
            {
                Assert.False(sortedList.Contains(value));
            }

            foreach (int value in sortedList)
            {
                Assert.Fail("There should not be any values in the sortedList after clear is called");
            }
        }

        [Test]
        public void TestAdd()
        {
            int numElements = 2000;
            SortedList<int> sortedList = new SortedList<int>(numElements);
            Random rGen = new Random();
            for (int i = 0; i < numElements; ++i)
            {
                int arbitraryValue = rGen.Next();
                sortedList.Add(arbitraryValue);
                Assert.IsTrue(IsSorted(sortedList));
            }
        }

        [Test]
        public void TestAddMultiSameValue()
        {
            int numCopies = 3;
            int numElements = 20000;
            Random rGen = new Random();
            SortedList<int> sortedList = new SortedList<int>(numElements*numCopies);
            Assert.AreEqual(0, sortedList.Count);
            for (int i = 0; i < numElements; ++i)
            {
                int value = rGen.Next();
                for (int j = 0; j < numCopies; ++j)
                {
                    sortedList.Add(value);
                }
            }

            Assert.AreEqual(numCopies*numElements, sortedList.Count);
            Assert.IsTrue(IsSorted(sortedList));
        }

        [Test]
        public void TestContains()
        {
            int numElements = 20000;
            SortedList<int> sortedList = new SortedList<int>(numElements);
            HashSet<int> uniqueValues = new HashSet<int>(sortedList);
            Random rGen = new Random();
            foreach (int value in sortedList)
            {
                Assert.IsTrue(sortedList.Contains(value));
            }

            /*
                Generate n elements not already in the set, make sure Contains doesn't return true
            */
            for (int i = 0; i < numElements; ++i)
            {
                int value;
                do
                {
                    value = rGen.Next();
                } while (!uniqueValues.Add(value));

                Assert.IsFalse(sortedList.Contains(value));
            }
        }

        [Test]
        public void TestRemove()
        {
            int numElements = 20000;
            SortedList<int> sortedList = GenerateRandomSortedList(numElements);
            Random rGen = new Random();
            do
            {
                int originalCount = sortedList.Count;
                int index = rGen.Next(originalCount);
                int value = sortedList[index];
                Assert.True(sortedList.Contains(value));
                bool removedOk = sortedList.Remove(value);
                Assert.True(removedOk);
                Assert.True(IsSorted(sortedList));
                Assert.AreEqual(originalCount - 1, sortedList.Count);
            } while (sortedList.Any());
            Assert.AreEqual(0, sortedList.Count);

            // Make sure count & enumeration aren't out of sync
            foreach (int value in sortedList)
            {
                Assert.Fail("SortedList should not have any items in them after removing them all");
            }
        }

        [Test]
        public void TestIndex()
        {
            SortedList<int> sortedList = new SortedList<int>();
            sortedList.Add(1);
            sortedList.Add(5);
            sortedList.Add(3);

            Assert.AreEqual(1, sortedList[0]);
            Assert.AreEqual(3, sortedList[1]);
            Assert.AreEqual(5, sortedList[2]);
        }

        private static bool IsSorted<T>(IEnumerable<T> collection) where T : IComparable<T>
        {
            bool sorted = true;
            var enumerator = collection.GetEnumerator();
            T previous = enumerator.Current;
            while (enumerator.MoveNext())
            {
                T value = enumerator.Current;
                sorted &= (value.CompareTo(previous) >= 0);
                previous = value;
            }
            return sorted;
        }

        private static SortedList<int> GenerateRandomSortedList(int numElements)
        {
            Assert.IsTrue(numElements > 0);
            Random rGen = new Random();
            SortedList<int> sortedList = new SortedList<int>(numElements);
            for (int i = 0; i < numElements; ++i)
            {
                int arbitraryValue = rGen.Next();
                sortedList.Add(arbitraryValue);
            }

            return sortedList;
        }
    }
}