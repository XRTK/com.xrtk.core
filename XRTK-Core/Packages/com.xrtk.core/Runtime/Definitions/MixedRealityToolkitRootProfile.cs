// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Serialization;
using XRTK.Attributes;
using XRTK.Definitions.BoundarySystem;
using XRTK.Definitions.CameraSystem;
using XRTK.Definitions.DiagnosticsSystem;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.NetworkingSystem;
using XRTK.Definitions.SpatialAwarenessSystem;
using XRTK.Definitions.TeleportSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.BoundarySystem;
using XRTK.Interfaces.CameraSystem;
using XRTK.Interfaces.DiagnosticsSystem;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.NetworkingSystem;
using XRTK.Interfaces.SpatialAwarenessSystem;
using XRTK.Interfaces.TeleportSystem;

namespace XRTK.Definitions
{
    /// <summary>
    /// The root profile for the Mixed Reality Toolkit's settings.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Toolkit Root Profile", fileName = "MixedRealityToolkitRootProfile", order = (int)CreateProfileMenuItemIndices.Configuration - 1)]
    public sealed class MixedRealityToolkitRootProfile : BaseMixedRealityProfile
    {
        #region Mixed Reality Toolkit system properties

        #region Camera System Properties

        [SerializeField]
        [Tooltip("Enable the Camera System on Startup.")]
        private bool enableCameraSystem = false;

