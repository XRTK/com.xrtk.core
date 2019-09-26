// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Definitions.Controllers;
using XRTK.Services;

namespace XRTK.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up controller pointers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System Profile", fileName = "MixedRealityInputSystemProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class MixedRealityInputSystemProfile : BaseMixedRealityProfile
    {
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
        [Tooltip("Input System Action Mapping profile for wiring up Controller input to Actions.")]
        private MixedRealityInputActionsProfile inputActionsProfile;

        /// <summary>
        /// Input System Action Mapping profile for wiring up Controller input to Actions.
        /// </summary>
        public MixedRealityInputActionsProfile InputActionsProfile
        {
            get => inputActionsProfile;
            internal set => inputActionsProfile = value;
        }

        [SerializeField]
        [Tooltip("Pointer Configuration options")]
        private MixedRealityPointerProfile pointerProfile;

        /// <summary>
        /// Pointer configuration options
        /// </summary>
        public MixedRealityPointerProfile PointerProfile
        {
            get => pointerProfile;
            internal set => pointerProfile = value;
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

        /// <summary>
        /// Is the speech Commands Enabled?
        /// </summary>
        public bool IsSpeechCommandsEnabled => speechCommandsProfile != null && SpeechDataProvider != null && MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled;

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

        /// <summary>
        /// Is Dictation Enabled?
        /// </summary>
        public bool IsDictationEnabled => MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled && DictationDataProvider != null;

        [SerializeField]
        [Tooltip("Device profile for registering platform specific input data sources.")]
        private MixedRealityControllerDataProvidersProfile controllerDataProvidersProfile;

        /// <summary>
        /// Active profile for controller mapping configuration
        /// </summary>
        public MixedRealityControllerDataProvidersProfile ControllerDataProvidersProfile
        {
            get => controllerDataProvidersProfile;
            internal set => controllerDataProvidersProfile = value;
        }

        [SerializeField]
        private MixedRealityControllerMappingProfiles controllerMappingProfiles;

        /// <summary>
        /// The profile for setting up registered data provider controller mappings.
        /// </summary>
        public MixedRealityControllerMappingProfiles ControllerMappingProfiles
        {
            get => controllerMappingProfiles;
            internal set => controllerMappingProfiles = value;
        }

        [SerializeField]
        [Tooltip("Device profile for rendering spatial controllers.")]
        private MixedRealityControllerVisualizationProfile controllerVisualizationProfile;

        /// <summary>
        /// Device profile for rendering spatial controllers.
        /// </summary>
        public MixedRealityControllerVisualizationProfile ControllerVisualizationProfile
        {
            get => controllerVisualizationProfile;
            internal set => controllerVisualizationProfile = value;
        }

        private IMixedRealityFocusProvider focusProvider;

        /// <summary>
        /// Current Registered <see cref="IMixedRealityFocusProvider"/>.
        /// </summary>
        public IMixedRealityFocusProvider FocusProvider => focusProvider ?? (focusProvider = MixedRealityToolkit.GetService<IMixedRealityFocusProvider>());

        private IMixedRealitySpeechDataProvider speechDataProvider;

        /// <summary>
        /// Current Registered <see cref="IMixedRealitySpeechDataProvider"/>
        /// </summary>
        public IMixedRealitySpeechDataProvider SpeechDataProvider => speechDataProvider ?? (speechDataProvider = MixedRealityToolkit.GetService<IMixedRealitySpeechDataProvider>());

        private IMixedRealityDictationDataProvider dictationDataProvider;

        /// <summary>
        /// Current Registered <see cref="IMixedRealityDictationDataProvider"/>.
        /// </summary>
        public IMixedRealityDictationDataProvider DictationDataProvider => dictationDataProvider ?? (dictationDataProvider = MixedRealityToolkit.GetService<IMixedRealityDictationDataProvider>());
    }
}