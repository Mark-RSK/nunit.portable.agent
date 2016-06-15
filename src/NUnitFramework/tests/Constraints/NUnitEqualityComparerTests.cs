﻿// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.TestUtilities;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class EqualityComparerTests
    {
        private Tolerance tolerance;
        private NUnitEqualityComparer comparer;

        [SetUp]
        public void Setup()
        {
            tolerance = Tolerance.Default;
            comparer = new NUnitEqualityComparer();
        }

        [TestCase(4, 4)]
        [TestCase(4.0d, 4.0d)]
        [TestCase(4.0f, 4.0f)]
        [TestCase(4, 4.0d)]
        [TestCase(4, 4.0f)]
        [TestCase(4.0d, 4)]
        [TestCase(4.0d, 4.0f)]
        [TestCase(4.0f, 4)]
        [TestCase(4.0f, 4.0d)]
        [TestCase(SpecialValue.Null, SpecialValue.Null)]
        [TestCase(null, null)]
        public void EqualItems(object x, object y)
        {
            Assert.That(comparer.AreEqual(x, y, ref tolerance));
        }

        [TestCase(4, 2)]
        [TestCase(4.0d, 2.0d)]
        [TestCase(4.0f, 2.0f)]
        [TestCase(4, 2.0d)]
        [TestCase(4, 2.0f)]
        [TestCase(4.0d, 2)]
        [TestCase(4.0d, 2.0f)]
        [TestCase(4.0f, 2)]
        [TestCase(4.0f, 2.0d)]
        [TestCase(4, SpecialValue.Null)]
        [TestCase(4, null)]
        public void UnequalItems(object greater, object lesser)
        {
            Assert.False(comparer.AreEqual(greater, lesser, ref tolerance));
            Assert.False(comparer.AreEqual(lesser, greater, ref tolerance));
        }

        [TestCase(double.PositiveInfinity, double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity, double.NegativeInfinity)]
        [TestCase(double.NaN, double.NaN)]
        [TestCase(float.PositiveInfinity, float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity, float.NegativeInfinity)]
        [TestCase(float.NaN, float.NaN)]
        public void SpecialFloatingPointValuesCompareAsEqual(object x, object y)
        {
            Assert.That(comparer.AreEqual(x, y, ref tolerance));
        }

#if !PORTABLE
        [Test]
        public void SameDirectoriesAreEqual()
        {
            using (var testDir = new TestDirectory())
            {
                var one = new DirectoryInfo(testDir.Directory.FullName);
                var two = new DirectoryInfo(testDir.Directory.FullName);
                Assert.That(comparer.AreEqual(one, two, ref tolerance));
            }
        }

        [Test]
        public void DifferentDirectoriesAreNotEqual()
        {
            using (var one = new TestDirectory())
            using (var two = new TestDirectory())
            {
                Assert.That(comparer.AreEqual(one, two, ref tolerance), Is.False);
            }
        }
#endif

        [Test]
        public void CanCompareArrayContainingSelfToSelf()
        {
            object[] array = new object[1];
            array[0] = array;

            Assert.True(comparer.AreEqual(array, array, ref tolerance));
        }

        [Test]
        public void IEquatableSuccess()
        {
            IEquatableWithoutEqualsOverridden x = new IEquatableWithoutEqualsOverridden(1);
            IEquatableWithoutEqualsOverridden y = new IEquatableWithoutEqualsOverridden(1);

            Assert.IsTrue(comparer.AreEqual(x, y, ref tolerance));
        }

        [Test]
        public void IEquatableDifferentTypesSuccess_WhenActualImplementsIEquatable()
        {
            int x = 1;
            Int32IEquatable y = new Int32IEquatable(1);

            // y.Equals(x) is what gets actually called
            // TODO: This should work both ways
            Assert.IsTrue(comparer.AreEqual(x, y, ref tolerance));
        }

        [Test]
        public void IEquatableDifferentTypesSuccess_WhenExpectedImplementsIEquatable()
        {
            int x = 1;
            Int32IEquatable y = new Int32IEquatable(1);

            // y.Equals(x) is what gets actually called
            // TODO: This should work both ways
            Assert.IsTrue(comparer.AreEqual(y, x, ref tolerance));
        }

        [Test]
        public void IEquatableSuccess_WhenExplicitInterfaceImplementation()
        {
            IEquatableWithExplicitImplementation x = new IEquatableWithExplicitImplementation(1);
            IEquatableWithExplicitImplementation y = new IEquatableWithExplicitImplementation(1);

            Assert.IsTrue(comparer.AreEqual(x, y, ref tolerance));
        }

        [Test]
        public void IEquatableFailure_WhenExplicitInterfaceImplementationWithDifferentValues()
        {
            IEquatableWithExplicitImplementation x = new IEquatableWithExplicitImplementation(1);
            IEquatableWithExplicitImplementation y = new IEquatableWithExplicitImplementation(2);

            Assert.IsFalse(comparer.AreEqual(x, y, ref tolerance));
        }

        [Test]
        public void ReferenceEqualityHasPrecedenceOverIEquatable()
        {
            NeverEqualIEquatable z = new NeverEqualIEquatable();

            Assert.IsTrue(comparer.AreEqual(z, z, ref tolerance));
        }

        [Test]
        public void IEquatableHasPrecedenceOverDefaultEquals()
        {
            NeverEqualIEquatableWithOverriddenAlwaysTrueEquals x = new NeverEqualIEquatableWithOverriddenAlwaysTrueEquals();
            NeverEqualIEquatableWithOverriddenAlwaysTrueEquals y = new NeverEqualIEquatableWithOverriddenAlwaysTrueEquals();

            Assert.IsFalse(comparer.AreEqual(x, y, ref tolerance));
        }

        [Test]
        public void ImplementingIEquatableDirectlyOnTheClass()
        {
            var obj1 = new EquatableObject { SomeProperty = 1 };
            var obj2 = new EquatableObject { SomeProperty = 1 };

            var n = new NUnitEqualityComparer();
            var tolerance = Tolerance.Exact;
            Assert.That(n.AreEqual(obj1, obj2, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj2, obj1, ref tolerance), Is.True);
        }

        [Test]
        public void ImplementingIEquatableOnABaseClassOrInterface()
        {
            var obj1 = new InheritedEquatableObject { SomeProperty = 1 };
            var obj2 = new InheritedEquatableObject { SomeProperty = 1 };

            var n = new NUnitEqualityComparer();
            var tolerance = Tolerance.Exact;
            Assert.That(n.AreEqual(obj1, obj2, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj2, obj1, ref tolerance), Is.True);
        }

        [Test]
        public void ImplementingIEquatableOnABaseClassOrInterfaceThroughInterface()
        {
            IEquatableObject obj1 = new InheritedEquatableObject { SomeProperty = 1 };
            IEquatableObject obj2 = new InheritedEquatableObject { SomeProperty = 1 };

            var n = new NUnitEqualityComparer();
            var tolerance = Tolerance.Exact;
            Assert.That(n.AreEqual(obj1, obj2, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj2, obj1, ref tolerance), Is.True);
        }

        [Test]
        public void CanHandleMultipleImplementationsOfIEquatable()
        {
            IEquatableObject obj1 = new InheritedEquatableObject { SomeProperty = 1 };
            IEquatableObject obj2 = new MultipleIEquatables { SomeProperty = 1 };
            var obj3 = new EquatableObject { SomeProperty = 1 };

            var n = new NUnitEqualityComparer();
            var tolerance = Tolerance.Exact;
            Assert.That(n.AreEqual(obj1, obj2, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj2, obj1, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj1, obj3, ref tolerance), Is.False);
            Assert.That(n.AreEqual(obj3, obj1, ref tolerance), Is.False);
            Assert.That(n.AreEqual(obj2, obj3, ref tolerance), Is.True);
            Assert.That(n.AreEqual(obj3, obj2, ref tolerance), Is.True);
        }

        [Test]
        public void IEnumeratorIsDisposed()
        {
            var enumeration = new EnumerableWithDisposeChecks<int>(new[] { 0, 1, 2, 3 });
            Assert.True(comparer.AreEqual(enumeration, enumeration, ref tolerance));
            Assert.That(enumeration.EnumeratorsDisposed);
        }
    }

    internal class EnumerableWithDisposeChecks<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> data;
        private readonly List<DisposableEnumerator<T>> enumerators = new List<DisposableEnumerator<T>>();

        public EnumerableWithDisposeChecks(T[] data)
        {
            this.data = data;
        }

        public bool EnumeratorsDisposed
        {
            get
            {
                foreach (var disposableEnumerator in enumerators)
                {
                    if (!disposableEnumerator.Disposed)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var enumerator = new DisposableEnumerator<T>(data.GetEnumerator());
            enumerators.Add(enumerator);
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }

    internal class DisposableEnumerator<T> : IEnumerator<T>
    {
        private bool disposedValue = false;
        private readonly IEnumerator<T> enumerator;

        public DisposableEnumerator(IEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
        }

        public bool Disposed { get { return disposedValue; } }

        public T Current
        {
            get
            {
                return enumerator.Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return enumerator.Current;
            }
        }

        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }

        public void Reset()
        {
            enumerator.Reset();
        }

        public void Dispose()
        {
            disposedValue = true;
        }

    }

    public class NeverEqualIEquatableWithOverriddenAlwaysTrueEquals : IEquatable<NeverEqualIEquatableWithOverriddenAlwaysTrueEquals>
    {
        public bool Equals(NeverEqualIEquatableWithOverriddenAlwaysTrueEquals other)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Int32IEquatable : IEquatable<int>
    {
        private readonly int value;

        public Int32IEquatable(int value)
        {
            this.value = value;
        }

        public bool Equals(int other)
        {
            return value.Equals(other);
        }
    }

    public class NeverEqualIEquatable : IEquatable<NeverEqualIEquatable>
    {
        public bool Equals(NeverEqualIEquatable other)
        {
            return false;
        }
    }

    public class IEquatableWithoutEqualsOverridden : IEquatable<IEquatableWithoutEqualsOverridden>
    {
        private readonly int value;

        public IEquatableWithoutEqualsOverridden(int value)
        {
            this.value = value;
        }

        public bool Equals(IEquatableWithoutEqualsOverridden other)
        {
            return value.Equals(other.value);
        }
    }

    public class IEquatableWithExplicitImplementation : IEquatable<IEquatableWithExplicitImplementation>
    {
        private readonly int value;

        public IEquatableWithExplicitImplementation(int value)
        {
            this.value = value;
        }

        bool IEquatable<IEquatableWithExplicitImplementation>.Equals(IEquatableWithExplicitImplementation other)
        {
            return value.Equals(other.value);
        }
    }

    public class EquatableObject : IEquatable<EquatableObject>
    {
        public int SomeProperty { get; set; }
        public bool Equals(EquatableObject other)
        {
            if (other == null)
                return false;

            return SomeProperty == other.SomeProperty;
        }
    }

    public interface IEquatableObject : IEquatable<IEquatableObject>
    {
        int SomeProperty { get; set; }
    }

    public class InheritedEquatableObject : IEquatableObject
    {
        public int SomeProperty { get; set; }

        public bool Equals(IEquatableObject other)
        {
            if (other == null)
                return false;

            return SomeProperty == other.SomeProperty;
        }
    }

    public class MultipleIEquatables : InheritedEquatableObject, IEquatable<EquatableObject>
    {
        public bool Equals(EquatableObject other)
        {
            if (other == null)
                return false;

            return SomeProperty == other.SomeProperty;
        }
    }
}
