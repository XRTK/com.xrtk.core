// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Controllers.Simulation.Hands;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up controller pointers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System Profile", fileName = "MixedRealityInputSystemProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class MixedRealityInputSystemProfile : BaseMixedRealityProfile
    {
        #region Global Input System Options

        [SerializeField]
        [Tooltip("The focus provider service concrete type to use when raycasting.")]
        [Implements(typeof(IMixedRealityFocusProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType focusProviderType;

        /// <summary>
        /// The focus provider service concrete type to use when raycasting.
        /// </summary>
        public SystemType FocusProviderType
        {
            get => focusProviderType;
            internal set => focusProviderType = value;
        }

        [SerializeField]
        [Tooltip("The concrete type of IMixedRealityGazeProvider to use.")]
        [Implements(typeof(IMixedRealityGazeProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType gazeProviderType;

        /// <summary>
        /// The concrete type of <see cref="IMixedRealityGazeProvider"/> to use.
        /// </summary>
        public SystemType GazeProviderType
        {
            get => gazeProviderType;
            internal set => gazeProviderType = value;
        }

        [Prefab]
        [SerializeField]
        [Tooltip("The gaze cursor prefab to use on the Gaze pointer.")]
        private GameObject gazeCursorPrefab = null;

        /// <summary>
        /// The gaze cursor prefab to use on the Gaze pointer.
        /// </summary>
        public GameObject GazeCursorPrefab => gazeCursorPrefab;

        #region Global Pointer Options

        [SerializeField]
        [Tooltip("Maximum distance at which all pointers can collide with a GameObject, unless it has an override extent.")]
        private float pointingExtent = 10f;

        /// <summary>
        /// Maximum distance at which all pointers can collide with a GameObject, unless it has an override extent.
        /// </summary>
        public float PointingExtent => pointingExtent;

        [SerializeField]
        [Tooltip("The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.")]
        private LayerMask[] pointingRaycastLayerMasks = { UnityEngine.Physics.DefaultRaycastLayers };

        /// <summary>
        /// The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.
        /// </summary>
        public LayerMask[] PointingRaycastLayerMasks => pointingRaycastLayerMasks;

        [SerializeField]
        private bool drawDebugPointingRays = false;

        /// <summary>
        /// Toggle to enable or disable debug pointing rays.
        /// </summary>
        public bool DrawDebugPointingRays => drawDebugPointingRays;

        [SerializeField]
        private Color[] debugPointingRayColors = { Color.green };

        /// <summary>
        /// The colors to use when debugging pointer rays.
        /// </summary>
        public Color[] DebugPointingRayColors => debugPointingRayColors;

        #endregion Global Pointer Options

        #region Global Hand Options

        [SerializeField]
        [Tooltip("If set, hand mesh data will be read and available for visualization. Disable for optimized performance.")]
        private bool handMeshingEnabled = false;

        /// <summary>
        /// If set, hand mesh data will be read and available for visualization. Disable for optimized performance.
        /// </summary>
        public bool HandMeshingEnabled => handMeshingEnabled;

        [SerializeField]
        [Tooltip("If set, hands will be setup with colliders and a rigidbody to work with Unity's physics system.")]
        private bool handPhysicsEnabled = false;

        /// <summary>
        /// If set, hands will be setup with colliders and a rigidbody to work with Unity's physics system.
        /// </summary>
        public bool HandPhysicsEnabled => handPhysicsEnabled;

        [SerializeField]
        [Tooltip("If set, hand colliders will be setup as triggers.")]
        private bool useTriggers = false;

        /// <summary>
        /// If set, hand colliders will be setup as triggers.
        /// </summary>
        public bool UseTriggers => useTriggers;

        [SerializeField]
        [Tooltip("Set the bounds mode to use for calculating hand bounds.")]
        private HandBoundsMode boundsMode = HandBoundsMode.Hand;

        /// <summary>
        /// Set the bounds mode to use for calculating hand bounds.
        /// </summary>
        public HandBoundsMode BoundsMode => boundsMode;

        [SerializeField]
        [Tooltip("Tracked hand poses for pose detection.")]
        private List<SimulatedHandControllerPoseData> trackedPoses = new List<SimulatedHandControllerPoseData>();

        /// <summary>
        /// Tracked hand poses for pose detection.
        /// </summary>
        public IReadOnlyList<SimulatedHandControllerPoseData> TrackedPoses => trackedPoses;

        #endregion Global Hand Options

        #endregion Global Input System Options

        #region Profile Options

        [SerializeField]
        [Tooltip("Input System Action Mapping profile for setting up avery action a user can make in your application.")]
        private MixedRealityInputActionsProfile inputActionsProfile;

        /// <summary>
        /// Input System Action Mapping profile for setting up avery action a user can make in your application.
        /// </summary>
        public MixedRealityInputActionsProfile InputActionsProfile
        {
            get => inputActionsProfile;
            internal set => inputActionsProfile = value;
        }

        [SerializeField]
        [Tooltip("Speech Command profile for wiring up Voice Input to Actions.")]
        private MixedRealitySpeechCommandsProfile speechCommandsProfile;

        /// <summary>
        /// Speech commands profile for configured speech commands, for use by the speech recognition system
        /// </summary>
        public MixedRealitySpeechCommandsProfile SpeechCommandsProfile
        {
            get => speechCommandsProfile;
            internal set => speechCommandsProfile = value;
        }

        [SerializeField]
        [Tooltip("Gesture Mapping Profile for recognizing gestures across all platforms.")]
        private MixedRealityGesturesProfile gesturesProfile;

        /// <summary>
        /// Gesture Mapping Profile for recognizing gestures across all platforms.
        /// </summary>
        public MixedRealityGesturesProfile GesturesProfile
        {
            get => gesturesProfile;
            internal set => gesturesProfile = value;
        }

        [SerializeField]
        [Tooltip("Device profile for registering platform specific input data sources.")]
        private MixedRealityInputDataProvidersProfile inputDataProvidersProfile;

        /// <summary>
        /// Active profile for controller mapping configuration
        /// </summary>
        public MixedRealityInputDataProvidersProfile InputDataProvidersProfile
        {
            get => inputDataProvidersProfile;
            internal set => inputDataProvidersProfile = value;
        }

        #endregion Profile Options
    }
}