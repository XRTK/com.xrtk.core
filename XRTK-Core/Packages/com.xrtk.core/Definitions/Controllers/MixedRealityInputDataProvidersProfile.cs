// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Definitions
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Controller Input Providers Profile", fileName = "MixedRealityInputDataProvidersProfile", order = (int)CreateProfileMenuItemIndices.InputDataProviders)]
    public class MixedRealityInputDataProvidersProfile : BaseMixedRealityServiceProfile<IMixedRealityInputDataProvider>
    {
    }
}