        /// <summary>
        /// Enable and configure the Camera Profile for the Mixed Reality Toolkit
        /// </summary>
        public bool IsCameraSystemEnabled
        {
            get => CameraSystemProfile != null && cameraSystemType != null && cameraSystemType.Type != null && enableCameraSystem;
            internal set => enableCameraSystem = value;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityCameraSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType cameraSystemType;

        /// <summary>
        /// Camera System class to instantiate at runtime.
        /// </summary>
        public SystemType CameraSystemType
        {
            get => cameraSystemType;
            internal set => cameraSystemType = value;
        }

        [SerializeField]
        [Tooltip("Camera system profile.")]
        [FormerlySerializedAs("cameraProfile")]
        private MixedRealityCameraSystemProfile cameraSystemProfile;

        /// <summary>
        /// Profile for customizing your camera and quality settings based on if your 
        /// head mounted display (HMD) is a transparent device or an occluded device.
        /// </summary>
        public MixedRealityCameraSystemProfile CameraSystemProfile
        {
            get => cameraSystemProfile;
            internal set => cameraSystemProfile = value;
        }

        #endregion Camera System Properties

        #region Input System Properties

        [SerializeField]
        [Tooltip("Enable the Input System on Startup.")]
        private bool enableInputSystem = false;

        /// <summary>
        /// Enable and configure the Input System component for the Mixed Reality Toolkit
        /// </summary>
        public bool IsInputSystemEnabled
        {
            get => inputSystemProfile != null && inputSystemType != null && inputSystemType.Type != null && enableInputSystem;
            internal set => enableInputSystem = value;
        }

        [SerializeField]
        [Tooltip("Input System profile for setting wiring up events and actions to input devices.")]
        private MixedRealityInputSystemProfile inputSystemProfile;

        /// <summary>
        /// Input System profile for setting wiring up events and actions to input devices.
        /// </summary>
        public MixedRealityInputSystemProfile InputSystemProfile
        {
            get => inputSystemProfile;
            internal set => inputSystemProfile = value;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityInputSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType inputSystemType;

        /// <summary>
        /// Input System Script File to instantiate at runtime.
        /// </summary>
        public SystemType InputSystemType
        {
            get => inputSystemType;
            internal set => inputSystemType = value;
        }

        #endregion Input System Properties

        #region Boundary System Properties

        [SerializeField]
        [Tooltip("Enable the Boundary on Startup.")]
        private bool enableBoundarySystem = false;

        /// <summary>
        /// Enable and configure the boundary system.
        /// </summary>
        public bool IsBoundarySystemEnabled
        {
            get => boundarySystemType != null && boundarySystemType.Type != null && enableBoundarySystem && BoundarySystemProfile != null;
            internal set => enableBoundarySystem = value;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityBoundarySystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType boundarySystemType;

        /// <summary>
        /// Boundary System Script File to instantiate at runtime.
        /// </summary>
        public SystemType BoundarySystemSystemType
        {
            get => boundarySystemType;
            internal set => boundarySystemType = value;
        }

        [SerializeField]
        [Tooltip("Profile for wiring up boundary visualization assets.")]
        [FormerlySerializedAs("boundaryVisualizationProfile")]
        private MixedRealityBoundaryProfile boundarySystemProfile;

        /// <summary>
        /// Active profile for controller mapping configuration
        /// </summary>
        public MixedRealityBoundaryProfile BoundarySystemProfile
        {
            get => boundarySystemProfile;
            internal set => boundarySystemProfile = value;
        }

        #endregion Boundary System Properties

        #region Teleportation System Properties

        [SerializeField]
        [Tooltip("Enable the Teleport System on Startup.")]
        private bool enableTeleportSystem = false;

        /// <summary>
        /// Enable and configure the boundary system.
        /// </summary>
        public bool IsTeleportSystemEnabled
        {
            get => teleportSystemType != null && teleportSystemType.Type != null && enableTeleportSystem;
            internal set => enableTeleportSystem = value;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityTeleportSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType teleportSystemType;

        /// <summary>
        /// Teleport System Script File to instantiate at runtime.
        /// </summary>
        public SystemType TeleportSystemSystemType
        {
            get => teleportSystemType;
            internal set => teleportSystemType = value;
        }

        [SerializeField]
        [Tooltip("Profile for configuring the teleport system.")]
        private MixedRealityTeleportSystemProfile teleportSystemProfile;

        /// <summary>
        /// Active profile for teleport configuration.
        /// </summary>
        public MixedRealityTeleportSystemProfile TeleportSystemProfile
        {
            get => teleportSystemProfile;
            internal set => teleportSystemProfile = value;
        }

        #endregion Teleportation System Properties

        #region Spatial Awareness System Properties

        [SerializeField]
        [Tooltip("Enable the Spatial Awareness system on Startup.")]
        private bool enableSpatialAwarenessSystem = false;

        /// <summary>
        /// Enable and configure the spatial awareness system.
        /// </summary>
        public bool IsSpatialAwarenessSystemEnabled
        {
            get => spatialAwarenessSystemType != null && spatialAwarenessSystemType.Type != null && enableSpatialAwarenessSystem && spatialAwarenessProfile != null;
            internal set => enableSpatialAwarenessSystem = value;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealitySpatialAwarenessSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType spatialAwarenessSystemType;

        /// <summary>
        /// Boundary System Script File to instantiate at runtime.
        /// </summary>
        public SystemType SpatialAwarenessSystemSystemType
        {
            get => spatialAwarenessSystemType;
            internal set => spatialAwarenessSystemType = value;
        }

        [SerializeField]
        [Tooltip("Profile for configuring the Spatial Awareness system.")]
        private MixedRealitySpatialAwarenessSystemProfile spatialAwarenessProfile;

        /// <summary>
        /// Active profile for spatial awareness configuration
        /// </summary>
        public MixedRealitySpatialAwarenessSystemProfile SpatialAwarenessProfile
        {
            get => spatialAwarenessProfile;
            internal set => spatialAwarenessProfile = value;
        }

        #endregion Spatial Awareness System Properties

        #region Networking System Properties

        [SerializeField]
        [Tooltip("Profile for wiring up networking assets.")]
        private MixedRealityNetworkSystemProfile networkingSystemProfile;

        /// <summary>
        /// Active profile for diagnostic configuration
        /// </summary>
        public MixedRealityNetworkSystemProfile NetworkingSystemProfile
        {
            get => networkingSystemProfile;
            internal set => networkingSystemProfile = value;
        }

        [SerializeField]
        [Tooltip("Enable Networking System on Startup.")]
        private bool enableNetworkingSystem = false;

        /// <summary>
        /// Is the networking system properly configured and enabled?
        /// </summary>
        public bool IsNetworkingSystemEnabled
        {
            get => enableNetworkingSystem && NetworkingSystemSystemType?.Type != null && networkingSystemProfile != null;
            internal set => enableNetworkingSystem = value;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityNetworkingSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType networkingSystemType;

        /// <summary>
        /// Diagnostics System Script File to instantiate at runtime
        /// </summary>
        public SystemType NetworkingSystemSystemType
        {
            get => networkingSystemType;
            internal set => networkingSystemType = value;
        }

        #endregion Networking System Properties

        #region Diagnostics System Properties

        [SerializeField]
        [Tooltip("Profile for wiring up diagnostic assets.")]
        private MixedRealityDiagnosticsSystemProfile diagnosticsSystemProfile;

        /// <summary>
        /// Active profile for diagnostic configuration
        /// </summary>
        public MixedRealityDiagnosticsSystemProfile DiagnosticsSystemProfile
        {
            get => diagnosticsSystemProfile;
            internal set => diagnosticsSystemProfile = value;
        }

        [SerializeField]
        [Tooltip("Enable Diagnostic System on Startup.")]
        private bool enableDiagnosticsSystem = false;

        /// <summary>
        /// Is the diagnostics system properly configured and enabled?
        /// </summary>
        public bool IsDiagnosticsSystemEnabled
        {
            get => enableDiagnosticsSystem && DiagnosticsSystemSystemType?.Type != null && diagnosticsSystemProfile != null;
            internal set => enableDiagnosticsSystem = value;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityDiagnosticsSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType diagnosticsSystemType;

        /// <summary>
        /// Diagnostics System Script File to instantiate at runtime
        /// </summary>
        public SystemType DiagnosticsSystemSystemType
        {
            get => diagnosticsSystemType;
            internal set => diagnosticsSystemType = value;
        }

        #endregion Diagnostics System Properties

        #endregion Mixed Reality Toolkit system properties

        [SerializeField]
        [Tooltip("All the additional non-required services registered with the Mixed Reality Toolkit.")]
        private MixedRealityRegisteredServiceProvidersProfile registeredServiceProvidersProfile = null;

        /// <summary>
        /// All the additional non-required systems, features, and managers registered with the Mixed Reality Toolkit.
        /// </summary>
        public MixedRealityRegisteredServiceProvidersProfile RegisteredServiceProvidersProfile
        {
            get => registeredServiceProvidersProfile;
            internal set => registeredServiceProvidersProfile = value;
        }
    }
}
