// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;

namespace XRTK.Definitions.Devices
{
    /// <summary>
    /// Maps the capabilities of controllers, linking the Physical inputs of a controller to a Logical construct in a runtime project<para/>
    /// </summary>
    /// <remarks>One definition should exist for each physical device input, such as buttons, triggers, joysticks, dpads, and more.</remarks>
    [Serializable]
    public class MixedRealityInteractionMapping
    {
        #region Constructors

        public MixedRealityInteractionMapping(string description = "None", List<InputProcessor> inputProcessors = null)
        {
            this.description = description;
            this.inputProcessors = inputProcessors ?? new List<InputProcessor>();

            inputAction = MixedRealityInputAction.None;
            stateChangeType = StateChangeType.Continuous;
            inputName = string.Empty;
            axisType = AxisType.None;
            inputType = DeviceInputType.None;
            keyCode = KeyCode.None;
            axisCodeX = string.Empty;
            axisCodeY = string.Empty;

            rawData = null;
            boolData = false;
            floatData = 0f;
            vector2Data = Vector2.zero;
            positionData = Vector3.zero;
            rotationData = Quaternion.identity;
            poseData = MixedRealityPose.ZeroIdentity;
            updated = false;
            activated = false;
        }

        public MixedRealityInteractionMapping(string description, AxisType axisType, DeviceInputType inputType, List<InputProcessor> inputProcessors = null)
            : this(description, inputProcessors)
        {
            this.axisType = axisType;
            this.inputType = inputType;
        }

        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="description">The description of the interaction mapping.</param>
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping.</param>
        /// <param name="inputName">Optional inputName value to get input for a coded input identity from a provider.</param>
        /// <param name="inputType">The physical input device / control type.</param>
        /// <param name="inputProcessors"></param>
        public MixedRealityInteractionMapping(string description, AxisType axisType, string inputName, DeviceInputType inputType, List<InputProcessor> inputProcessors = null)
            : this(description, axisType, inputType, inputProcessors)
        {
            this.inputName = inputName;
        }

        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="description">The description of the interaction mapping.</param>
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control type.</param>
        /// <param name="axisCodeX">Optional horizontal or single axis value to get axis data from Unity's old input system.</param>
        /// <param name="axisCodeY">Optional vertical axis value to get axis data from Unity's old input system.</param>
        /// <param name="inputProcessors"></param>
        public MixedRealityInteractionMapping(string description, AxisType axisType, DeviceInputType inputType, string axisCodeX, string axisCodeY = "", List<InputProcessor> inputProcessors = null)
            : this(description, axisType, inputType, inputProcessors)
        {
            this.axisCodeX = axisCodeX;
            this.axisCodeY = axisCodeY;
        }

        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="description">The description of the interaction mapping.</param>
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control type.</param>
        /// <param name="keyCode">Optional KeyCode value to get input from Unity's old input system</param>
        /// <param name="inputProcessors"></param>
        public MixedRealityInteractionMapping(string description, AxisType axisType, DeviceInputType inputType, KeyCode keyCode, List<InputProcessor> inputProcessors = null)
            : this(description, axisType, inputType, inputProcessors)
        {
            this.keyCode = keyCode;
        }

        /// <summary>
        /// Creates a copy of a <see cref="MixedRealityInteractionMapping"/>
        /// </summary>
        /// <param name="mapping"></param>
        public MixedRealityInteractionMapping(MixedRealityInteractionMapping mapping) : this()
        {
            description = mapping.description;
            axisType = mapping.axisType;
            inputType = mapping.inputType;
            inputAction = mapping.inputAction;
            keyCode = mapping.keyCode;
            inputName = mapping.inputName;
            axisCodeX = mapping.axisCodeX;
            axisCodeY = mapping.axisCodeY;
            inputProcessors = mapping.inputProcessors;
            stateChangeType = mapping.stateChangeType;

            rawData = null;
            boolData = false;
            floatData = 0f;
            vector2Data = Vector2.zero;
            positionData = Vector3.zero;
            rotationData = Quaternion.identity;
            poseData = MixedRealityPose.ZeroIdentity;
            activated = false;
            updated = false;
        }

