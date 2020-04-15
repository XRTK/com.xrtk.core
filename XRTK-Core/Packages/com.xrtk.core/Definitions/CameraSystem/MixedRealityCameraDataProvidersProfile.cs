// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.CameraSystem;

namespace XRTK.Definitions.CameraSystem
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Camera System/Data Providers Profile", fileName = "MixedRealityCameraDataProvidersProfile", order = (int)CreateProfileMenuItemIndices.Camera)]
    public class MixedRealityCameraDataProvidersProfile : BaseMixedRealityServiceProfile<IMixedRealityCameraDataProvider>
    {
    }
}