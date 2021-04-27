// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.EventDatum.Locomotion;
using XRTK.Interfaces.LocomotionSystem;

public class TestTeleportProvider : MonoBehaviour, IMixedRealityTeleportProvider
{
    #region Implementation of IMixedRealityTeleportHandler

    /// <inheritdoc />
    public void OnLocomotionRequest(LocomotionEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public void OnLocomotionStarted(LocomotionEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public void OnLocomotionCompleted(LocomotionEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public void OnLocomotionCanceled(LocomotionEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