        #endregion Constructors

        #region Serialized Properties

        [SerializeField]
        [Tooltip("The human readable description of the interaction mapping.")]
        private string description;

        /// <summary>
        /// The description of the interaction mapping.
        /// </summary>
        public string Description => description;

        [SerializeField]
        [Tooltip("Influences how the Interaction determines state changes that will raise the InputAction.")]
        private StateChangeType stateChangeType;

        /// <summary>
        /// Influences how the Interaction determines state changes that will raise the <see cref="MixedRealityInputAction"/>.
        /// </summary>
        //public StateChangeType StateChangeType => stateChangeType;

        [SerializeField]
        [Tooltip("The axis type of the button, e.g. Analogue, Digital, etc.")]
        private AxisType axisType;

        /// <summary>
        /// The axis type of the button, e.g. Analogue, Digital, etc.
        /// </summary>
        public AxisType AxisType => axisType;

        [SerializeField]
        [Tooltip("The primary action of the input as defined by the controller SDK.")]
        private DeviceInputType inputType;

        /// <summary>
        /// The primary action of the input as defined by the controller SDK.
        /// </summary>
        public DeviceInputType InputType => inputType;

        [SerializeField]
        [Tooltip("Action to be raised to the Input Manager when the input data has changed.")]
        private MixedRealityInputAction inputAction;

        /// <summary>
        /// Action to be raised to the Input Manager when the input data has changed.
        /// </summary>
        public MixedRealityInputAction MixedRealityInputAction
        {
            get => inputAction;
            set => inputAction = value;
        }

        [SerializeField]
        [Tooltip("Optional KeyCode value to get input from Unity's old input system.")]
        private KeyCode keyCode;

        /// <summary>
        /// Optional KeyCode value to get input from Unity's old input system.
        /// </summary>
        public KeyCode KeyCode => keyCode;

        [SerializeField]
        [Tooltip("Optional inputName value to get input for a coded input identity from a provider.")]
        private string inputName;

        /// <summary>
        /// Optional inputName value to get input for a coded input identity from a provider.
        /// </summary>
        public string InputName => inputName;

        [SerializeField]
        [Tooltip("Optional horizontal or single axis value to get axis data from Unity's old input system.")]
        private string axisCodeX;

        /// <summary>
        /// Optional horizontal or single axis value to get axis data from Unity's old input system.
        /// </summary>
        public string AxisCodeX => axisCodeX;

        [SerializeField]
        [Tooltip("Optional vertical axis value to get axis data from Unity's old input system.")]
        private string axisCodeY;

        /// <summary>
        /// Optional vertical axis value to get axis data from Unity's old input system.
        /// </summary>
        public string AxisCodeY => axisCodeY;

        [SerializeField]
        private List<InputProcessor> inputProcessors;

        internal List<InputProcessor> InputProcessors
        {
            get => inputProcessors;
            set => inputProcessors = value;
        }

        #endregion Serialized Properties

        private bool activated;

        /// <summary>
        /// Has the control mechanism been given a signal that deviates from its initial state?
        /// </summary>
        public bool ControlActivated
        {
            get
            {
                bool returnValue = activated;

                if (activated)
                {
                    activated = false;
                }

                return returnValue;
            }
            private set => activated = value;
        }

        private bool updated;

        /// <summary>
        /// Has the value been updated since the last reading?
        /// </summary>
        public bool Updated
        {
            get
            {
                bool returnValue = updated;

                if (updated)
                {
                    updated = false;
                }

                return returnValue;
            }
            private set => updated = value;
        }

        #region Definition Data Items

        private object rawData;

        private bool boolData;

        private float floatData;

        private Vector2 vector2Data;

        private Vector3 positionData;

        private Quaternion rotationData;

        private MixedRealityPose poseData;

        #endregion Definition Data Items

        #region Data Properties

        /// <summary>
        /// The Raw (object) data value.
        /// </summary>
        /// <remarks>Only supported for a Raw mapping axis type.</remarks>
        public object RawData
        {
            get => rawData;
            set
            {
                if (AxisType != AxisType.Raw)
                {
                    Debug.LogError($"{nameof(RawData)} value can only be set when the {nameof(AxisType)} is {nameof(AxisType.Raw)}\nPlease check the {inputType} mapping for the current controller");
                }

                ControlActivated = rawData != value;
                // use the internal reading for changed so we don't reset it.
                Updated = activated || value != null;
                rawData = value;
            }
        }

