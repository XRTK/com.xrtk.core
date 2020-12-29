// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XRTK.Definitions;
using XRTK.Interfaces.Authentication;
using XRTK.Utilities.Async;

namespace XRTK.Services.Authentication
{
    /// <summary>
    /// Concrete implementation of the <see cref="IMixedRealityAuthenticationSystem"/>
    /// </summary>
    public class AuthenticationSystem : BaseSystem, IMixedRealityAuthenticationSystem
    {
        /// <inheritdoc />
        public AuthenticationSystem(BaseMixedRealityProfile profile) : base(profile)
        {
        }

        #region IMixedRealityAuthenticationSystem Implementation

        /// <inheritdoc />
        public event Action<IMixedRealityAuthenticationDataProvider, IAuthenticatedAccount> OnLoggedIn;

        /// <inheritdoc />
        public event Action<IMixedRealityAuthenticationDataProvider> OnLoggedOut;

        private List<IAuthenticatedAccount> activeAccounts = new List<IAuthenticatedAccount>();

        /// <inheritdoc />
        public IReadOnlyCollection<IAuthenticatedAccount> ActiveAccounts => activeAccounts;

        /// <inheritdoc />
        public IReadOnlyCollection<IMixedRealityAuthenticationDataProvider> ActiveAuthenticationProviders { get; }

        /// <inheritdoc />
        public async Task LoginAsync(IMixedRealityAuthenticationDataProvider provider)
        {
            await provider.LoginAsync();
            await Awaiters.UnityMainThread;
            activeAccounts.Add(null);
            OnLoggedIn?.Invoke(null, null);
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void LogOutAllSessions()
        {
            activeAccounts.Remove(null);
            OnLoggedOut?.Invoke(null);
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void ClearAllTokenCaches()
        {
            throw new NotImplementedException();
        }

        #endregion IMixedRealityAuthenticationSystem Implementation
    }
}
