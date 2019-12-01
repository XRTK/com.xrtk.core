// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands
{
    public abstract class BaseHandControllerVisualizer : MonoBehaviour, IMixedRealityHandDataHandler
    {
        private IMixedRealityHandControllerDataProvider dataProvider;

        [SerializeField]
        [Tooltip("Should a gizmo be drawn to represent the hand bounds.")]
        private bool drawBoundsGizmo = true;

        /// <summary>
        /// Gets or sets the handedness this visulizer should visualize.
        /// </summary>
        public Handedness Handedness { get; set; }

        /// <summary>
        /// Should a gizmo be drawn to represent the hand bounds.
        /// </summary>
        public bool DrawBoundsGizmo
        {
            get { return drawBoundsGizmo; }
            set { drawBoundsGizmo = value; }
        }

        /// <summary>
        /// The currently active hand visualization profile.
        /// </summary>
        protected MixedRealityHandControllerVisualizationProfile Profile => MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.HandVisualizationProfile;

        /// <summary>
        /// Executes when the visualizer is enabled.
        /// </summary>
        protected virtual void OnEnable()
        {
            dataProvider = MixedRealityToolkit.GetService<IMixedRealityHandControllerDataProvider>();
            dataProvider.Register(this);
        }

        /// <summary>
        /// Called by the Unity runtime when gizmos should be drawn.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (drawBoundsGizmo)
            {
                foreach (var controller in dataProvider.ActiveControllers)
                {
                    if (controller.ControllerHandedness == Handedness && controller is IMixedRealityHandController handController)
                    {
                        Gizmos.DrawWireCube(handController.Bounds.center, handController.Bounds.size);
                    }
                }
            }
        }

        /// <summary>
        /// Executes when the visuailzer is disabled.
        /// </summary>
        protected virtual void OnDisable()
        {
            dataProvider.Unregister(this);
        }

        /// <inheritdoc />
        public virtual void OnHandDataUpdated(InputEventData<HandData> eventData) { }
    }
}