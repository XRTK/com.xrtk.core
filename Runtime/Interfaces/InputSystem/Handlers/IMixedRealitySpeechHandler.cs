﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;
using XRTK.EventDatum.Input;

namespace XRTK.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement to react to speech recognition.
    /// </summary>
    public interface IMixedRealitySpeechHandler : IEventSystemHandler
    {
        void OnSpeechKeywordRecognized(SpeechEventData eventData);
    }
}