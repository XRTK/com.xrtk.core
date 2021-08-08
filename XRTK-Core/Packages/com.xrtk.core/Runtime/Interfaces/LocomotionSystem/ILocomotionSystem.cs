// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Events;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Interfaces.LocomotionSystem
{
    /// <summary>
    /// System interface for a locomotion system in the Mixed Reality Toolkit.
    /// All replacement systems for providing locomotion functionality should derive from this interface.
    /// </summary>
    public interface ILocomotionSystem : IMixedRealityEventSystem
    {
        /// <summary>
        /// Gets a list of currently enabled <see cref="ILocomotionProvider"/>s.
        /// </summary>
        IReadOnlyList<ILocomotionProvider> EnabledLocomotionProviders { get; }

        /// <summary>
        /// Enables a locomotion provider of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="ILocomotionProvider"/> to enable.</typeparam>
        void EnableLocomotionProvider<T>() where T : ILocomotionProvider;

        /// <summary>
        /// Enables a locomotion provider of type <paramref name="locomotionProviderType"/>.
        /// </summary>
        /// <paramref name="locomotionProviderType">Type of the <see cref="ILocomotionProvider"/> to enable.</typeparam>
        void EnableLocomotionProvider(Type locomotionProviderType);

        /// <summary>
        /// Disables a locomotion provider of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="ILocomotionProvider"/> to disable.</typeparam>
        void DisableLocomotionProvider<T>() where T : ILocomotionProvider;

        /// <summary>
        /// Disables a locomotion provider of type <paramref name="locomotionProviderType"/>.
        /// </summary>
        /// <paramref name="locomotionProviderType">Type of the <see cref="ILocomotionProvider"/> to disable.</typeparam>
        void DisableLocomotionProvider(Type locomotionProviderType);

        /// <summary>
        /// A <see cref="ILocomotionProvider"/> was enabled.
        /// </summary>
        /// <param name="locomotionProvider">The enabled <see cref="ILocomotionProvider"/>.</param>
        void OnLocomotionProviderEnabled(ILocomotionProvider locomotionProvider);

        /// <summary>
        /// A <see cref="ILocomotionProvider"/> was disabled.
        /// </summary>
        /// <param name="locomotionProvider">The disabled <see cref="ILocomotionProvider"/>.</param>
        void OnLocomotionProviderDisabled(ILocomotionProvider locomotionProvider);

        /// <summary>
        /// Raise a teleportation target request event.
        /// </summary>
        /// <param name="teleportLocomotionProvider">The <see cref="ITeleportLocomotionProvider"/> that requests a teleport target.</param>
        /// <param name="inputSource">The <see cref="IMixedRealityInputSource"/> the <paramref name="teleportLocomotionProvider"/> requests the teleport location from.</param>
        void RaiseTeleportTargetRequest(ITeleportLocomotionProvider teleportLocomotionProvider, IMixedRealityInputSource inputSource);

        /// <summary>
        /// Raises a teleportation started event for <see cref="ILocomotionSystemHandler"/>s.
        /// </summary>
        /// <param name="locomotionProvider">The <see cref="ITeleportLocomotionProvider"/> that started teleportation.</param>
        /// <param name="inputSource">The <see cref="IMixedRealityInputSource"/> the <paramref name="locomotionProvider"/>'s teleport request originated from.</param>
        /// <param name="pose">The target <see cref="MixedRealityPose"/> the teleportation is going for.</param>
        /// <param name="hotSpot">The teleport target hot spot, if any.</param>
        void RaiseTeleportStarted(ITeleportLocomotionProvider locomotionProvider, IMixedRealityInputSource inputSource, MixedRealityPose pose, ITeleportHotSpot hotSpot);

        /// <summary>
        /// Raises a teleportation completed event for <see cref="ILocomotionSystemHandler"/>s.
        /// </summary>
        /// <param name="locomotionProvider">The <see cref="ITeleportLocomotionProvider"/> whose teleportation has completed.</param>
        /// <param name="inputSource">The <see cref="IMixedRealityInputSource"/> the <paramref name="locomotionProvider"/>'s teleport request originated from.</param>
        /// <param name="pose">The target <see cref="MixedRealityPose"/> the teleportation was going for.</param>
        /// <param name="hotSpot">The teleport target hot spot, if any.</param>
        void RaiseTeleportCompleted(ITeleportLocomotionProvider locomotionProvider, IMixedRealityInputSource inputSource, MixedRealityPose pose, ITeleportHotSpot hotSpot);

        /// <summary>
        /// Raises a teleportation canceled event for <see cref="ILocomotionSystemHandler"/>s.
        /// </summary>
        /// <param name="locomotionProvider">The <see cref="ITeleportLocomotionProvider"/> that canceled a previously started teleport.</param>
        /// <param name="inputSource">The <see cref="IMixedRealityInputSource"/> the <paramref name="locomotionProvider"/>'s teleport request originated from.</param>
        void RaiseTeleportCanceled(ITeleportLocomotionProvider locomotionProvider, IMixedRealityInputSource inputSource);
    }
}
