// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using System.Diagnostics;
using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using Debug = UnityEngine.Debug;

namespace XRTK.Tests.InputSystem
{
    public class TestFixture_02_InteractionDefinitionTests
    {
        #region Utilities

        private static void Assert_NoChange_NoUpdate(MixedRealityInteractionMapping mapping)
        {
            Assert.IsFalse(mapping.ControlActivated);
            Assert.IsFalse(mapping.Updated);
        }

        private static void Assert_Change_NoUpdate(MixedRealityInteractionMapping mapping)
        {
            Assert.IsTrue(mapping.ControlActivated);
            Assert.IsFalse(mapping.Updated);
        }

        private static void Assert_NoChange_Update(MixedRealityInteractionMapping mapping)
        {
            Assert.IsFalse(mapping.ControlActivated);
            Assert.IsTrue(mapping.Updated);
        }

        private static void Assert_Change_Update(MixedRealityInteractionMapping mapping)
        {
            Assert.IsTrue(mapping.ControlActivated);
            Assert.IsTrue(mapping.Updated);
        }

        #endregion Utilities

        #region objects

        public MixedRealityInteractionMapping InitializeRawInteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.Raw, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_01_01_InitializedRawData()
        {
            var interaction = InitializeRawInteractionMapping();

            var initialValue = interaction.RawData;

            // Test to make sure the initial values are correct.
            Assert.IsNull(initialValue);
            Assert.IsFalse(interaction.ControlActivated);
            Assert.IsFalse(interaction.Updated);

            interaction.RawData = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.IsNull(interaction.RawData);
            Assert.IsFalse(interaction.ControlActivated);
            Assert.IsFalse(interaction.Updated);
        }

        /// <summary>
        /// We test by setting the interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_01_02_ObjectChangedAndUpdated()
        {
            var interaction = InitializeRawInteractionMapping();
            var initialValue = interaction.RawData;
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            interaction.RawData = testValue1;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.ControlActivated);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.RawData;

