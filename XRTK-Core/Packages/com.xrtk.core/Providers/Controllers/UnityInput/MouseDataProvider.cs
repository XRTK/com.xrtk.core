// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Definitions.Controllers.UnityInput.Profiles;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Providers.Controllers;
using XRTK.Services;
using XRTK.Utilities.Physics;

namespace XRTK.Providers.Controllers.UnityInput
{
    /// <summary>
    /// The mouse data provider.
    /// </summary>
    public class MouseDataProvider : BaseControllerDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        public MouseDataProvider(string name, uint priority, MouseControllerDataProviderProfile profile)
            : base(name, priority, profile)
        {
        }

        /// <summary>
        /// Current Mouse Controller.
        /// </summary>
        public MouseController Controller { get; private set; }

        /// <inheritdoc />
        public override void Enable()
        {
            if (!Input.mousePresent)
            {
                Disable();
                return;
            }

#if UNITY_EDITOR
            if (UnityEditor.EditorWindow.focusedWindow != null)
            {
                UnityEditor.EditorWindow.focusedWindow.ShowNotification(new GUIContent("Press \"ESC\" to regain mouse control"));
            }
#endif

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; ;

            MixedRealityRaycaster.DebugEnabled = true;

            try
            {
                Controller = new MouseController(this, TrackingState.NotApplicable, Handedness.Any, GetControllerMappingProfile(typeof(MouseController), Handedness.Any));
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create {nameof(MouseController)}!\n{e}");
                return;
            }

            MixedRealityToolkit.InputSystem?.RaiseSourceDetected(Controller.InputSource, Controller);
            AddController(Controller);
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (Input.mousePresent && Controller == null) { Enable(); }

            Controller?.Update();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            if (Controller != null)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceLost(Controller.InputSource, Controller);
                RemoveController(Controller);
            }
        }
    }
}