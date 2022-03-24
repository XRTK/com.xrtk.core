// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using System;
using System.Diagnostics;
using XRTK.Extensions;
using Debug = UnityEngine.Debug;

namespace XRTK.Tests.Extensions
{
    public class EnumExtensionTests
    {
        [Flags]
        private enum TestEnumFlags
        {
            None = 0,
            First = 1,
            Second = 2,
            Third = 4,
            Fourth = 8,
            Fifth = 16
        }

        private static bool HasFlag(int value, int flag)
        {
            return (value & flag) == flag;
        }

        private static int SetFlag(int value, int flag)
        {
            return value | flag;
        }

        private static int UnsetFlag(int value, int flag)
        {
            return value & ~flag;
        }

        private static int ToggleFlag(int value, int flag)
        {
            return value ^ flag;
        }

        private static bool HasFlag<T>(T value, T flag) where T : Enum
        {
            return ((int)(object)value & (int)(object)flag) == (int)(object)flag;
        }

        private static T SetFlag<T>(T value, T flag) where T : Enum
        {
            return (T)(object)((int)(object)value | (int)(object)flag);
        }

        private static T UnsetFlag<T>(T value, T flag) where T : Enum
        {
            return (T)(object)((int)(object)value & ~(int)(object)flag);
        }

        private static T ToggleFlag<T>(T value, T flag) where T : Enum
        {
            return (T)(object)((int)(object)value ^ (int)(object)flag);
        }

        [Test]
        public void Test_01_HasFlags()
        {
            var testFlags = TestEnumFlags.First | TestEnumFlags.Third | TestEnumFlags.Fifth;

            Assert.IsTrue(testFlags.HasFlags(TestEnumFlags.First));
            Assert.IsTrue(HasFlag((int)testFlags, (int)TestEnumFlags.First));
            Assert.IsFalse(testFlags.HasFlags(TestEnumFlags.Second));
            Assert.IsFalse(HasFlag((int)testFlags, (int)TestEnumFlags.Second));
            Assert.IsTrue(testFlags.HasFlags(TestEnumFlags.Third));
            Assert.IsTrue(HasFlag((int)testFlags, (int)TestEnumFlags.Third));
            Assert.IsFalse(testFlags.HasFlags(TestEnumFlags.Fourth));
            Assert.IsFalse(HasFlag((int)testFlags, (int)TestEnumFlags.Fourth));
            Assert.IsTrue(testFlags.HasFlags(TestEnumFlags.Fifth));
            Assert.IsTrue(HasFlag((int)testFlags, (int)TestEnumFlags.Fifth));
            Assert.IsTrue(testFlags.HasFlags(TestEnumFlags.First | TestEnumFlags.Third | TestEnumFlags.Fifth));
            Assert.IsTrue(HasFlag((int)testFlags, (int)(TestEnumFlags.First | TestEnumFlags.Third | TestEnumFlags.Fifth)));
            Assert.IsFalse(testFlags.HasFlags(TestEnumFlags.Second | TestEnumFlags.Fourth));
            Assert.IsFalse(HasFlag((int)testFlags, (int)(TestEnumFlags.Second | TestEnumFlags.Fourth)));
        }

