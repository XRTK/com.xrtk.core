// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.Hands;

namespace XRTK.Definitions.Controllers.Hands
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Data Providers/Hands/Hand Controller Data Provider Profile", fileName = "MixedRealityHandControllerDataProviderProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class HandControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        [SerializeField]
        [Tooltip("The concrete type to use for created hand controllers.")]
        [Extends(typeof(BaseHandController), TypeGrouping.ByNamespaceFlat)]
        private SystemType handControllerType = null;

        /// <summary>
        /// The concrete type to use for created hand controllers.
        /// </summary>
        public SystemType HandControllerType => handControllerType;
    }
}
