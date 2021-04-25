// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces.LocomotionSystem;

namespace XRTK.Services.LocomotionSystem
{
    /// <summary>
    /// Attach this component to any <see cref="GameObject"/> to make that object's
    /// <see cref="Transform"/> the target for locomotion when the component is enabled,
    /// that is the object being translated in space when locomotion occurs.
    ///
    /// When no enabled instance of <see cref="LocomotionTargetOverride"/> is found the
    /// <see cref="MixedRealityLocomotionSystem"/> will target the active
    /// <see cref="Interfaces.CameraSystem.IMixedRealityCameraRig.CameraTransform"/> if the
    /// <see cref="Interfaces.CameraSystem.IMixedRealityCameraSystem"/> is active or fallback to
    /// <see cref="Utilities.CameraCache.Main"/> parent ultimately.
    /// </summary>
    public class LocomotionTargetOverride : MonoBehaviour
    {
        private MixedRealityLocomotionSystem locomotionSystem = null;
        /// <summary>
        /// Gets the currently active <see cref="MixedRealityLocomotionSystem"/> instance.
        /// </summary>
        protected MixedRealityLocomotionSystem LocomotionSystem
            => locomotionSystem ?? (locomotionSystem = MixedRealityToolkit.GetSystem<IMixedRealityLocomotionSystem>() as MixedRealityLocomotionSystem);

        /// <summary>
        /// This method is called just before any of the update methods is
        /// called for the fist time on this behaviour.
        /// </summary>
        private void Start()
        {
            if (LocomotionSystem == null)
            {
                Debug.LogError($"No active {nameof(MixedRealityLocomotionSystem)} found. {nameof(LocomotionTargetOverride)} can only work with the system enabled.");
                Destroy(this);
                return;
            }

            if (LocomotionSystem.LocomotionTargetOverride != null)
            {
                Debug.LogError($"There can only be one instance of {nameof(LocomotionTargetOverride)} in the scene!");
                Destroy(this);
                return;
            }

            LocomotionSystem.LocomotionTargetOverride = this;
        }

        /// <summary>
        /// This method is called when the behaviour will be destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (LocomotionSystem != null)
            {
                LocomotionSystem.LocomotionTargetOverride = null;
            }
        }
    }
}
