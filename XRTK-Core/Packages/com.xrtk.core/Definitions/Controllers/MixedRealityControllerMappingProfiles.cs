// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers;

namespace XRTK.Definitions.Controllers
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Mapping Profiles", fileName = "MixedRealityControllerMappingProfiles", order = (int)CreateProfileMenuItemIndices.ControllerMappings)]
    public class MixedRealityControllerMappingProfiles : BaseMixedRealityProfile
    {
        [SerializeField]
        private List<BaseMixedRealityControllerMappingProfile> controllerMappingProfiles = new List<BaseMixedRealityControllerMappingProfile>();

        /// <summary>
        /// A list of all the Controller Mapping Profiles.
        /// </summary>
        public List<BaseMixedRealityControllerMappingProfile> ControllerMappingProfiles
        {
            get => controllerMappingProfiles;
            internal set => controllerMappingProfiles = value;
        }

        /// <summary>
        /// The list of controller mappings across all <see cref="ControllerMappingProfiles"/>.
        /// </summary>
        public List<MixedRealityControllerMapping> MixedRealityControllerMappings
        {
            get
            {
                var mappings = new List<MixedRealityControllerMapping>();

                if (controllerMappingProfiles != null)
                {
                    for (int i = 0; i < controllerMappingProfiles.Count; i++)
                    {
                        if (controllerMappingProfiles[i] != null)
                        {
                            for (int j = 0; j < controllerMappingProfiles[i].ControllerMappings?.Length; j++)
                            {

                                mappings.Add(controllerMappingProfiles[i].ControllerMappings[j]);
                            }
                        }
                    }
                }

                return mappings;
            }
        }
    }
}