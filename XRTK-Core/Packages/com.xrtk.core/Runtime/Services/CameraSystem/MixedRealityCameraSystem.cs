// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.CameraSystem;
using XRTK.Interfaces.CameraSystem;
using XRTK.Utilities;

namespace XRTK.Services.CameraSystem
{
    /// <summary>
    /// The Mixed Reality Toolkit's default implementation of the <see cref="IMixedRealityCameraSystem"/>.
    /// </summary>
    [System.Runtime.InteropServices.Guid("5C656EE3-FE7C-4FB3-B3EE-DF3FC0D0973D")]
    public class MixedRealityCameraSystem : BaseSystem, IMixedRealityCameraSystem
    {
        /// <inheritdoc />
        public MixedRealityCameraSystem(MixedRealityCameraSystemProfile profile)
            : base(profile)
        {
        }

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override uint Priority => 0;

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            Debug.Assert(cameraDataProviders.Count == 0, $"Failed to clean up {cameraDataProviders}!");
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealityCameraSystem Impelementation

        private readonly HashSet<IMixedRealityCameraDataProvider> cameraDataProviders = new HashSet<IMixedRealityCameraDataProvider>();

        /// <inheritdoc />
        public IReadOnlyCollection<IMixedRealityCameraDataProvider> CameraDataProviders => cameraDataProviders;

        private IMixedRealityCameraRig mainCameraRig;

        /// <inheritdoc />
        public IMixedRealityCameraRig MainCameraRig
        {
            get
            {
                if (mainCameraRig == null)
                {
                    // for now we always expect the rig to be on the transform root of the main camera with tag "mainCamera".
                    Debug.Assert(CameraCache.Main.transform.root != null, "The main camera has no ancestors! A valid XRRig needs to be setup.");
                    mainCameraRig = CameraCache.Main.transform.root.GetComponent<IMixedRealityCameraRig>();
                    Debug.Assert(mainCameraRig != null, $"Failed to find a valid {nameof(IMixedRealityCameraRig)} on the root of the main camera!");
                }

                return mainCameraRig;
            }
        }

        /// <inheritdoc />
        public void RegisterCameraDataProvider(IMixedRealityCameraDataProvider dataProvider)
        {
            cameraDataProviders.Add(dataProvider);
        }

        /// <inheritdoc />
        public void UnRegisterCameraDataProvider(IMixedRealityCameraDataProvider dataProvider)
        {
            cameraDataProviders.Remove(dataProvider);
        }

        #endregion IMixedRealityCameraSystem Impelementation
    }
}
