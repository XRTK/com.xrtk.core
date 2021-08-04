// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces;

namespace XRTK.Definitions
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Base Service Profile", fileName = "MixedRealityServiceProfile", order = (int)CreateProfileMenuItemIndices.RegisteredServiceProviders)]
    public class MixedRealityServiceProfile : BaseMixedRealityServiceProfile<IMixedRealityService>
    { }
}
