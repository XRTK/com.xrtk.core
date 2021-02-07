using System;
using System.IO;
using UnityEngine;
using XRTK.Editor.Utilities;
using XRTK.Utilities.WindowsDevicePortal.DataStructures;

namespace XRTK.Editor.BuildPipeline
{
    public static class UwpBuildDeployPreferences
    {
        public static Version MIN_SDK_VERSION = new Version("10.0.17763.0");
        private const string EDITOR_PREF_BUILD_CONFIG = "BuildDeployWindow_BuildConfig";
        private const string EDITOR_PREF_FORCE_REBUILD = "BuildDeployWindow_ForceRebuild";
        private const string EDITOR_PREF_CONNECT_INFOS = "BuildDeployWindow_DeviceConnections";
        private const string EDITOR_PREF_FULL_REINSTALL = "BuildDeployWindow_FullReinstall";
        private const string EDITOR_PREF_USE_SSL = "BuildDeployWindow_UseSSL";
        private const string EDITOR_PREF_PROCESS_ALL = "BuildDeployWindow_ProcessAll";
        private const string EDITOR_PREF_APP_ICON_SETTINGS_PATH = "ProjectSettings/Xrtk_MixedRealityIconPath.json";

        /// <summary>
        /// The current Build Configuration. (Debug, Release, or Master)
        /// </summary>
        public static string BuildConfig
        {
            get => EditorPreferences.Get(EDITOR_PREF_BUILD_CONFIG, "master");
            set => EditorPreferences.Set(EDITOR_PREF_BUILD_CONFIG, value.ToLower());
        }

        /// <summary>
        /// Current setting to force rebuilding the appx.
        /// </summary>
        public static bool ForceRebuild
        {
            get => EditorPreferences.Get(EDITOR_PREF_FORCE_REBUILD, false);
            set => EditorPreferences.Set(EDITOR_PREF_FORCE_REBUILD, value);
        }

        /// <summary>
        /// Current setting to fully uninstall and reinstall the appx.
        /// </summary>
        public static bool FullReinstall
        {
            get => EditorPreferences.Get(EDITOR_PREF_FULL_REINSTALL, true);
            set => EditorPreferences.Set(EDITOR_PREF_FULL_REINSTALL, value);
        }

        private static string appIconPath;

        /// <summary>
        /// The path to the 3d app icon .glb asset.
        /// </summary>
        public static string MixedRealityAppIconPath
        {
            get
            {
                if (!string.IsNullOrEmpty(appIconPath))
                {
                    return appIconPath;
                }

                var projectSettingsPath = Path.GetFullPath(EDITOR_PREF_APP_ICON_SETTINGS_PATH);

                if (!File.Exists(projectSettingsPath))
                {
                    var appIconAsset = new MixedRealityAppIcon { MixedRealityAppIconPath = string.Empty };
                    JsonUtility.ToJson(appIconAsset);
                    return appIconAsset.MixedRealityAppIconPath;
                }

                var data = File.ReadAllText(projectSettingsPath);
                return JsonUtility.FromJson<MixedRealityAppIcon>(data).MixedRealityAppIconPath;
            }
            set
            {
                var projectSettingsPath = Path.GetFullPath(EDITOR_PREF_APP_ICON_SETTINGS_PATH);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    Debug.Assert(value.EndsWith(".glb"), "3d App Icon must be a .glb asset");
                }

                appIconPath = value;
                File.WriteAllText(projectSettingsPath, JsonUtility.ToJson(new MixedRealityAppIcon { MixedRealityAppIconPath = value }));
            }
        }

        /// <summary>
        /// The current device portal connections.
        /// </summary>
        public static string DevicePortalConnections
        {
            get => EditorPreferences.Get(
                    EDITOR_PREF_CONNECT_INFOS,
                    JsonUtility.ToJson(
                            new DevicePortalConnections(
                                    new DeviceInfo("127.0.0.1", string.Empty, string.Empty, "Local Machine"))));
            set => EditorPreferences.Set(EDITOR_PREF_CONNECT_INFOS, value);
        }

        /// <summary>
        /// Current setting to use Single Socket Layer connections to the device portal.
        /// </summary>
        public static bool UseSSL
        {
            get => EditorPreferences.Get(EDITOR_PREF_USE_SSL, true);
            set => EditorPreferences.Set(EDITOR_PREF_USE_SSL, value);
        }

        /// <summary>
        /// Current setting to target all the devices registered to the build window.
        /// </summary>
        public static bool TargetAllConnections
        {
            get => EditorPreferences.Get(EDITOR_PREF_PROCESS_ALL, false);
            set => EditorPreferences.Set(EDITOR_PREF_PROCESS_ALL, value);
        }
    }
}
