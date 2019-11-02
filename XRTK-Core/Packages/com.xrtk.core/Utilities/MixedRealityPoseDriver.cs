// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.SpatialTracking;
using XRTK.Services;

namespace XRTK.Definitions.Utilities
{
    public class MixedRealityPoseDriver : TrackedPoseDriver
    {
        private bool headHeightPositionUpdated = false;

        protected override void OnBeforeRender()
        {
            base.OnBeforeRender();

            if (!headHeightPositionUpdated)
            {
                var cameraHeight = MixedRealityToolkit.CameraSystem.CameraRig.PlayerCamera.transform.position.y;
                if (cameraHeight < 0.5f)
                {
                    MixedRealityToolkit.CameraSystem.CameraRig.HeadTransform.position = new Vector3(0, MixedRealityToolkit.CameraSystem.InitialHeadHeight, 0);
                }
                headHeightPositionUpdated = true;
            }
        }
    }
}