        [Test]
        public void Test_02_SetFlags()
        {
            var testFlags1 = TestEnumFlags.None;
            var testFlags2 = TestEnumFlags.None;

            Assert.IsFalse(testFlags1.HasFlags(TestEnumFlags.Third));
            Assert.IsFalse(HasFlag((int)testFlags2, (int)TestEnumFlags.Third));

            testFlags1 = testFlags1.SetFlags(TestEnumFlags.Third);
            testFlags2 = (TestEnumFlags)SetFlag((int)testFlags2, (int)TestEnumFlags.Third);

            Assert.IsTrue(testFlags1 == testFlags2);
            Assert.IsTrue(testFlags1.HasFlags(TestEnumFlags.Third));
            Assert.IsTrue(HasFlag((int)testFlags2, (int)TestEnumFlags.Third));

            Assert.IsFalse(testFlags1.HasFlags(TestEnumFlags.Fourth));
            Assert.IsFalse(HasFlag((int)testFlags2, (int)TestEnumFlags.Fourth));

            testFlags1 = testFlags1.SetFlags(TestEnumFlags.Fourth);
            testFlags2 = (TestEnumFlags)SetFlag((int)testFlags2, (int)TestEnumFlags.Fourth);

            Assert.IsTrue(testFlags1 == testFlags2);
            Assert.IsTrue(testFlags1.HasFlags(TestEnumFlags.Fourth));
            Assert.IsTrue(HasFlag((int)testFlags2, (int)TestEnumFlags.Fourth));

            Assert.IsFalse(testFlags1.HasFlags(TestEnumFlags.First | TestEnumFlags.Second | TestEnumFlags.Fifth));
            Assert.IsFalse(HasFlag((int)testFlags2, (int)(TestEnumFlags.First | TestEnumFlags.Second | TestEnumFlags.Fifth)));

            testFlags1 = testFlags1.SetFlags(TestEnumFlags.First | TestEnumFlags.Second | TestEnumFlags.Fifth);
            testFlags2 = (TestEnumFlags)SetFlag((int)testFlags2, (int)(TestEnumFlags.First | TestEnumFlags.Second | TestEnumFlags.Fifth));

            Assert.IsTrue(testFlags1 == testFlags2);
            Assert.IsTrue(testFlags1.HasFlags(TestEnumFlags.First | TestEnumFlags.Second | TestEnumFlags.Fifth));
            Assert.IsTrue(HasFlag((int)testFlags2, (int)(TestEnumFlags.First | TestEnumFlags.Second | TestEnumFlags.Fifth)));
        }

        [Test]
        public void Test_03_UnsetFlags()
        {
            var testFlags1 = ~(TestEnumFlags)0;
            var testFlags2 = ~(TestEnumFlags)0;

            Assert.IsTrue(testFlags1.HasFlags(TestEnumFlags.Third));
            Assert.IsTrue(HasFlag((int)testFlags2, (int)TestEnumFlags.Third));

            testFlags1 = testFlags1.UnsetFlags(TestEnumFlags.Third);
            testFlags2 = (TestEnumFlags)UnsetFlag((int)testFlags2, (int)TestEnumFlags.Third);

            Assert.IsTrue(testFlags1 == testFlags2);
            Assert.IsFalse(testFlags1.HasFlags(TestEnumFlags.Third));
            Assert.IsFalse(HasFlag((int)testFlags2, (int)TestEnumFlags.Third));

            Assert.IsTrue(testFlags1 == testFlags1.UnsetFlags(TestEnumFlags.Third));
            Assert.IsTrue(testFlags2 == (TestEnumFlags)UnsetFlag((int)testFlags2, (int)TestEnumFlags.Third));

            Assert.IsTrue(testFlags1.HasFlags(TestEnumFlags.Second | TestEnumFlags.Fourth));
            Assert.IsTrue(HasFlag((int)testFlags2, (int)(TestEnumFlags.Second | TestEnumFlags.Fourth)));

            testFlags1 = testFlags1.UnsetFlags(TestEnumFlags.Second | TestEnumFlags.Fourth);
            testFlags2 = (TestEnumFlags)UnsetFlag((int)testFlags2, (int)(TestEnumFlags.Second | TestEnumFlags.Fourth));

            Assert.IsTrue(testFlags1 == testFlags2);
            Assert.IsFalse(testFlags1.HasFlags(TestEnumFlags.Second | TestEnumFlags.Fourth));
            Assert.IsFalse(HasFlag((int)testFlags2, (int)(TestEnumFlags.Second | TestEnumFlags.Fourth)));
        }

