// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Devices
{
    /// <summary>
    /// Maps the capabilities of controllers, linking the Physical inputs of a controller to a Logical construct in a runtime project<para/>
    /// </summary>
    /// <remarks>One definition should exist for each physical device input, such as buttons, triggers, joysticks, dpads, and more.</remarks>
    [Serializable]
    public class MixedRealityInteractionMapping
    {
        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="id">Identity for mapping</param>
        /// <param name="description">The description of the interaction mapping.</param> 
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control</param>
        /// <param name="axisCodeX">Optional horizontal or single axis value to get axis data from Unity's old input system.</param>
        /// <param name="axisCodeY">Optional vertical axis value to get axis data from Unity's old input system.</param>
        /// <param name="invertXAxis">Optional horizontal axis invert option.</param>
        /// <param name="invertYAxis">Optional vertical axis invert option.</param> 
        public MixedRealityInteractionMapping(uint id, string description, AxisType axisType, DeviceInputType inputType, string axisCodeX, string axisCodeY = "", bool invertXAxis = false, bool invertYAxis = false)
        {
            this.id = id;
            this.description = description;
            this.axisType = axisType;
            this.inputType = inputType;
            inputAction = MixedRealityInputAction.None;
            keyCode = KeyCode.None;
            this.inputName = string.Empty;
            this.axisCodeX = axisCodeX;
            this.axisCodeY = axisCodeY;
            this.invertXAxis = invertXAxis;
            this.invertYAxis = invertYAxis;
            rawData = null;
            boolData = false;
            floatData = 0f;
            vector2Data = Vector2.zero;
            positionData = Vector3.zero;
            rotationData = Quaternion.identity;
            poseData = MixedRealityPose.ZeroIdentity;
            activated = false;
            updated = false;
            inputName = string.Empty;
        }

        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="id">Identity for mapping</param>
        /// <param name="description">The description of the interaction mapping.</param> 
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control</param>
        /// <param name="keyCode">Optional KeyCode value to get input from Unity's old input system</param>
        public MixedRealityInteractionMapping(uint id, string description, AxisType axisType, DeviceInputType inputType, KeyCode keyCode)
        {
            this.id = id;
            this.description = description;
            this.axisType = axisType;
            this.inputType = inputType;
            inputAction = MixedRealityInputAction.None;
            this.keyCode = keyCode;
            axisCodeX = string.Empty;
            axisCodeY = string.Empty;
            rawData = null;
            boolData = false;
            floatData = 0f;
            vector2Data = Vector2.zero;
            positionData = Vector3.zero;
            rotationData = Quaternion.identity;
            poseData = MixedRealityPose.ZeroIdentity;
            activated = false;
            updated = false;
            inputName = string.Empty;
            invertXAxis = false;
            invertYAxis = false;
        }

        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="id">Identity for mapping</param>
        /// <param name="description">The description of the interaction mapping.</param> 
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control</param>
        /// <param name="inputName">Optional inputName value to get input for a coded input identity from a provider</param>
        public MixedRealityInteractionMapping(uint id, string description, AxisType axisType, string inputName, DeviceInputType inputType)
        {
            this.id = id;
            this.description = description;
            this.axisType = axisType;
            this.inputType = inputType;
            inputAction = MixedRealityInputAction.None;
            this.inputName = inputName;
            axisCodeX = string.Empty;
            axisCodeY = string.Empty;
            rawData = null;
            boolData = false;
            floatData = 0f;
            vector2Data = Vector2.zero;
            positionData = Vector3.zero;
            rotationData = Quaternion.identity;
            poseData = MixedRealityPose.ZeroIdentity;
            activated = false;
            updated = false;
            keyCode = KeyCode.None;
            invertXAxis = false;
            invertYAxis = false;
        }

        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="id">Identity for mapping</param>
        /// <param name="description">The description of the interaction mapping.</param> 
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control</param>
        /// <param name="inputAction">The logical MixedRealityInputAction that this input performs</param>
        /// <param name="keyCode">Optional KeyCode value to get input from Unity's old input system</param>
        /// <param name="axisCodeX">Optional horizontal or single axis value to get axis data from Unity's old input system.</param>
        /// <param name="axisCodeY">Optional vertical axis value to get axis data from Unity's old input system.</param>
        /// <param name="invertXAxis">Optional horizontal axis invert option.</param>
        /// <param name="invertYAxis">Optional vertical axis invert option.</param> 
        public MixedRealityInteractionMapping(uint id, string description, AxisType axisType, DeviceInputType inputType, MixedRealityInputAction inputAction, KeyCode keyCode = KeyCode.None, string axisCodeX = "", string axisCodeY = "", bool invertXAxis = false, bool invertYAxis = false)
        {
            this.id = id;
            this.description = description;
            this.axisType = axisType;
            this.inputType = inputType;
            this.inputAction = inputAction;
            this.keyCode = keyCode;
            this.axisCodeX = axisCodeX;
            this.axisCodeY = axisCodeY;
            this.invertXAxis = invertXAxis;
            this.invertYAxis = invertYAxis;
            rawData = null;
            boolData = false;
            floatData = 0f;
            vector2Data = Vector2.zero;
            positionData = Vector3.zero;
            rotationData = Quaternion.identity;
            poseData = MixedRealityPose.ZeroIdentity;
            activated = false;
            updated = false;
            inputName = string.Empty;
        }

        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="id">Identity for mapping</param>
        /// <param name="description">The description of the interaction mapping.</param> 
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control</param>
        /// <param name="inputAction">The logical MixedRealityInputAction that this input performs</param>
        /// <param name="inputName">Optional inputName value to get input for a coded input identity from a provider</param>
        /// <param name="axisCodeX">Optional horizontal or single axis value to get axis data from Unity's old input system.</param>
        /// <param name="axisCodeY">Optional vertical axis value to get axis data from Unity's old input system.</param>
        /// <param name="invertXAxis">Optional horizontal axis invert option.</param>
        /// <param name="invertYAxis">Optional vertical axis invert option.</param> 
        public MixedRealityInteractionMapping(uint id, string description, AxisType axisType, DeviceInputType inputType, string inputName, MixedRealityInputAction inputAction, string axisCodeX = "", string axisCodeY = "", bool invertXAxis = false, bool invertYAxis = false)
        {
            this.id = id;
            this.description = description;
            this.axisType = axisType;
            this.inputType = inputType;
            this.inputAction = inputAction;
            this.inputName = inputName;
            this.axisCodeX = axisCodeX;
            this.axisCodeY = axisCodeY;
            this.invertXAxis = invertXAxis;
            this.invertYAxis = invertYAxis;
            rawData = null;
            boolData = false;
            floatData = 0f;
            vector2Data = Vector2.zero;
            positionData = Vector3.zero;
            rotationData = Quaternion.identity;
            poseData = MixedRealityPose.ZeroIdentity;
            activated = false;
            updated = false;
            keyCode = KeyCode.None;
        }

        /// <summary>
        /// Creates a copy of a <see cref="MixedRealityInteractionMapping"/>
        /// </summary>
        /// <param name="mixedRealityInteractionMapping"></param>
        public MixedRealityInteractionMapping(MixedRealityInteractionMapping mixedRealityInteractionMapping)
        {
            id = mixedRealityInteractionMapping.id;
            description = mixedRealityInteractionMapping.description;
            axisType = mixedRealityInteractionMapping.axisType;
            inputType = mixedRealityInteractionMapping.inputType;
            inputAction = mixedRealityInteractionMapping.inputAction;
            keyCode = mixedRealityInteractionMapping.keyCode;
            inputName = mixedRealityInteractionMapping.inputName;
            axisCodeX = mixedRealityInteractionMapping.axisCodeX;
            axisCodeY = mixedRealityInteractionMapping.axisCodeY;
            invertXAxis = mixedRealityInteractionMapping.invertXAxis;
            invertYAxis = mixedRealityInteractionMapping.invertYAxis;
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

        #region Interaction Properties

        [SerializeField]
        [Tooltip("The Id assigned to the Interaction.")]
        private uint id;

        /// <summary>
        /// The Id assigned to the Interaction.
        /// </summary>
        public uint Id => id;

        [SerializeField]
        [Tooltip("The description of the interaction mapping.")]
        private string description;

        /// <summary>
        /// The description of the interaction mapping.
        /// </summary>
        public string Description => description;

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
            internal set => inputAction = value;
        }

        [SerializeField]
        [Tooltip("Optional KeyCode value to get input from Unity's old input system.")]
        private KeyCode keyCode;

        /// <summary>
        /// Optional KeyCode value to get input from Unity's old input system.
        /// </summary>
        public KeyCode KeyCode => keyCode;

        [SerializeField]
        [Tooltip("Optional KeyCode value to get input from Unity's old input system.")]
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
        [Tooltip("Should the X axis be inverted?")]
        private bool invertXAxis;

        /// <summary>
        /// Should the X axis be inverted?
        /// </summary>
        /// <remarks>
        /// Only valid for <see cref="Utilities.AxisType.SingleAxis"/> and <see cref="Utilities.AxisType.DualAxis"/> inputs.
        /// </remarks>
        public bool InvertXAxis
        {
            get => invertXAxis;
            set
            {
                if (axisType != AxisType.SingleAxis && axisType != AxisType.DualAxis)
                {
                    Debug.LogWarning("Inverted X axis only valid for Single or Dual Axis inputs.");
                    return;
                }

                invertXAxis = value;
            }
        }

        [SerializeField]
        [Tooltip("Should the Y axis be inverted?")]
        private bool invertYAxis;

        /// <summary>
        /// Should the Y axis be inverted?
        /// </summary>
        /// <remarks>
        /// Only valid for <see cref="Utilities.AxisType.DualAxis"/> inputs.
        /// </remarks>
        public bool InvertYAxis
        {
            get => invertYAxis;
            set
            {
                if (axisType != AxisType.DualAxis)
                {
                    Debug.LogWarning("Inverted Y axis only valid for Dual Axis inputs.");
                    return;
                }

                invertYAxis = value;
            }
        }

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

        #endregion Interaction Properties

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
        /// <remarks>Only supported for a Raw mapping axis type</remarks>
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
        /// <remarks>Only supported for a Digital mapping axis type</remarks>
        public bool BoolData
        {
            get => boolData;

            set
            {
                if (AxisType != AxisType.Digital && AxisType != AxisType.SingleAxis && AxisType != AxisType.DualAxis)
                {
                    Debug.LogError($"{nameof(BoolData)} value can only be set when the {nameof(AxisType)} is {nameof(AxisType.SingleAxis)} or {nameof(AxisType.Digital)}\nPlease check the {inputType} mapping for the current controller");
                }

                ControlActivated = boolData != value;
                boolData = value;
            }
        }

        /// <summary>
        /// The Float data value.
        /// </summary>
        /// <remarks>Only supported for a SingleAxis mapping axis type</remarks>
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

                if (invertXAxis)
                {
                    newValue *= -1f;
                }

                Updated = !floatData.Equals(newValue) || !floatData.Equals(0f);
                floatData = newValue;
            }
        }

        /// <summary>
        /// The Vector2 data value.
        /// </summary>
        /// <remarks>Only supported for a DualAxis mapping axis type</remarks>
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

                if (invertXAxis)
                {
                    newValue.x *= -1f;
                }

                if (invertYAxis)
                {
                    newValue.y *= -1f;
                }

                Updated = vector2Data != newValue || !newValue.x.Equals(0f) && !newValue.y.Equals(0f);
                vector2Data = newValue;
            }
        }

        /// <summary>
        /// The ThreeDof Vector3 Position data value.
        /// </summary>
        /// <remarks>Only supported for a ThreeDof mapping axis type</remarks>
        public Vector3 PositionData
        {
            get => positionData;

            set
            {
                if (AxisType != AxisType.ThreeDofPosition)
                {
                    Debug.LogError($"{nameof(PositionData)} value can only be set when the {nameof(AxisType)} is {nameof(AxisType.ThreeDofPosition)}.\nPlease check the {inputType} mapping for the current controller");
                }

                Updated = positionData != value || !value.x.Equals(0f) && !value.y.Equals(0f) && !value.z.Equals(0f);
                positionData = value;
            }
        }

        /// <summary>
        /// The ThreeDof Quaternion Rotation data value.
        /// </summary>
        /// <remarks>Only supported for a ThreeDof mapping axis type</remarks>
        public Quaternion RotationData
        {
            get => rotationData;

            set
            {
                if (AxisType != AxisType.ThreeDofRotation)
                {
                    Debug.LogError($"{nameof(RotationData)} value can only be set when the {nameof(AxisType)} is {nameof(AxisType.ThreeDofRotation)}\nPlease check the {inputType} mapping for the current controller");
                }

                Updated = rotationData != value || !value.x.Equals(0f) && !value.y.Equals(0f) && !value.z.Equals(0f) && value.w.Equals(1f);
                rotationData = value;
            }
        }

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

                Updated = poseData != value || !value.Equals(MixedRealityPose.ZeroIdentity);
                poseData = value;
                positionData = poseData.Position;
                rotationData = poseData.Rotation;
            }
        }

        #endregion Data Properties
    }
}
