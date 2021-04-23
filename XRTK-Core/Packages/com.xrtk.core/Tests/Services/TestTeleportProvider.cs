// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.EventDatum.Teleport;
using XRTK.Interfaces.TeleportSystem.Handlers;

public class TestTeleportProvider : MonoBehaviour, IMixedRealityTeleportProvider
{
    #region Implementation of IMixedRealityTeleportHandler

    /// <inheritdoc />
    public void OnTeleportRequest(TeleportEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public void OnTeleportStarted(TeleportEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public void OnTeleportCompleted(TeleportEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public void OnTeleportCanceled(TeleportEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
