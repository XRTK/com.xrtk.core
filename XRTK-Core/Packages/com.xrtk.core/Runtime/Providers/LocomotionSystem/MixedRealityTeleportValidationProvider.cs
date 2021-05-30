// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Services.LocomotionSystem;
using XRTK.Services;
using XRTK.Utilities;

namespace XRTK.Providers.LocomotionSystem
{
    /// <summary>
    /// The Mixed Reality Toolkit's specific implementation of the <see cref="IMixedRealityTeleportValidationProvider"/>.
    /// </summary>
    [System.Runtime.InteropServices.Guid("14199fd8-1636-4147-bb08-6475e76ed1cd")]
    public class MixedRealityTeleportValidationProvider : BaseDataProvider, IMixedRealityLocomotionDataProvider
    {
        /// <inheritdoc />
        public MixedRealityTeleportValidationProvider(string name, uint priority, MixedRealityTeleportValidationProviderProfile profile, IMixedRealityLocomotionSystem parentService)
            : base(name, priority, profile, parentService)
        {
            hotSpotsOnly = profile.HotSpotsOnly;
            validLayers = profile.ValidLayers;
            invalidLayers = profile.InvalidLayers;
            upDirectionThreshold = profile.UpDirectionThreshold;
            maxDistanceSquare = profile.MaxDistance * profile.MaxDistance;
            maxHeightDistance = profile.MaxHeightDistance;
        }

        private readonly bool hotSpotsOnly;
        private readonly LayerMask validLayers;
        private readonly LayerMask invalidLayers;
        private readonly float upDirectionThreshold;
        private readonly float maxDistanceSquare;
        private readonly float maxHeightDistance;

        /// <inheritdoc />
        public TeleportValidationResult IsValid(IPointerResult pointerResult, IMixedRealityTeleportHotSpot teleportHotSpot = null)
        {
            TeleportValidationResult teleportValidationResult = TeleportValidationResult.Valid;

            // Check hotspots only.
            if (hotSpotsOnly && (teleportHotSpot == null || !teleportHotSpot.IsActive))
            {
                teleportValidationResult = TeleportValidationResult.Invalid;
            }
            // Check distance.
            else if ((pointerResult.EndPoint - CameraCache.Main.transform.position).sqrMagnitude > maxDistanceSquare ||
                Mathf.Abs(pointerResult.EndPoint.y - CameraCache.Main.transform.position.y) > maxHeightDistance)
            {
                teleportValidationResult = TeleportValidationResult.Invalid;
            }
            // Check if it's in valid layers.
            else if (((1 << pointerResult.CurrentPointerTarget.layer) & validLayers.value) == 0)
            {
                teleportValidationResult = TeleportValidationResult.Invalid;
            }
            // Check if it's in invalid layers.
            else if (((1 << pointerResult.CurrentPointerTarget.layer) & invalidLayers) != 0)
            {
                teleportValidationResult = TeleportValidationResult.Invalid;
            }
            // If it's NOT a hotspot, check if the hit normal is too steep (Hotspots override dot requirements).
            else if (teleportHotSpot == null || !teleportHotSpot.IsActive)
            {
                teleportValidationResult = Vector3.Dot(pointerResult.LastRaycastHit.normal, Vector3.up) > upDirectionThreshold
                    ? TeleportValidationResult.Valid
                    : TeleportValidationResult.Invalid;
            }

            return teleportValidationResult;
        }
    }
}
