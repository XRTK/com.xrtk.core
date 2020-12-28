// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions;
using XRTK.Interfaces.Authentication;

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
    }
}
