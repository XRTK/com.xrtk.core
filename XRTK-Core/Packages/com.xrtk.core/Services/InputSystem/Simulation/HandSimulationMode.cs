// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Services.InputSystem.Simulation
{
    public enum HandSimulationMode
    {
        // Disable hand simulation
        Disabled,
        // Raises gesture events only
        Gestures,
        // Provide a fully articulated hand controller
        Articulated,
    }
}