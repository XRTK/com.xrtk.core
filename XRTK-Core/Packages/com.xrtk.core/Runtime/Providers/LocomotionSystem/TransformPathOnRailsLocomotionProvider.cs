// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces.LocomotionSystem;
using XRTK.Definitions.LocomotionSystem;

namespace XRTK.Providers.LocomotionSystem
{
    [System.Runtime.InteropServices.Guid("e6a350da-0953-44f7-b02f-8697c0c7e201")]
    public class TransformPathOnRailsLocomotionProvider : BaseLocomotionProvider, IOnRailsLocomotionProvider
    {
        /// <inheritdoc />
        public TransformPathOnRailsLocomotionProvider(string name, uint priority, BaseLocomotionProviderProfile profile, IMixedRealityLocomotionSystem parentService)
            : base(name, priority, profile, parentService) { }
    }
}
