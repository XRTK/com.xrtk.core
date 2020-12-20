// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.TeleportSystem;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.TeleportSystem;
using XRTK.Utilities;

namespace XRTK.Services.Teleportation
{
    /// <summary>
    /// The Mixed Reality Toolkit's specific implementation of the <see cref="IMixedRealityTeleportValidationDataProvider"/>.
    /// </summary>
    [System.Runtime.InteropServices.Guid("14199fd8-1636-4147-bb08-6475e76ed1cd")]
    public class MixedRealityTeleportValidationDataProvider : BaseDataProvider, IMixedRealityTeleportValidationDataProvider
    {
        /// <inheritdoc />
        public MixedRealityTeleportValidationDataProvider(string name, uint priority, MixedRealityTeleportValidationDataProviderProfile profile, IMixedRealityTeleportSystem parentService)
            : base(name, priority, profile, parentService)
        {
            validLayers = profile.ValidLayers;
            invalidLayers = profile.InvalidLayers;
            upDirectionThreshold = profile.UpDirectionThreshold;
            maxDistanceSquare = profile.MaxDistance * profile.MaxDistance;
        }

        private readonly LayerMask validLayers;
        private readonly LayerMask invalidLayers;
        private readonly float upDirectionThreshold;
        private readonly float maxDistanceSquare;

        /// <inheritdoc />
        public TeleportValidationResult IsValid(IPointerResult pointerResult, IMixedRealityTeleportHotSpot teleportHotSpot = null)
        {
            TeleportValidationResult teleportValidationResult;

            // Check distance.
            if ((pointerResult.EndPoint - CameraCache.Main.transform.position).sqrMagnitude > maxDistanceSquare)
            {
                teleportValidationResult = TeleportValidationResult.Invalid;
            }
            // Check if it's in our valid layers
            else if (((1 << pointerResult.CurrentPointerTarget.layer) & validLayers.value) != 0)
            {
                // See if it's a hot spot
                if (teleportHotSpot != null && teleportHotSpot.IsActive)
                {
                    teleportValidationResult = TeleportValidationResult.HotSpot;
                }
                else
                {
                    // If it's NOT a hotspot, check if the hit normal is too steep 
                    // (Hotspots override dot requirements)
                    teleportValidationResult = Vector3.Dot(pointerResult.LastRaycastHit.normal, Vector3.up) > upDirectionThreshold
                        ? TeleportValidationResult.Valid
                        : TeleportValidationResult.Invalid;
                }
            }
            else if (((1 << pointerResult.CurrentPointerTarget.layer) & invalidLayers) != 0)
            {
                teleportValidationResult = TeleportValidationResult.Invalid;
            }
            else
            {
                teleportValidationResult = TeleportValidationResult.None;
            }

            return teleportValidationResult;
        }
    }
}
