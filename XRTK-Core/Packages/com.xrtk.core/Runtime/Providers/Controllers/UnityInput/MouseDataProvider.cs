// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers.UnityInput.Profiles;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Utilities.Physics;

namespace XRTK.Providers.Controllers.UnityInput
{
    /// <summary>
    /// The mouse data provider.
    /// </summary>
    [System.Runtime.InteropServices.Guid("067CE7D4-8277-4E18-834E-3DC712074B72")]
    public class MouseDataProvider : BaseControllerDataProvider
    {
        /// <inheritdoc />
        public MouseDataProvider(string name, uint priority, MouseControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
        }

        /// <summary>
        /// Current Mouse Controller.
        /// </summary>
        public MouseController Controller { get; private set; }

        /// <inheritdoc />
        public override void Enable()
        {
            if (MouseController.IsInGameWindow && Controller == null)
            {
                CreateController();
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();
            Controller?.Update();
        }

        /// <inheritdoc />
        public override void OnApplicationFocus(bool isFocused)
        {
            base.OnApplicationFocus(isFocused);

            if (Controller != null)
            {
                Cursor.visible = !isFocused;
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            if (Controller != null)
            {
                DestroyController();
            }
        }

        private void CreateController()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorWindow.focusedWindow != null)
            {
                UnityEditor.EditorWindow.focusedWindow.ShowNotification(new GUIContent("Press \"ESC\" to regain mouse control"));
            }
#endif

            Cursor.visible = false;
            MixedRealityRaycaster.DebugEnabled = true;

            try
            {
                Controller = new MouseController(this, TrackingState.NotApplicable, Handedness.Any, GetControllerProfile(typeof(MouseController), Handedness.Any));
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create {nameof(MouseController)}!\n{e}");
                return;
            }

            InputSystem?.RaiseSourceDetected(Controller.InputSource, Controller);
            AddController(Controller);
        }

        private void DestroyController()
        {
            InputSystem?.RaiseSourceLost(Controller.InputSource, Controller);
            RemoveController(Controller);
        }
    }
}