        /// <summary>
        /// The Bool data value.
        /// </summary>
        /// <remarks>Only supported for a Digital mapping axis type.</remarks>
        public bool BoolData
        {
            get => boolData;

            set
            {
                if (AxisType != AxisType.Digital && AxisType != AxisType.SingleAxis && AxisType != AxisType.DualAxis)
                {
                    Debug.LogError($"{nameof(BoolData)} value can only be set when the {nameof(AxisType)} is {nameof(AxisType.SingleAxis)} or {nameof(AxisType.Digital)}\nPlease check the {inputType} mapping for the current controller");
                }

                var newValue = value;

                for (int i = 0; i < BoolInputProcessors.Count; i++)
                {
                    BoolInputProcessors[i].Process(ref newValue);
                }

                ControlActivated = boolData != newValue;
                boolData = newValue;
            }
        }

        private IReadOnlyList<InputProcessor<bool>> boolInputProcessors;

        private IReadOnlyList<InputProcessor<bool>> BoolInputProcessors => boolInputProcessors ?? (boolInputProcessors = GetInputProcessorForType<bool>());

        /// <summary>
        /// The Float data value.
        /// </summary>
        /// <remarks>Only supported for a SingleAxis mapping axis type.</remarks>
        public float FloatData
        {
            get => floatData;

            set
            {
                if (AxisType != AxisType.Digital &&
                    AxisType != AxisType.SingleAxis)
                {
                    Debug.LogError($"{nameof(FloatData)} value can only be set when the {nameof(AxisType)} is {nameof(AxisType.SingleAxis)} or {nameof(AxisType.Digital)}\nPlease check the {inputType} mapping for the current controller");
                }

                var newValue = value;

                for (int i = 0; i < FloatInputProcessors.Count; i++)
                {
                    FloatInputProcessors[i].Process(ref newValue);
                }

                Updated = !floatData.Equals(newValue) || !floatData.Equals(0f);
                floatData = newValue;
            }
        }

        private IReadOnlyList<InputProcessor<float>> floatInputProcessors;

        private IReadOnlyList<InputProcessor<float>> FloatInputProcessors => floatInputProcessors ?? (floatInputProcessors = GetInputProcessorForType<float>());

        /// <summary>
        /// The Vector2 data value.
        /// </summary>
        /// <remarks>Only supported for a DualAxis mapping axis type.</remarks>
        public Vector2 Vector2Data
        {
            get => vector2Data;

            set
            {
                if (AxisType != AxisType.DualAxis)
                {
                    Debug.LogError($"{nameof(Vector2Data)} value can only be set when the {nameof(AxisType)} is {nameof(AxisType.DualAxis)}\nPlease check the {inputType} mapping for the current controller");
                }

                var newValue = value;

                for (int i = 0; i < Vector2InputProcessors.Count; i++)
                {
                    Vector2InputProcessors[i].Process(ref newValue);
                }

                Updated = vector2Data != newValue || !newValue.x.Equals(0f) && !newValue.y.Equals(0f);
                vector2Data = newValue;
            }
        }

        private IReadOnlyList<InputProcessor<Vector2>> vector2InputProcessors;

        private IReadOnlyList<InputProcessor<Vector2>> Vector2InputProcessors => vector2InputProcessors ?? (vector2InputProcessors = GetInputProcessorForType<Vector2>());

        /// <summary>
        /// The ThreeDof Vector3 Position data value.
        /// </summary>
        /// <remarks>Only supported for a ThreeDof mapping axis type.</remarks>
        public Vector3 PositionData
        {
            get => positionData;

            set
            {
                if (AxisType != AxisType.ThreeDofPosition)
                {
                    Debug.LogError($"{nameof(PositionData)} value can only be set when the {nameof(AxisType)} is {nameof(AxisType.ThreeDofPosition)}.\nPlease check the {inputType} mapping for the current controller");
                }

                var newValue = value;

                for (int i = 0; i < Vector3InputProcessors.Count; i++)
                {
                    Vector3InputProcessors[i].Process(ref newValue);
                }

                Updated = positionData != newValue || !newValue.x.Equals(0f) && !newValue.y.Equals(0f) && !newValue.z.Equals(0f);
                positionData = newValue;
            }
        }

