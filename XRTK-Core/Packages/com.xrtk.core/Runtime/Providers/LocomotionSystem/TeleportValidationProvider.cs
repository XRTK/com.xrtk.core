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
    /// The Mixed Reality Toolkit's specific implementation of the <see cref="ITeleportValidationProvider"/>.
    /// </summary>
    [System.Runtime.InteropServices.Guid("14199fd8-1636-4147-bb08-6475e76ed1cd")]
    public class TeleportValidationProvider : BaseDataProvider, ITeleportValidationProvider
    {
        /// <inheritdoc />
        public TeleportValidationProvider(string name, uint priority, TeleportValidationProviderProfile profile, ILocomotionSystem parentService)
            : base(name, priority, profile, parentService)
        {
            anchorsOnly = profile.AnchorsOnly;
            validLayers = profile.ValidLayers;
            invalidLayers = profile.InvalidLayers;
            upDirectionThreshold = profile.UpDirectionThreshold;
            maxDistanceSquare = profile.MaxDistance * profile.MaxDistance;
            maxHeightDistance = profile.MaxHeightDistance;
        }

        private readonly bool anchorsOnly;
        private readonly LayerMask validLayers;
        private readonly LayerMask invalidLayers;
        private readonly float upDirectionThreshold;
        private readonly float maxDistanceSquare;
        private readonly float maxHeightDistance;

        /// <inheritdoc />
        public TeleportValidationResult IsValid(IPointerResult pointerResult, ITeleportAnchor anchor = null)
        {
            TeleportValidationResult teleportValidationResult;

            // Check distance.
            if ((pointerResult.EndPoint - CameraCache.Main.transform.position).sqrMagnitude > maxDistanceSquare ||
                Mathf.Abs(pointerResult.EndPoint.y - CameraCache.Main.transform.position.y) > maxHeightDistance)
            {
                teleportValidationResult = TeleportValidationResult.Invalid;
            }
            // Check anchors only.
            else if (anchorsOnly && (anchor == null || !anchor.IsActive))
            {
                teleportValidationResult = TeleportValidationResult.Invalid;
            }
            // Check if it's in our valid layers
            else if (((1 << pointerResult.CurrentPointerTarget.layer) & validLayers.value) != 0)
            {
                // See if it's a hot spot
                if (anchor != null && anchor.IsActive)
                {
                    teleportValidationResult = TeleportValidationResult.Anchor;
                }
                else
                {
                    // If it's NOT an anchor, check if the hit normal is too steep 
                    // (Anchors override dot requirements)
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
