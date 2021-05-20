// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Net;
using XRTK.Definitions.NetworkingSystem;
using XRTK.Interfaces.NetworkingSystem;

namespace XRTK.Services.NetworkingSystem
{
    /// <summary>
    /// The Mixed Reality Toolkit's default implementation of the <see cref="IMixedRealityNetworkingSystem"/>
    /// </summary>
    [System.Runtime.InteropServices.Guid("6CB7110B-058B-4AA6-B81D-792FE443191A")]
    public class MixedRealityNetworkingSystem : BaseEventSystem, IMixedRealityNetworkingSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile"></param>
        public MixedRealityNetworkingSystem(MixedRealityNetworkSystemProfile profile)
            : base(profile)
        {
        }

        #region IMixedRealityNetworkingSystem Implementation

        /// <inheritdoc />
        public HashSet<IMixedRealityNetworkDataProvider> NetworkDataProviders { get; } = new HashSet<IMixedRealityNetworkDataProvider>();

        /// <inheritdoc />
        public bool HasInternetConnection
        {
            get
            {
                try
                {
                    using (new WebClient().OpenRead("https://www.google.com"))
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <inheritdoc />
        public uint RequestNetworkDataProviderSourceId()
        {
            return 0;
        }

        /// <inheritdoc />
        public void RaiseNetworkDataProviderDetected(IMixedRealityNetworkDataProvider networkDataProvider)
        {
        }

        /// <inheritdoc />
        public void RaiseNetworkDataProviderLost(IMixedRealityNetworkDataProvider networkDataProvider)
        {
        }

        /// <inheritdoc />
        public void SendData<T>(T data)
        {
            // Notes: we can mix and match or do some special routing here if we wanted.
            foreach (var networkDataProvider in NetworkDataProviders)
            {
                networkDataProvider.SendData(data);
            }
        }

        /// <inheritdoc />
        public void RaiseDataReceived<T>(T data)
        {
            // TODO forward to all the registered listeners.
        }

        #endregion IMixedRealityNetworkingSystem Implementation
    }
}
