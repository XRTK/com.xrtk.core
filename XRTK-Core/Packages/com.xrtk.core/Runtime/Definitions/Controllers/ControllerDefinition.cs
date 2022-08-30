// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Controllers
{
    [Serializable]
    public struct ControllerDefinition
    {
        public ControllerDefinition(SystemType controllerType, Handedness handedness = Handedness.None)
        {
            ControllerType = controllerType;
            Handedness = handedness;

            string description = null;

            if (ControllerType?.Type != null)
            {
                if (handedness == Handedness.Right || handedness == Handedness.Left)
                {
                    description = $"{handedness}{ControllerType.Type.Name}";
                }
                else
                {
                    description = ControllerType.Type.Name;
                }
            }

            Description = description;
        }

        public ControllerDefinition(string description, SystemType controllerType, Handedness handedness = Handedness.None)
            : this(controllerType, handedness)
        {
            Description = description;
        }

        public readonly SystemType ControllerType;

        public readonly string Description;

        public readonly Handedness Handedness;
    }
}
