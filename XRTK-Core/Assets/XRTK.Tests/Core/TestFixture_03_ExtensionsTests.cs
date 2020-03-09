// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using UnityEngine;
using XRTK.Extensions;

namespace XRTK.Tests.Core
{
    public class TestFixture_03_ExtensionsTests
    {
        [Test]
        public void Test_01_Array_AddItem_Extension()
        {
            Vector2[] testArray = null;
            Vector2[] newArray =  null;
            try
            {
                newArray = testArray.AddItem(Vector2.zero);
            }
            catch { }

            Assert.IsNotNull(newArray);

            testArray = new Vector2[5];
            newArray = testArray.AddItem(Vector2.one);

            Assert.IsTrue(newArray[newArray.Length - 1] == Vector2.one);
            Assert.IsTrue(newArray.Length > testArray.Length);
            Assert.IsTrue(newArray.Length - testArray.Length == 1);
        }

        [Test]
        public void Test_02_Array_InitialiseArray_Extension()
        {
            Vector2[] testArray = null;
            Vector2[] newArray = null;
            try
            {
                newArray = testArray.InitialiseArray(Vector2.one);
            }
            catch { }

            Assert.IsNotNull(newArray);
            Assert.IsTrue(newArray.Length == 1);
            Assert.IsTrue(newArray[0] == Vector2.one);

            testArray = new Vector2[5];
            for (int i = 0; i < testArray.Length; i++)
            {
                testArray[i] = Vector2.one;
            }
            newArray = testArray.InitialiseArray(Vector2.zero);

            Assert.IsTrue(newArray[0] == Vector2.zero);
        }
    }
}