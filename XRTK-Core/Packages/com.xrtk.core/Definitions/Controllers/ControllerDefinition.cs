// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers
{
    public struct ControllerDefinition
    {
        public ControllerDefinition(Type controllerType, Handedness handedness, bool useCustomInteractions) : this()
        {
            ControllerType = controllerType;
            Handedness = handedness;
            UseCustomInteractions = useCustomInteractions;
        }

        public readonly Type ControllerType;

        public readonly Handedness Handedness;

        public readonly bool UseCustomInteractions;
    }
}