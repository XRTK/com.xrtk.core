// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.NetworkingSystem
{
    /// <summary>
    /// Configuration profile settings for setting up networking.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Network System Profile", fileName = "MixedRealityNetworkSystemProfile", order = (int)CreateProfileMenuItemIndices.Networking)]
    public class MixedRealityNetworkSystemProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private NetworkDataProviderConfiguration[] registeredNetworkDataProviders = new NetworkDataProviderConfiguration[0];

        /// <summary>
        /// The list of registered network data providers.
        /// </summary>
        public NetworkDataProviderConfiguration[] RegisteredNetworkDataProviders => registeredNetworkDataProviders;
    }
}