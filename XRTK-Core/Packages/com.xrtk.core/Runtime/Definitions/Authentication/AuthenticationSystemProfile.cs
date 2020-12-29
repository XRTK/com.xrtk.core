// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces.Authentication;

namespace XRTK.Definitions.Authentication
{
    public class AuthenticationSystemProfile : BaseMixedRealityServiceProfile<IMixedRealityAuthenticationDataProvider>
    {
        [SerializeField]
        private bool cacheUserTokens = true;

        private bool CacheUserTokens => cacheUserTokens;
    }
}
