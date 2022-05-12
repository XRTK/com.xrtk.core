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

            Debug.Assert(cameraDataProviders.Count == 0, "Failed to clean up camera data provider references!");
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealityCameraSystem Impelementation

        private readonly HashSet<IMixedRealityCameraDataProvider> cameraDataProviders = new HashSet<IMixedRealityCameraDataProvider>();

        /// <inheritdoc />
        public IReadOnlyCollection<IMixedRealityCameraDataProvider> CameraDataProviders => cameraDataProviders;

        private IMixedRealityCameraRig mainCameraRig = null;

        /// <inheritdoc />
        public IMixedRealityCameraRig MainCameraRig
        {
            get
            {
                if (mainCameraRig == null)
                {
                    foreach (var dataProvider in cameraDataProviders)
                    {
                        if (dataProvider.CameraRig.PlayerCamera == CameraCache.Main)
                        {
                            mainCameraRig = dataProvider.CameraRig;
                        }
                    }
                }

                return mainCameraRig;
            }
        }

        /// <inheritdoc />
        public TrackingType TrackingType
        {
            get
            {
                foreach (var dataProvider in cameraDataProviders)
                {
                    if (dataProvider.CameraRig.PlayerCamera == CameraCache.Main)
                    {
                        return dataProvider.TrackingType;
                    }
                }

                // If we can't find the active camera data provider we must rely
                // on whatever the platform default is.
                return TrackingType.Auto;
            }
        }

#if XRTK_USE_LEGACYVR
        /// <inheritdoc />
        public void SetHeadHeight(float value, bool setForAllCameraProviders = false)
        {
            foreach (var dataProvider in cameraDataProviders)
            {
                if (setForAllCameraProviders ||
                    dataProvider.CameraRig == MainCameraRig)
                {
                    dataProvider.HeadHeight = value;

                    if (!setForAllCameraProviders)
                    {
                        break;
                    }
                }
            }
        }
#endif

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