        private IReadOnlyList<InputProcessor<Vector3>> vector3InputProcessors;

        private IReadOnlyList<InputProcessor<Vector3>> Vector3InputProcessors => vector3InputProcessors ?? (vector3InputProcessors = GetInputProcessorForType<Vector3>());

        /// <summary>
        /// The ThreeDof Quaternion Rotation data value.
        /// </summary>
        /// <remarks>Only supported for a ThreeDof mapping axis type.</remarks>
        public Quaternion RotationData
        {
            get => rotationData;

            set
            {
                if (AxisType != AxisType.ThreeDofRotation)
                {
                    Debug.LogError($"{nameof(RotationData)} value can only be set when the {nameof(AxisType)} is {nameof(AxisType.ThreeDofRotation)}\nPlease check the {inputType} mapping for the current controller");
                }

                var newValue = value;

                for (int i = 0; i < QuaternionInputProcessors.Count; i++)
                {
                    QuaternionInputProcessors[i].Process(ref newValue);
                }

                Updated = rotationData != newValue || !newValue.x.Equals(0f) && !newValue.y.Equals(0f) && !newValue.z.Equals(0f) && newValue.w.Equals(1f);
                rotationData = newValue;
            }
        }

        private IReadOnlyList<InputProcessor<Quaternion>> quaternionInputProcessors;

        private IReadOnlyList<InputProcessor<Quaternion>> QuaternionInputProcessors => quaternionInputProcessors ?? (quaternionInputProcessors = GetInputProcessorForType<Quaternion>());

        /// <summary>
        /// The Pose data value.
        /// </summary>
        /// <remarks>Only supported for <see cref="Utilities.AxisType.SixDof"/> <see cref="AxisType"/>s</remarks>
        public MixedRealityPose PoseData
        {
            get => poseData;
            set
            {
                if (AxisType != AxisType.SixDof)
                {
                    Debug.LogError($"{nameof(PoseData)} value can only be set when the {nameof(AxisType)} is {nameof(AxisType.SixDof)}\nPlease check the {inputType} mapping for the current controller");
                }

                var newValue = value;

                for (int i = 0; i < PoseInputProcessors.Count; i++)
                {
                    PoseInputProcessors[i].Process(ref newValue);
                }

                Updated = poseData != newValue || !newValue.Equals(MixedRealityPose.ZeroIdentity);
                poseData = newValue;
                positionData = poseData.Position;
                rotationData = poseData.Rotation;
            }
        }

        private IReadOnlyList<InputProcessor<MixedRealityPose>> poseInputProcessors;

        private IReadOnlyList<InputProcessor<MixedRealityPose>> PoseInputProcessors => poseInputProcessors ?? (poseInputProcessors = GetInputProcessorForType<MixedRealityPose>());

        #endregion Data Properties

        /// <summary>
        /// Get the input processors for a specific axis type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public IReadOnlyList<InputProcessor<T>> GetInputProcessorForType<T>() where T : struct
        {
            var processors = new List<InputProcessor<T>>(inputProcessors.Count);

            for (int i = 0; i < inputProcessors.Count; i++)
            {
                var genericTypeConstraint = inputProcessors[i].GetType().FindTopmostGenericTypeArguments();

                if (genericTypeConstraint[0] == typeof(T))
                {
                    processors.Add((InputProcessor<T>)inputProcessors[i]);
                }
            }

            return processors;
        }

        /// <summary>
        /// Get the input processors for a specific <see cref="InputProcessor"/> type. (i.e. InvertDualAxisProcessor).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T GetInputProcessors<T>() where T : InputProcessor
        {
            for (int i = 0; i < inputProcessors.Count; i++)
            {
                if (inputProcessors[i].GetType() == typeof(T))
                {
                    return (T)inputProcessors[i];
                }
            }

            return null;
        }
    }
}