// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XRTK.Definitions.Authentication;

namespace XRTK.Interfaces.Authentication
{
    /// <summary>
    /// Provider agnostic Interface contract for user authentication with <see href="https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-acquire-cache-tokens#public-client-applications">public client applications</see>.
    /// </summary>
    public interface IMixedRealityAuthenticationSystem : IMixedRealitySystem
    {
        /// <summary>
        /// Signals when the user has been logged in
        /// </summary>
        event Action<IMixedRealityAuthenticationDataProvider, IAuthenticatedAccount> OnLoggedIn;

        /// <summary>
        /// Signals when the user has been logged out.
        /// </summary>
        event Action<IMixedRealityAuthenticationDataProvider> OnLoggedOut;

        /// <summary>
        /// Gets the currently logged in <see cref="IAuthenticatedAccount"/>s.
        /// </summary>
        IReadOnlyCollection<IAuthenticatedAccount> ActiveAccounts { get; }

        /// <summary>
        /// All of the currently active <see cref="IMixedRealityAuthenticationDataProvider"/>s.
        /// </summary>
        IReadOnlyCollection<IMixedRealityAuthenticationDataProvider> ActiveAuthenticationProviders { get; }

        /// <summary>
        /// Start Login task.
        /// </summary>
        /// <param name="provider">The <see cref="IMixedRealityAuthenticationDataProvider"/> to login with.</param>
        /// <remarks>
        /// This may prompt the user to authenticate with the <see cref="AuthenticationDataProviderProfile.LoginPrefab"/>
        /// </remarks>
        /// <returns>Completed <see cref="Task"/>.</returns>
        Task LoginAsync(IMixedRealityAuthenticationDataProvider provider);

        /// <summary>
        /// Logs out of all active sessions in all active <see cref="IMixedRealityAuthenticationDataProvider"/>s.
        /// </summary>
        void LogOutAllSessions();

        /// <summary>
        /// Clears out all of the token caches for all active <see cref="IMixedRealityAuthenticationDataProvider"/>s.
        /// </summary>
        void ClearAllTokenCaches();
    }
}
