// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Definitions.LocomotionSystem
{
    /// <summary>
    /// Configuration profile settings for <see cref="Services.LocomotionSystem.MixedRealityLocomotionSystem"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Locomotion System Profile", fileName = "MixedRealityLocomotionSystemProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class MixedRealityLocomotionSystemProfile : BaseMixedRealityServiceProfile<IMixedRealityLocomotionDataProvider>
    {

    }
}