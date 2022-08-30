// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Platforms;
using XRTK.Interfaces.InputSystem;
using XRTK.Services.InputSystem;

namespace XRTK.Tests.InputSystem
{
    public static class InputSystemTestUtilities
    {
        public static MixedRealityServiceConfiguration<IMixedRealityInputSystem> TestInputSystemConfiguration
            => new MixedRealityServiceConfiguration<IMixedRealityInputSystem>(typeof(MixedRealityInputSystem), nameof(MixedRealityInputSystem), 1, new[] { new AllPlatforms() }, SetupInputSystemProfile());

        public static MixedRealityInputSystemProfile SetupInputSystemProfile()
        {
            // Create blank Input System Profiles
            var inputSystemProfile = ScriptableObject.CreateInstance<MixedRealityInputSystemProfile>();
            inputSystemProfile.FocusProviderType = typeof(FocusProvider);
            inputSystemProfile.GazeProviderType = typeof(GazeProvider);
            inputSystemProfile.SpeechCommandsProfile = ScriptableObject.CreateInstance<MixedRealitySpeechCommandsProfile>();
            return inputSystemProfile;
        }
    }
}
