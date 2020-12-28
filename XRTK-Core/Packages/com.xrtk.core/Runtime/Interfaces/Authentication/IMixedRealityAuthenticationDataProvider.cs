// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;

namespace XRTK.Interfaces.Authentication
{
    /// <summary>
    /// Interface contract for specific identity provider implementations for use in the <see cref="IMixedRealityAuthenticationSystem"/>.
    /// </summary>
    public interface IMixedRealityAuthenticationDataProvider : IMixedRealityDataProvider
    {
        /// <summary>
        /// Is there currently a valid user logged in with a valid token?
        /// </summary>
        bool IsUserLoggedIn { get; }

        /// <summary>
        /// Start Login task.
        /// </summary>
        /// <returns>Completed <see cref="Task"/>.</returns>
        Task LoginAsync();

        /// <summary>
        /// Log the user out.
        /// </summary>
        /// <param name="reAuthenticate"></param>
        void Logout(bool reAuthenticate = true);
    }
}
