// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.Devices;

namespace XRTK.Providers.Controllers
{
    /// <summary>
    /// Base Controller Mapping profile.
    /// </summary>
    public abstract class BaseMixedRealityControllerMappingProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private bool hasSetupDefaults = false;

        /// <summary>
        /// Has this controller profile setup the default interactions?
        /// </summary>
        protected bool HasSetupDefaults => hasSetupDefaults;

        /// <summary>
        /// The supported controller type for this profile.
        /// </summary>
        public virtual SupportedControllerType ControllerType => SupportedControllerType.GenericUnity;

        [SerializeField]
        private MixedRealityControllerMapping[] controllerMappings;

        /// <summary>
        /// The controller mappings for this device.
        /// </summary>
        public MixedRealityControllerMapping[] ControllerMappings
        {
            get => controllerMappings;
            protected set => controllerMappings = value;
        }

        /// <summary>
        /// The partial path to the controller's texture. This path is meaningless outside of the context of the editor.
        /// </summary>
        /// <remarks>
        /// The path only needs to include the name of the controller.<para/>
        /// The file name should be formatted as:<para/>XRTK/StandardAssets/Textures/{ControllerName}_{handedness}_{theme}_{scaled}.png<para/>
        /// scaled suffix is optional.<para/>
        /// Custom file paths should never call the base property.
        /// </remarks>
        public virtual string TexturePath
        {
            get
            {
                // this path is meaningless outside of the editor.
#if UNITY_EDITOR
                return $"{Utilities.Editor.MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/StandardAssets/Textures/";
#else
                return string.Empty;
#endif
            }
        }

        protected virtual void Awake()
        {
            if (!hasSetupDefaults)
            {
                for (int i = 0; i < controllerMappings?.Length; i++)
                {
                    controllerMappings[i].SetDefaultInteractionMapping();
                }

                hasSetupDefaults = true;
            }
        }
    }
}