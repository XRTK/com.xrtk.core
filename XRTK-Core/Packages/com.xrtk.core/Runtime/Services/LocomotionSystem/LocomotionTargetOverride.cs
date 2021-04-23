// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Attach this component to any <see cref="GameObject"/> to make that object
    /// the target for locomotion, that is the object being translated in space when
    /// locomotion occurs. By default the <see cref="MixedRealityLocomotionSystem"/>
    /// will target the active <see cref="Interfaces.CameraSystem.IMixedRealityCameraRig.GameObject"/>.
    /// </summary>
    public class LocomotionTargetOverride : MonoBehaviour { }
}
