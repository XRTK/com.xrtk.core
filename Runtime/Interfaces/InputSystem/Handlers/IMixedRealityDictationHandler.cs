﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.EventDatum.Input;

namespace XRTK.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement dictation events.
    /// </summary>
    public interface IMixedRealityDictationHandler : IEventSystemHandler
    {
        void OnDictationHypothesis(DictationEventData eventData);

        void OnDictationResult(DictationEventData eventData);

        void OnDictationComplete(DictationEventData eventData);

        void OnDictationError(DictationEventData eventData);
    }
}
