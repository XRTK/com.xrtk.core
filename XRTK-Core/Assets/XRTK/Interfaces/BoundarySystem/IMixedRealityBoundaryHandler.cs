// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.EventDatum.Boundary;

namespace XRTK.Interfaces.BoundarySystem
{
    public interface IMixedRealityBoundaryHandler : IEventSystemHandler
    {
        /// <summary>
        /// Raised when the boundary visualization has changed.
        /// </summary>
        /// <param name="eventData"></param>
        void OnBoundaryVisualizationChanged(BoundaryEventData eventData);
    }
}
