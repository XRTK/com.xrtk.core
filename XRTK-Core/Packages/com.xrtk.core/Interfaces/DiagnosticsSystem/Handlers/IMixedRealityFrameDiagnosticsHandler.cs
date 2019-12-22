// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Interfaces.DiagnosticsSystem.Handlers
{
    public interface IMixedRealityFrameDiagnosticsHandler : IMixedRealityDiagnosticsHandler
    {
        /// <summary>
        /// FPS have changed.
        /// </summary>
        /// <param name="oldFPS">The old FPS.</param>
        /// <param name="newFPS">The new FPS.</param>
        /// <param name="isGPU">Is it a GPU frame rate?</param>
        void OnFrameRateChanged(int oldFPS, int newFPS, bool isGPU);

        /// <summary>
        /// Missed frames diagnostics have changed.
        /// </summary>
        /// <param name="missedFrames">Array containing missed frames information. If true, the frame missed the target frame frate.</param>
        void OnMissedFramesChanged(bool[] missedFrames);
    }
}