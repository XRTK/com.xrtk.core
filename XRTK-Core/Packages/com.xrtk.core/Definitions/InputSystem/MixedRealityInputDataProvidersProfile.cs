// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers;

namespace XRTK.Definitions.InputSystem
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Input Data Providers Profile", fileName = "MixedRealityInputDataProvidersProfile", order = (int)CreateProfileMenuItemIndices.InputDataProviders)]
    public class MixedRealityInputDataProvidersProfile : BaseMixedRealityServiceProfile<IMixedRealityInputDataProvider>
    {
    }
}