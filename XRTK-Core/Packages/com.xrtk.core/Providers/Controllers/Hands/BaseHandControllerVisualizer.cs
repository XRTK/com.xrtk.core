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

        /// <summary>
        /// Gets or sets the handedness this visulizer should visualize.
        /// </summary>
        public Handedness Handedness { get; set; }

        /// <summary>
        /// The currently active hand visualization profile.
        /// </summary>
        protected MixedRealityHandControllerVisualizationProfile Profile => MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.HandVisualizationProfile;

        /// <summary>
        /// Executes when the visualizer is enabled for the first time.
        /// </summary>
        protected virtual void Start()
        {
            dataProvider = MixedRealityToolkit.GetService<IMixedRealityHandControllerDataProvider>();
            dataProvider.Register(this);
        }

        /// <summary>
        /// Executes when the visuailzer is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            dataProvider.Unregister(this);
        }

        /// <inheritdoc />
        public virtual void OnHandDataUpdated(InputEventData<HandData> eventData) { }
    }
}