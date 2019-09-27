// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Utilities
{
    public static class PlatformUtility
    {
        /// <summary>
        /// Checks to see if the <see cref="RuntimePlatform"/> is part of the <see cref="SupportedPlatforms"/>
        /// </summary>
        /// <param name="runtimePlatform"></param>
        /// <param name="platforms"></param>
        /// <returns>True, if the runtime platform is included in the list of supported platforms, otherwise false.</returns>
        public static bool IsPlatformSupported(this RuntimePlatform runtimePlatform, SupportedPlatforms platforms)
        {
            if ((int)platforms == -1)
            {
                return true;
            }

            if (platforms == 0)
            {
                return false;
            }

            var target = GetSupportedPlatformMask(runtimePlatform);
            return IsPlatformSupported(target, platforms);
        }

        private static SupportedPlatforms GetSupportedPlatformMask(RuntimePlatform runtimePlatform)
        {
            SupportedPlatforms supportedPlatforms = 0;

            switch (runtimePlatform)
            {
                case RuntimePlatform.Android:
                    supportedPlatforms |= SupportedPlatforms.Android;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    supportedPlatforms |= SupportedPlatforms.iOS;
                    break;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    supportedPlatforms |= SupportedPlatforms.WindowsStandalone;
                    break;
                case RuntimePlatform.WSAPlayerARM:
                case RuntimePlatform.WSAPlayerX86:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.XboxOne:
                    supportedPlatforms |= SupportedPlatforms.WindowsUniversal;
                    break;
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    supportedPlatforms |= SupportedPlatforms.MacStandalone;
                    break;
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                    supportedPlatforms |= SupportedPlatforms.LinuxStandalone;
                    break;
                case RuntimePlatform.Lumin:
                    supportedPlatforms |= SupportedPlatforms.Lumin;
                    break;
            }

            return supportedPlatforms;
        }

        private static bool IsPlatformSupported(SupportedPlatforms target, SupportedPlatforms supported)
        {
            return (target & supported) != 0;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Checks to see if the <see cref="RuntimePlatform"/> is part of the <see cref="SupportedPlatforms"/>
        /// </summary>
        /// <param name="editorBuildTarget"></param>
        /// <param name="platforms"></param>
        /// <returns>True, if the runtime platform is included in the list of supported platforms, otherwise false.</returns>
        public static bool IsPlatformSupported(this UnityEditor.BuildTarget editorBuildTarget, SupportedPlatforms platforms)
        {
            var isEditorSupported = IsEditorSupported(platforms);

            if ((int)platforms == -1)
            {
                return true;
            }

            if (!isEditorSupported || platforms == 0)
            {
                return false;
            }

            var target = GetSupportedPlatformMask(editorBuildTarget);
            return IsPlatformSupported(target, platforms);
        }

        private static bool IsEditorSupported(SupportedPlatforms platforms)
        {
            return (platforms & SupportedPlatforms.Editor) != 0;
        }

        private static SupportedPlatforms GetSupportedPlatformMask(UnityEditor.BuildTarget editorBuildTarget)
        {
            SupportedPlatforms supportedPlatforms = 0;

            switch (editorBuildTarget)
            {
                case UnityEditor.BuildTarget.Android:
                    supportedPlatforms |= SupportedPlatforms.Android;
                    break;
                case UnityEditor.BuildTarget.iOS:
                    supportedPlatforms |= SupportedPlatforms.iOS;
                    break;
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    supportedPlatforms |= SupportedPlatforms.WindowsStandalone;
                    break;
                case UnityEditor.BuildTarget.WSAPlayer:
                case UnityEditor.BuildTarget.XboxOne:
                    supportedPlatforms |= SupportedPlatforms.WindowsUniversal;
                    break;
                case UnityEditor.BuildTarget.StandaloneOSX:
                    supportedPlatforms |= SupportedPlatforms.MacStandalone;
                    break;
                case UnityEditor.BuildTarget.StandaloneLinux64:
                    supportedPlatforms |= SupportedPlatforms.LinuxStandalone;
                    break;
                case UnityEditor.BuildTarget.Lumin:
                    supportedPlatforms |= SupportedPlatforms.Lumin;
                    break;
            }

            return supportedPlatforms;
        }
#endif
    }
}