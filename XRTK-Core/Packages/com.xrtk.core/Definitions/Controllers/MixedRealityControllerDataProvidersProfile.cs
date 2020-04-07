// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Definitions.Controllers
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Data Providers Profile", fileName = "MixedRealityControllerDataProvidersProfile", order = (int)CreateProfileMenuItemIndices.ControllerDataProviders)]
    public class MixedRealityControllerDataProvidersProfile : BaseMixedRealityServiceProfile<IMixedRealityControllerDataProvider>
    {
    }
}