            // Check the values
            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1, testValue1);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.ControlActivated);
            Assert.IsFalse(interaction.Updated);

            interaction.RawData = testValue2;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.ControlActivated);
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.RawData;

            // Check the values
            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2, testValue2);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.ControlActivated);
            Assert.IsFalse(interaction.Updated);

            interaction.RawData = initialValue;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.ControlActivated);
            Assert.IsTrue(interaction.Updated);

            var setValue3 = interaction.RawData;

            // Check the values
            Assert.IsNull(interaction.RawData);
            Assert.AreEqual(initialValue, setValue3);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.ControlActivated);
            Assert.IsFalse(interaction.Updated);
        }

        /// <summary>
        /// We test by setting the interaction data to the same object multiple times.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true.<para/>
        /// </summary>
        [Test]
        public void Test_01_03_ObjectNoChangeAndUpdated()
        {
            var interaction = InitializeRawInteractionMapping();
            var testValue = new object();

            interaction.RawData = testValue;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.ControlActivated);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.RawData;

            // Check the values
            Assert.IsNotNull(setValue1);
            Assert.AreEqual(testValue, setValue1);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.ControlActivated);
            Assert.IsFalse(interaction.Updated);

            interaction.RawData = testValue;

            // Make sure if we set the same value changed is false
            Assert.IsFalse(interaction.ControlActivated);

            // Make sure if we set the same value updated is true
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.RawData;

            // Check the values
            Assert.IsNotNull(setValue2);
            Assert.AreEqual(testValue, setValue2);
            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.ControlActivated);
            Assert.IsFalse(interaction.Updated);
        }

        #endregion objects

        #region bools

        public MixedRealityInteractionMapping InitializeBoolInteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.Digital, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_02_01_InitializedBoolData()
        {
            var interaction = InitializeBoolInteractionMapping();
            var initialValue = interaction.BoolData;

            // Test to make sure the initial values are correct.
            Assert.IsFalse(initialValue);
            Assert_NoChange_NoUpdate(interaction);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            interaction.BoolData = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.IsFalse(initialValue);
            Assert_NoChange_NoUpdate(interaction);
        }

        /// <summary>
        /// We test by setting the interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_02_02_Bool_Changed_NoUpdate()
        {
            var interaction = InitializeBoolInteractionMapping();
            const bool testValue1 = true;
            const bool testValue2 = false;

            // Set the value
            interaction.BoolData = testValue1;
            Assert_Change_NoUpdate(interaction);

            // Check the value
            Assert.IsTrue(interaction.BoolData);
            Assert.True(interaction.BoolData == testValue1);
            Assert_NoChange_NoUpdate(interaction);

            // Set the value
            interaction.BoolData = testValue2;
            Assert_Change_NoUpdate(interaction);

            // Check the value
            Assert.IsFalse(interaction.BoolData);
            Assert.True(interaction.BoolData == testValue2);
            Assert_NoChange_NoUpdate(interaction);
        }

        /// <summary>
        /// We test by setting the interaction data to the same object multiple times.<para/>
        /// Check that the value can be changed, then subsequent identical values show no change.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_02_03_Bool_NoChange_NoUpdate()
        {
            var interaction = InitializeBoolInteractionMapping();
            const bool testValue = true;

            // Set the value
            interaction.BoolData = testValue;
            Assert_Change_NoUpdate(interaction);

            // Check the value
            Assert.IsTrue(testValue);
            Assert.True(interaction.BoolData == testValue);
            Assert_NoChange_NoUpdate(interaction);

            // Set the value
            interaction.BoolData = testValue;
            Assert_NoChange_NoUpdate(interaction);

            // Check the value
            Assert.IsTrue(testValue);
            Assert.True(interaction.BoolData == testValue);
            Assert_NoChange_NoUpdate(interaction);
        }

        #endregion bools

        #region float

        public MixedRealityInteractionMapping InitializeFloatInteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.SingleAxis, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_03_01_InitializedFloatData()
        {
            var interaction = InitializeFloatInteractionMapping();
            var initialValue = interaction.FloatData;

            // Test to make sure the initial values are correct.
            Assert.AreEqual(0d, initialValue, double.Epsilon);
            Assert_NoChange_NoUpdate(interaction);

            interaction.FloatData = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.AreEqual(0d, initialValue, double.Epsilon);
            Assert_NoChange_NoUpdate(interaction);
        }

        /// <summary>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_03_02_Float_Changed_Updated()
        {
            var interaction = InitializeFloatInteractionMapping();

            // Initialize the bool values
            var initialBoolValue = interaction.BoolData;
            const bool boolValue1 = true;
            const bool boolValue2 = false;

            // Initialize the float values
            var initialFloatValue = interaction.FloatData;
            const float floatValue1 = 1f;
            const float floatValue2 = 9001f;

            // Set the bool value
            interaction.BoolData = boolValue1;
            Assert_Change_NoUpdate(interaction);

            // Set the float value
            interaction.FloatData = floatValue1;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.IsTrue(boolValue1 == interaction.BoolData);
            Assert.AreEqual(floatValue1, interaction.FloatData, double.Epsilon);

            Assert_NoChange_NoUpdate(interaction);

            // Set the bool value
            interaction.BoolData = boolValue2;
            Assert_Change_NoUpdate(interaction);

            // Set the float value
            interaction.FloatData = floatValue2;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.IsTrue(boolValue2 == interaction.BoolData);
            Assert.AreEqual(floatValue2, interaction.FloatData, double.Epsilon);
            Assert_NoChange_NoUpdate(interaction);

            // Set the bool value
            interaction.BoolData = initialBoolValue;
            Assert_NoChange_NoUpdate(interaction);

            // Set the float value
            interaction.FloatData = initialFloatValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.IsTrue(initialBoolValue == interaction.BoolData);
            Assert.AreEqual(initialFloatValue, interaction.FloatData, double.Epsilon);
            Assert_NoChange_NoUpdate(interaction);
        }

        /// <summary>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_03_03_Float_NoChange_Updated()
        {
            var interaction = InitializeFloatInteractionMapping();
            var initialValue = interaction.FloatData;
            const float testValue = 1f;

            // Set the values
            interaction.FloatData = testValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.AreEqual(testValue, interaction.FloatData, double.Epsilon);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.FloatData = testValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.AreEqual(testValue, interaction.FloatData, double.Epsilon);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.FloatData = initialValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.AreEqual(initialValue, interaction.FloatData, double.Epsilon);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.FloatData = initialValue;
            Assert_NoChange_NoUpdate(interaction);

            // Check the values
            Assert.AreEqual(initialValue, interaction.FloatData, double.Epsilon);
            Assert_NoChange_NoUpdate(interaction);
        }

        /// <summary>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_03_03_Float_NoChange_Updated_Inverted()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.SingleAxis, DeviceInputType.None, MixedRealityInputAction.None, KeyCode.None, string.Empty, string.Empty, true);
            var initialValue = interaction.FloatData;
            const float testValue1 = 1f;
            const float testValue2 = -1f;

            // Set the values
            interaction.FloatData = testValue1;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.AreEqual(testValue1 * -1f, interaction.FloatData, double.Epsilon);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.FloatData = testValue2;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.AreEqual(testValue2 * -1f, interaction.FloatData, double.Epsilon);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.FloatData = initialValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.AreEqual(initialValue * -1f, interaction.FloatData, double.Epsilon);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.FloatData = initialValue;
            Assert_NoChange_NoUpdate(interaction);

            // Check the values
            Assert.AreEqual(initialValue * -1f, interaction.FloatData, double.Epsilon);
            Assert_NoChange_NoUpdate(interaction);
        }

        #endregion float

        #region Vector2

        public MixedRealityInteractionMapping InitializeVector2InteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.DualAxis, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_04_01_InitializedVector2()
        {
            var interaction = InitializeVector2InteractionMapping();
            var initialValue = interaction.Vector2Data;

            // Test to make sure the initial values are correct.
            Assert.True(initialValue == Vector2.zero);

            Assert_NoChange_NoUpdate(interaction);

            interaction.Vector2Data = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.True(initialValue == Vector2.zero);

            Assert_NoChange_NoUpdate(interaction);
        }

        [Test]
        public void Test_04_01_01_Vector2_One()
        {
            var iterations = 10000000;
            var watch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                var test = Vector2.one;
            }

            watch.Stop();
            Debug.Log($"Test Vector2.one execution Time: {watch.Elapsed.TotalMilliseconds} ms");

            watch = Stopwatch.StartNew();
            var vector2One = new Vector2(1f, 1f);

            for (int i = 0; i < iterations; i++)
            {
                var test = vector2One;
            }

            watch.Stop();
            Debug.Log($"Test cached Vector2 execution Time: {watch.Elapsed.TotalMilliseconds} ms");

            watch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                var test = new Vector2(1f, 1f);
            }

            watch.Stop();
            Debug.Log($"Test new Vector2 execution Time: {watch.Elapsed.TotalMilliseconds} ms");
        }

        [Test]
        public void Test_04_01_02_SpeedChecks()
        {
            var iterations = 10000000;
            var value = new Vector2(1, 1);
            var vector2Data = new Vector2(2, 2);
            var changed = false;
            var updated = false;
            var invertAxis = true;

            var cachedVector2 = new Vector2(1f, 1f);

            var watch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                var invertMultiplier = Vector2.one;

                if (invertAxis)
                {
                    invertMultiplier.x = -1f;
                }

                if (!invertAxis)
                {
                    invertMultiplier.y = -1f;
                }

                invertAxis = !invertAxis;
                var newValue = value * invertMultiplier;
                changed = vector2Data != newValue;
                updated = changed || !newValue.Equals(cachedVector2);
                // use the internal reading for changed so we don't reset it.
                vector2Data = newValue;
            }

            watch.Stop();
            Debug.Log($"Test Vector2.one execution Time: {watch.Elapsed.TotalMilliseconds} ms");

            watch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                var invertMultiplier = cachedVector2;

                if (invertAxis)
                {
                    invertMultiplier.x = -1f;
                }
                else
                {
                    invertMultiplier.x = 1f;
                }

                if (!invertAxis)
                {
                    invertMultiplier.y = -1f;
                }
                else
                {
                    invertMultiplier.y = 1f;
                }

                invertAxis = !invertAxis;
                var newValue = value * invertMultiplier;
                changed = vector2Data != newValue;
                updated = changed || !newValue.Equals(cachedVector2);
                vector2Data = newValue;
            }

            watch.Stop();
            Debug.Log($"Test Vector2 execution Time: {watch.Elapsed.TotalMilliseconds} ms");

            watch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                var newValue = value * new Vector2(invertAxis ? -1f : 1f, !invertAxis ? -1f : 1f);
                invertAxis = !invertAxis;
                changed = vector2Data != newValue;
                updated = changed || !newValue.Equals(cachedVector2);
                vector2Data = newValue;
            }

            watch.Stop();
            Debug.Log($"Test new Vector2 execution Time: {watch.Elapsed.TotalMilliseconds} ms");

            watch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                var newValue = value;

                if (invertAxis)
                {
                    newValue.x *= -1f;
                }

                if (!invertAxis)
                {
                    newValue.y *= -1f;
                }

                invertAxis = !invertAxis;
                changed = vector2Data != newValue;
                updated = changed || !newValue.Equals(cachedVector2);
                vector2Data = newValue;
            }

            watch.Stop();
            Debug.Log($"Test Modify Vector2 execution Time: {watch.Elapsed.TotalMilliseconds} ms");
        }

        /// <summary>
        /// We test by setting the interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_04_02_01_Vector2_Changed_Updated()
        {
            var interaction = InitializeVector2InteractionMapping();
            var initialValue = interaction.Vector2Data;
            var testValue1 = Vector2.up;
            var testValue2 = Vector2.down;
            var testValue3 = new Vector2(0.25f, 1f);
            var testValue4 = new Vector2(0.75f, 1f);

            // set the values
            interaction.Vector2Data = testValue1;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.Vector2Data == testValue1);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.Vector2Data = testValue2;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.Vector2Data == testValue2);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.Vector2Data = testValue3;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.Vector2Data == testValue3);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.Vector2Data = testValue4;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.Vector2Data == testValue4);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.Vector2Data = initialValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.Vector2Data == initialValue);
            Assert_NoChange_NoUpdate(interaction);
        }

        /// <summary>
        /// We test by setting the inverted interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_04_02_02_Vector2_NoChanged_Updated_Inverted()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.DualAxis, DeviceInputType.None, MixedRealityInputAction.None, KeyCode.None, string.Empty, string.Empty, true, true);

            var initialValue = interaction.Vector2Data;
            var testValue1 = Vector2.up;
            var testValue2 = Vector2.down;
            var testValue3 = new Vector2(0.25f, 1f);
            var testValue4 = new Vector2(0.75f, 1f);

            // Set the values
            interaction.Vector2Data = testValue1;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.Vector2Data == testValue1 * -1f);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.Vector2Data = testValue2;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.Vector2Data == testValue2 * -1f);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.Vector2Data = testValue3;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.Vector2Data == testValue3 * -1f);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.Vector2Data = testValue4;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.Vector2Data == testValue4 * -1f);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.Vector2Data = initialValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.Vector2Data == initialValue);
            Assert_NoChange_NoUpdate(interaction);
        }

        /// <summary>
        /// We test by setting the interaction data to the same object multiple times.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true.<para/>
        /// </summary>
        [Test]
        public void Test_04_03_Vector2_NoChange_Updated()
        {
            var interaction = InitializeVector2InteractionMapping();
            var testValue = Vector2.one;

            // Set the values
            interaction.Vector2Data = testValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.Vector2Data == testValue);
            Assert_NoChange_NoUpdate(interaction);

            interaction.Vector2Data = testValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.Vector2Data == testValue);
            Assert_NoChange_NoUpdate(interaction);
        }

        #endregion Vector2

        #region Vector3

        public MixedRealityInteractionMapping InitializeVector3InteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.ThreeDofPosition, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by setting the interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_05_01_InitializedVector3()
        {
            var interaction = InitializeVector3InteractionMapping();
            var initialValue = interaction.PositionData;

            // Test to make sure the initial values are correct.
            Assert.True(initialValue == Vector3.zero);
            Assert_NoChange_NoUpdate(interaction);

            interaction.PositionData = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.True(initialValue == Vector3.zero);
            Assert_NoChange_NoUpdate(interaction);
        }

        /// <summary>
        /// We test by setting the interaction data to the same object multiple times.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true.<para/>
        /// </summary>
        [Test]
        public void Test_05_02_Vector3_NoChange_Updated()
        {
            var interaction = InitializeVector3InteractionMapping();
            var initialValue = interaction.PositionData;
            var testValue1 = Vector3.one;
            var testValue2 = Vector3.one * 0.5f;

            // Set the values
            interaction.PositionData = testValue1;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.PositionData == testValue1);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.PositionData = testValue2;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.PositionData == testValue2);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.PositionData = initialValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.PositionData == initialValue);
            Assert_NoChange_NoUpdate(interaction);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_05_03_Vector3_NoChange_NoUpdate()
        {
            var interaction = InitializeVector3InteractionMapping();
            var initialValue = interaction.PositionData;
            var testValue = Vector3.one;

            // Set the values
            interaction.PositionData = testValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.PositionData == testValue);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.PositionData = initialValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.PositionData == initialValue);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.PositionData = initialValue;
            Assert_NoChange_NoUpdate(interaction);

            // Check the values
            Assert.True(interaction.PositionData == initialValue);
            Assert_NoChange_NoUpdate(interaction);
        }

        #endregion Vector3

        #region Quaternion

        public MixedRealityInteractionMapping InitializeQuaternionInteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.ThreeDofRotation, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by setting the interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_06_01_InitializeQuaternion()
        {
            var interaction = InitializeQuaternionInteractionMapping();
            var initialValue = interaction.RotationData;

            // Test to make sure the initial values are correct.
            Assert.IsTrue(initialValue == Quaternion.identity);
            Assert_NoChange_NoUpdate(interaction);

            interaction.RotationData = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.IsTrue(initialValue == Quaternion.identity);
            Assert_NoChange_NoUpdate(interaction);
        }

        /// <summary>
        /// We test by setting the interaction data to the same object multiple times.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true.<para/>
        /// </summary>
        [Test]
        public void Test_06_02_Quaternion_NoChanged_Updated()
        {
            var interaction = InitializeQuaternionInteractionMapping();
            var initialValue = interaction.RotationData;
            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.Euler(270f, 270f, 270f);

            // Set the values
            interaction.RotationData = testValue1;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.RotationData == testValue1);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.RotationData = testValue2;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.RotationData == testValue2);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.RotationData = initialValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.RotationData == initialValue);
            Assert_NoChange_NoUpdate(interaction);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_06_03_Quaternion_NoChange_NoUpdate()
        {
            var interaction = InitializeQuaternionInteractionMapping();
            var initialValue = interaction.RotationData;
            var testValue = Quaternion.Euler(45f, 45f, 45f);

            // Set the values
            interaction.RotationData = testValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.RotationData == testValue);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.RotationData = initialValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.RotationData == initialValue);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.RotationData = initialValue;
            Assert_NoChange_NoUpdate(interaction);

            // Check the values
            Assert.True(interaction.RotationData == initialValue);
            Assert_NoChange_NoUpdate(interaction);
        }

        #endregion Quaternion

        #region MixedRealityPose

        public MixedRealityInteractionMapping InitializeMixedRealityPoseInteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.SixDof, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by setting the interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_07_01_InitializePoseData()
        {
            var interaction = InitializeMixedRealityPoseInteractionMapping();
            var initialValue = interaction.PoseData;

            // Test to make sure the initial values are correct.
            Assert.IsTrue(initialValue == MixedRealityPose.ZeroIdentity);
            Assert_NoChange_NoUpdate(interaction);

            interaction.PoseData = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.IsTrue(initialValue == MixedRealityPose.ZeroIdentity);
            Assert_NoChange_NoUpdate(interaction);
        }

        /// <summary>
        /// We test by setting the interaction data to the same object multiple times.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true.<para/>
        /// </summary>
        [Test]
        public void Test_07_02_MixedRealityPose_NoChanged_Updated()
        {
            var interaction = InitializeMixedRealityPoseInteractionMapping();
            var initialValue = interaction.PoseData;
            var testValue1 = new MixedRealityPose(Vector3.up, Quaternion.identity);
            var testValue2 = new MixedRealityPose(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            // Set the values
            interaction.PoseData = testValue1;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.PoseData == testValue1);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.PoseData = testValue2;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.PoseData == testValue2);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.PoseData = initialValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.PoseData == initialValue);
            Assert_NoChange_NoUpdate(interaction);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.ControlActivated"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_07_03_MixedRealityPose_NoChange_NoUpdate()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.SixDof, DeviceInputType.None, MixedRealityInputAction.None);
            var initialValue = interaction.PoseData;
            var testValue = new MixedRealityPose(Vector3.up, Quaternion.identity);

            // Set the values
            interaction.PoseData = testValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.PoseData == testValue);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.PoseData = initialValue;
            Assert_NoChange_Update(interaction);

            // Check the values
            Assert.True(interaction.PoseData == initialValue);
            Assert_NoChange_NoUpdate(interaction);

            // Set the values
            interaction.PoseData = initialValue;
            Assert_NoChange_NoUpdate(interaction);

            // Check the values
            Assert.True(interaction.PoseData == initialValue);
            Assert_NoChange_NoUpdate(interaction);
        }

        #endregion MixedRealityPose
    }
}