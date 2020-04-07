// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers
{
    [Serializable]
    public struct ControllerDefinition
    {
        public ControllerDefinition(SystemType controllerType, Handedness handedness = Handedness.None, bool useCustomInteractions = false) : this()
        {
            ControllerType = controllerType;
            Handedness = handedness;
            UseCustomInteractions = useCustomInteractions;
        }

        public readonly SystemType ControllerType;

        public readonly Handedness Handedness;

        public readonly bool UseCustomInteractions;
    }
}