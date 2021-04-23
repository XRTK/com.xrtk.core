// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;

namespace XRTK.Editor.Utilities
{
    /// <summary>
    /// Helper utility for managing controller mappings in the editor.
    /// </summary>
    public static class ControllerMappingUtilities
    {
        #region InputAxisConfig

        // Default value for the dead zone. This should match the default used by Unity for the pre-created Horizontal and Vertical axes.
        private const float DEFAULT_DEAD_ZONE = 0.19f;

        /// <summary>
        /// Get the InputManagerAxis data needed to configure the Input Mappings for a controller
        /// </summary>
        /// <returns></returns>
        public static InputManagerAxis[] UnityInputManagerAxes => new[]
        {
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_1,  Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 1  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_2,  Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 2  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_3,  Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 3  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_4,  Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 4  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_5,  Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 5  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_6,  Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 6  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_7,  Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 7  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_8,  Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 8  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_9,  Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 9  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_10, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 10 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_11, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 11 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_12, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 12 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_13, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 13 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_14, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 14 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_15, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 15 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_16, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 16 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_17, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 17 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_18, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 18 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_19, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 19 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_20, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 20 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_21, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 21 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_22, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 22 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_23, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 23 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_24, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 24 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_25, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 25 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_26, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 26 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_27, Dead = DEFAULT_DEAD_ZONE, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 27 }
        };

        #endregion InputAxisConfig

        #region Controller Texture Loading

        private static readonly Dictionary<Tuple<Type, Handedness, bool>, Texture2D> CachedTextures = new Dictionary<Tuple<Type, Handedness, bool>, Texture2D>();

        /// <summary>
        /// Gets a texture based on the <see cref="MixedRealityControllerMappingProfile"/>.
        /// </summary>
        /// <param name="mappingProfile"></param>
        /// <returns>The texture for the controller profile, if none found then a generic texture is returned.</returns>
        /// <remarks>
        /// The file name should be formatted as:<para/>XRTK/StandardAssets/Textures/{ControllerTypeName}_{handedness}_{theme}_{scaled}.png<para/>
        /// scaled suffix is optional.<para/>
        /// </remarks>
        public static Texture2D GetControllerTexture(MixedRealityControllerMappingProfile mappingProfile)
        {
            return GetControllerTextureCached(mappingProfile);
        }

        /// <summary>
        /// Gets a texture based on the <see cref="MixedRealityControllerMappingProfile"/>.
        /// </summary>
        /// <param name="mappingProfile"></param>
        /// <returns>The scaled texture for the controller profile, if none found then a generic texture is returned.</returns>
        /// <remarks>
        /// The file name should be formatted as:<para/>XRTK/StandardAssets/Textures/{ControllerName}_{handedness}_{theme}_{scaled}.png<para/>
        /// </remarks>
        public static Texture2D GetControllerTextureScaled(MixedRealityControllerMappingProfile mappingProfile)
        {
            return GetControllerTextureCached(mappingProfile, true);
        }

        private static Texture2D GetControllerTextureCached(MixedRealityControllerMappingProfile mappingProfile, bool scaled = false)
        {
            var key = new Tuple<Type, Handedness, bool>(mappingProfile.ControllerType.Type, mappingProfile.Handedness, scaled);

            if (CachedTextures.TryGetValue(key, out var texture))
            {
                return texture;
            }

            texture = GetControllerTextureInternal(mappingProfile, scaled);
            CachedTextures.Add(key, texture);
            return texture;
        }

        private static readonly string RootTexturePath = $"{PathFinderUtility.XRTK_Core_RelativeFolderPath}/Runtime/StandardAssets/Textures/";

        private static Texture2D GetControllerTextureInternal(MixedRealityControllerMappingProfile mappingProfile, bool scaled)
        {
            Texture2D texture = null;

            if (mappingProfile != null &&
                mappingProfile.ControllerType.Type != null)
            {
                var controllerName = mappingProfile.ControllerType.Type.Name.Replace("OpenVR", string.Empty);
                controllerName = controllerName.Replace("Simulated", string.Empty);
                texture = GetControllerTextureInternal($"{RootTexturePath}{controllerName}", mappingProfile.Handedness, scaled);

                if (texture != null)
                {
                    return texture;
                }

                texture = GetControllerTextureInternal($"{RootTexturePath}{controllerName}", Handedness.None, scaled);
            }

            if (texture == null)
            {
                texture = GetControllerTextureInternal($"{RootTexturePath}Generic_controller", mappingProfile.Handedness, scaled);
            }

            if (texture == null)
            {
                texture = GetControllerTextureInternal($"{RootTexturePath}Generic_controller", Handedness.Right, scaled);
            }

            return texture;
        }

        private static Texture2D GetControllerTextureInternal(string fullTexturePath, Handedness handedness, bool scaled)
        {
            var handednessSuffix = string.Empty;

            switch (handedness)
            {
                case Handedness.Left:
                    handednessSuffix = "_left";
                    break;
                case Handedness.Right:
                    handednessSuffix = "_right";
                    break;
            }

            var themeSuffix = EditorGUIUtility.isProSkin ? "_white" : "_black";

            return (Texture2D)AssetDatabase.LoadAssetAtPath($"{fullTexturePath}{handednessSuffix}{themeSuffix}{(scaled ? "_scaled" : string.Empty)}.png", typeof(Texture2D));
        }

        #endregion Controller Texture Loading
    }
}