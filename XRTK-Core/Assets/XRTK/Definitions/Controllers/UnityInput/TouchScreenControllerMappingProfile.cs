// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;

namespace XRTK.DataProviders.Controllers.UnityInput.Profiles
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Mappings/Touch Screen Mapping Profile", fileName = "TouchScreenMappingProfile")]
    public class TouchScreenControllerMappingProfile : BaseMixedRealityControllerMappingProfile
    {
        /// <inheritdoc />
        public override SupportedControllerType ControllerType => SupportedControllerType.TouchScreen;

        protected override void Awake()
        {
            if (!HasSetupDefaults)
            {
                ControllerMappings = new[]
                {
                    new MixedRealityControllerMapping("Touch Screen Input", typeof(UnityTouchController), Handedness.Any)
                };
            }

            base.Awake();
        }
    }
}