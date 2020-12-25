// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.InputSystem;
using XRTK.Services.InputSystem;

namespace XRTK.Tests.InputSystem
{
    public static class InputSystemTestUtilities
    {
        public static MixedRealityInputSystemProfile SetupInputSystemProfile()
        {
            // Create blank Input System Profiles
            var inputSystemProfile = ScriptableObject.CreateInstance<MixedRealityInputSystemProfile>();
            inputSystemProfile.FocusProviderType = typeof(FocusProvider);
            inputSystemProfile.GazeProviderType = typeof(GazeProvider);
            inputSystemProfile.InputActionsProfile = ScriptableObject.CreateInstance<MixedRealityInputActionsProfile>();
            inputSystemProfile.GesturesProfile = ScriptableObject.CreateInstance<MixedRealityGesturesProfile>();
            inputSystemProfile.SpeechCommandsProfile = ScriptableObject.CreateInstance<MixedRealitySpeechCommandsProfile>();

            return inputSystemProfile;
        }
    }
}