        [Test]
        public void Test_04_ToggleFlags()
        {
            var testFlags1 = TestEnumFlags.Fourth | TestEnumFlags.First | TestEnumFlags.Third | TestEnumFlags.Second;
            var testFlags2 = TestEnumFlags.Fourth | TestEnumFlags.First | TestEnumFlags.Third | TestEnumFlags.Second;

            Assert.IsTrue(testFlags1 == testFlags2);
            Assert.IsFalse(testFlags1.HasFlags(TestEnumFlags.Fifth));
            Assert.IsFalse(HasFlag((int)testFlags2, (int)TestEnumFlags.Fifth));

            testFlags1 = testFlags1.ToggleFlags(TestEnumFlags.Fifth);
            testFlags2 = (TestEnumFlags)ToggleFlag((int)testFlags2, (int)TestEnumFlags.Fifth);

            Assert.IsTrue(testFlags1 == testFlags2);
            Assert.IsTrue(testFlags1.HasFlags(TestEnumFlags.Fifth));
            Assert.IsTrue(HasFlag((int)testFlags2, (int)TestEnumFlags.Fifth));

            Assert.IsTrue(testFlags1.HasFlags(TestEnumFlags.Third));
            Assert.IsTrue(HasFlag((int)testFlags2, (int)TestEnumFlags.Third));

            testFlags1 = testFlags1.ToggleFlags(TestEnumFlags.Third);
            testFlags2 = (TestEnumFlags)ToggleFlag((int)testFlags2, (int)TestEnumFlags.Third);

            Assert.IsTrue(testFlags1 == testFlags2);
            Assert.IsFalse(testFlags1.HasFlags(TestEnumFlags.Third));
            Assert.IsFalse(HasFlag((int)testFlags2, (int)TestEnumFlags.Third));

            Assert.IsTrue(testFlags1.HasFlags(TestEnumFlags.Second | TestEnumFlags.Fourth));
            Assert.IsTrue(HasFlag((int)testFlags2, (int)(TestEnumFlags.Second | TestEnumFlags.Fourth)));

            testFlags1 = testFlags1.ToggleFlags(TestEnumFlags.Second | TestEnumFlags.Fourth);
            testFlags2 = (TestEnumFlags)ToggleFlag((int)testFlags2, (int)(TestEnumFlags.Second | TestEnumFlags.Fourth));

            Assert.IsTrue(testFlags1 == testFlags2);
            Assert.IsFalse(testFlags1.HasFlags(TestEnumFlags.Second | TestEnumFlags.Fourth));
            Assert.IsFalse(HasFlag((int)testFlags2, (int)(TestEnumFlags.Second | TestEnumFlags.Fourth)));
        }

        [Test]
        public void Test_05_SpeedTests()
        {
            var testFlags1 = TestEnumFlags.None;
            var testFlags2 = TestEnumFlags.None;
            var testFlags3 = TestEnumFlags.None;
            var iterations = 10000000;

            var watch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                testFlags1 = ToggleFlag(testFlags1, TestEnumFlags.First | TestEnumFlags.Second | TestEnumFlags.Third | TestEnumFlags.Fifth);
            }

            watch.Stop();
            Debug.Log($"Test ToggleFlag Boxed execution Time: {watch.Elapsed.TotalMilliseconds} ms");

            watch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                testFlags2 = testFlags2.ToggleFlags(TestEnumFlags.First | TestEnumFlags.Second | TestEnumFlags.Third | TestEnumFlags.Fifth);
            }

            watch.Stop();
            Debug.Log($"Test ToggleFlag Extension execution Time: {watch.Elapsed.TotalMilliseconds} ms");

            watch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                testFlags3 = (TestEnumFlags)ToggleFlag((int)testFlags3, (int)(TestEnumFlags.First | TestEnumFlags.Second | TestEnumFlags.Third | TestEnumFlags.Fifth));
            }

            watch.Stop();
            Debug.Log($"Test ToggleFlag int execution Time: {watch.Elapsed.TotalMilliseconds} ms");
        }
    }
}
