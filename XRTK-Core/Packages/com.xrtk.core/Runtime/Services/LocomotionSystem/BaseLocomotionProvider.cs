// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Extensions;
using XRTK.Interfaces.CameraSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Utilities;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Base implementation for locomotion providers working with the <see cref="MixedRealityLocomotionSystem"/>.
    /// </summary>
    public abstract class BaseLocomotionProvider : MonoBehaviour
    {
        private MixedRealityLocomotionSystem locomotionSystem = null;
        /// <summary>
        /// Gets the currently active <see cref="MixedRealityLocomotionSystem"/> instance.
        /// </summary>
        protected MixedRealityLocomotionSystem LocomotionSystem
            => locomotionSystem ?? (locomotionSystem = MixedRealityToolkit.GetSystem<IMixedRealityLocomotionSystem>() as MixedRealityLocomotionSystem);

        /// <summary>
        /// Gets the player camera <see cref="Transform"/>.
        /// </summary>
        protected virtual Transform CameraTransform
        {
            get
            {
                return MixedRealityToolkit.TryGetSystem<IMixedRealityCameraSystem>(out var cameraSystem)
                    ? cameraSystem.MainCameraRig.CameraTransform
                    : CameraCache.Main.transform;
            }
        }

        /// <summary>
        /// Gets the target <see cref="Transform"/> for locomotion.
        /// </summary>
        protected virtual Transform LocomotionTargetTransform
        {
            get
            {
                if (LocomotionSystem.LocomotionTargetOverride.IsNull() ||
                    !LocomotionSystem.LocomotionTargetOverride.enabled)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.Assert(!CameraTransform.parent.IsNull(), $"The {nameof(MixedRealityLocomotionSystem)} expects the camera to be parented under another transform!");
                    }

                    return CameraTransform.parent;
                }

                return LocomotionSystem.LocomotionTargetOverride.transform;
            }
        }
    }
}
