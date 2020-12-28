// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using XRTK.Definitions.Authentication;
using XRTK.Interfaces.Authentication;
using XRTK.Services;
using XRTK.Utilities.Async;

namespace XRTK.Providers.Authentication
{
    public class Microsoft_MSAL_AuthenticationDataProvider : BaseDataProvider, IMixedRealityAuthenticationDataProvider
    {
        /// <inheritdoc />
        public Microsoft_MSAL_AuthenticationDataProvider(string name, uint priority, AuthenticationDataProviderProfile profile, IMixedRealityAuthenticationSystem parentService)
            : base(name, priority, profile, parentService)
        {
        }

        #region IMixedRealityAuthenticationDataProvider Implementation

        /// <inheritdoc />
        public bool IsUserLoggedIn { get; } = false;

        /// <inheritdoc />
        public async Task LoginAsync()
        {
            await Awaiters.UnityMainThread;
        }

        /// <inheritdoc />
        public void Logout(bool reAuthenticate = true)
        {
        }

        #endregion IMixedRealityAuthenticationDataProvider Implementation
    }
}