﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.Events;
using XRTK.Interfaces.Providers.Controllers;

namespace XRTK.Interfaces.InputSystem
{
    /// <summary>
    /// Manager interface for a Input system in the Mixed Reality Toolkit
    /// All replacement systems for providing Input System functionality should derive from this interface
    /// </summary>
    public interface IMixedRealityInputSystem : IMixedRealityEventSystem
    {
        /// <summary>
        /// Event that's raised when the Input is enabled.
        /// </summary>
        event Action InputEnabled;

        /// <summary>
        /// Event that's raised when the Input is disabled.
        /// </summary>
        event Action InputDisabled;

        /// <summary>
        /// List of the Interaction Input Sources as detected by the input manager like hands or motion controllers.
        /// </summary>
        IReadOnlyCollection<IMixedRealityInputSource> DetectedInputSources { get; }

        /// <summary>
        /// List of <see cref="IMixedRealityController"/>s currently detected by the input manager.
        /// </summary>
        /// <remarks>
        /// This property is similar to <see cref="DetectedInputSources"/>, as this is a subset of those <see cref="IMixedRealityInputSource"/>s in that list.
        /// </remarks>
        IReadOnlyCollection<IMixedRealityController> DetectedControllers { get; }

        /// <summary>
        /// The current Focus Provider that's been implemented by this Input System.
        /// </summary>
        IMixedRealityFocusProvider FocusProvider { get; }

        /// <summary>
        /// The current Gaze Provider that's been implemented by this Input System.
        /// </summary>
        IMixedRealityGazeProvider GazeProvider { get; }

        /// <summary>
        /// Indicates if input is currently enabled or not.
        /// </summary>
        bool IsInputEnabled { get; }

        /// <summary>
        /// Push a disabled input state onto the Input System.
        /// While input is disabled no events will be sent out and the cursor displays
        /// a waiting animation.
        /// </summary>
        void PushInputDisable();

        /// <summary>
        /// Pop disabled input state. When the last disabled state is 
        /// popped off the stack input will be re-enabled.
        /// </summary>
        void PopInputDisable();

        /// <summary>
        /// Clear the input disable stack, which will immediately re-enable input.
        /// </summary>
        void ClearInputDisableStack();

        /// <summary>
        /// Push a game object into the modal input stack. Any input handlers
        /// on the game object are given priority to input events before any focused objects.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        void PushModalInputHandler(GameObject inputHandler);

        /// <summary>
        /// Remove the last game object from the modal input stack.
        /// </summary>
        void PopModalInputHandler();

        /// <summary>
        /// Clear all modal input handlers off the stack.
        /// </summary>
        void ClearModalInputStack();

        /// <summary>
        /// Push a game object into the fallback input stack. Any input handlers on
        /// the game object are given input events when no modal or focused objects consume the event.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        void PushFallbackInputHandler(GameObject inputHandler);

        /// <summary>
        /// Remove the last game object from the fallback input stack.
        /// </summary>
        void PopFallbackInputHandler();

        /// <summary>
        /// Clear all fallback input handlers off the stack.
        /// </summary>
        void ClearFallbackInputStack();

        #region IMixedRealityController Utilities

        /// <summary>
        /// Tried to get a <see cref="IMixedRealityController"/> from the <see cref="DetectedControllers"/> list.
        /// </summary>
        /// <param name="inputSource">The <see cref="IMixedRealityInputSource"/> you want to get a controller reference for.</param>
        /// <param name="controller">The <see cref="IMixedRealityController"/> that was found in the list of <see cref="DetectedControllers"/></param>
        /// <returns>True, if an <see cref="IMixedRealityController"/> is found.</returns>
        bool TryGetController(IMixedRealityInputSource inputSource, out IMixedRealityController controller);

        #endregion IMixedRealityController Utilities

        #region Input Events

        /// <summary>
        /// Raised when an input event is triggered.
        /// </summary>
        /// <remarks>
        /// WARNING: This event should not be subscribed to by MonoBehaviours!
        /// Use the InputHandler interfaces instead.
        /// </remarks>
        event Action<BaseInputEventData> OnInputEvent;

        #region Input Source Events

        /// <summary>
        /// Generates a new unique input source id.<para/>
        /// </summary>
        /// <remarks>All Input Sources are required to call this method in their constructor or initialization.</remarks>
        /// <returns>a new unique Id for the input source.</returns>
        uint GenerateNewSourceId();

        /// <summary>
        /// Request a new generic input source from the system.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pointers"></param>
        /// <returns></returns>
        IMixedRealityInputSource RequestNewGenericInputSource(string name, IMixedRealityPointer[] pointers = null);

        /// <summary>
        /// Raise the event that the Input Source was detected.
        /// </summary>
        /// <param name="source">The detected Input Source.</param>
        /// <param name="controller"></param>
        void RaiseSourceDetected(IMixedRealityInputSource source, IMixedRealityController controller = null);

        /// <summary>
        /// Raise the event that the Input Source was lost.
        /// </summary>
        /// <param name="source">The lost Input Source.</param>
        /// <param name="controller"></param>
        void RaiseSourceLost(IMixedRealityInputSource source, IMixedRealityController controller = null);

        /// <summary>
        /// Raise the event that the Input Source's tracking state has changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="state"></param>
        void RaiseSourceTrackingStateChanged(IMixedRealityInputSource source, IMixedRealityController controller, TrackingState state);

        /// <summary>
        /// Raise the event that the Input Source position was changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="position"></param>
        void RaiseSourcePositionChanged(IMixedRealityInputSource source, IMixedRealityController controller, Vector2 position);

        /// <summary>
        /// Raise the event that the Input Source position was changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="position"></param>
        void RaiseSourcePositionChanged(IMixedRealityInputSource source, IMixedRealityController controller, Vector3 position);

        /// <summary>
        /// Raise the event that the Input Source position was changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="rotation"></param>
        void RaiseSourceRotationChanged(IMixedRealityInputSource source, IMixedRealityController controller, Quaternion rotation);

        /// <summary>
        /// Raise the event that the Input Source position was changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="controller"></param>
        /// <param name="position"></param>
        void RaiseSourcePoseChanged(IMixedRealityInputSource source, IMixedRealityController controller, MixedRealityPose position);

        #endregion Input Source Events

        #region Focus Events

        /// <summary>
        /// Raise the pre-focus changed event.
        /// </summary>
        /// <remarks>This event is useful for doing logic before the focus changed event.</remarks>
        /// <param name="pointer">The pointer that the focus change event is raised on.</param>
        /// <param name="oldFocusedObject">The old focused object.</param>
        /// <param name="newFocusedObject">The new focused object.</param>
        void RaisePreFocusChanged(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject);

        /// <summary>
        /// Raise the focus changed event.
        /// </summary>
        /// <param name="pointer">The pointer that the focus change event is raised on.</param>
        /// <param name="oldFocusedObject">The old focused object.</param>
        /// <param name="newFocusedObject">The new focused object.</param>
        void RaiseFocusChanged(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject);

        /// <summary>
        /// Raise the focus enter event.
        /// </summary>
        /// <param name="pointer">The pointer that has focus.</param>
        /// <param name="focusedObject">The <see cref="GameObject"/> that the pointer has entered focus on.</param>
        void RaiseFocusEnter(IMixedRealityPointer pointer, GameObject focusedObject);

        /// <summary>
        /// Raise the focus exit event.
        /// </summary>
        /// <param name="pointer">The pointer that has lost focus.</param>
        /// <param name="unfocusedObject">The <see cref="GameObject"/> that the pointer has exited focus on.</param>
        void RaiseFocusExit(IMixedRealityPointer pointer, GameObject unfocusedObject);

        #endregion Focus Events

        #region Pointers

        #region Pointer Down

        /// <summary>
        /// Raise the pointer down event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="inputAction">The action associated with this event.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerDown(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, IMixedRealityInputSource inputSource = null);

        #endregion Pointer Down

        #region Pointer Click

        /// <summary>
        /// Raise the pointer clicked event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="inputAction">The action associated with this event.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerClicked(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, IMixedRealityInputSource inputSource = null);

        #endregion Pointer Click

        #region Pointer Up

        /// <summary>
        /// Raise the pointer up event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="inputAction">The action associated with this event.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerUp(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, IMixedRealityInputSource inputSource = null);

        #endregion Pointer Up

        /// <summary>
        /// Raise the pointer scroll event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="scrollAction">The action associated with this event.</param>
        /// <param name="scrollDelta">The distance this pointer has scrolled since the scroll event was last raised.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerScroll(IMixedRealityPointer pointer, MixedRealityInputAction scrollAction, Vector2 scrollDelta, IMixedRealityInputSource inputSource = null);

        #region Pointer Dragging

        /// <summary>
        /// Raise the pointer drag begin event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="draggedAction">The action associated with this event.</param>
        /// <param name="dragDelta">The distance this pointer has been moved since the last time the dragged event was last raised.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerDragBegin(IMixedRealityPointer pointer, MixedRealityInputAction draggedAction, Vector3 dragDelta, IMixedRealityInputSource inputSource = null);

        /// <summary>
        /// Raise the pointer drag event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="draggedAction">The action associated with this event.</param>
        /// <param name="dragDelta">The distance this pointer has been moved since the last time the dragged event was last raised.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerDrag(IMixedRealityPointer pointer, MixedRealityInputAction draggedAction, Vector3 dragDelta, IMixedRealityInputSource inputSource = null);

        /// <summary>
        /// Raise the pointer drag end event.
        /// </summary>
        /// <param name="pointer">The pointer where the event originates.</param>
        /// <param name="draggedAction">The action associated with this event.</param>
        /// <param name="dragDelta">The distance this pointer has been moved since the last time the dragged event was last raised.</param>
        /// <param name="inputSource">The input source this event is associated to, if null, the pointer's parent input source is used.</param>
        void RaisePointerDragEnd(IMixedRealityPointer pointer, MixedRealityInputAction draggedAction, Vector3 dragDelta, IMixedRealityInputSource inputSource = null);

        #endregion Pointer Dragging

        #endregion Pointers

        #region Generic Input Events

        #region Input Down

        /// <summary>
        /// Raise the input down event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputDown(IMixedRealityInputSource source, MixedRealityInputAction inputAction);

        /// <summary>
        /// Raise the input down event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction);

        #endregion Input Down

        #region Input Pressed

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, MixedRealityInputAction inputAction);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="pressAmount"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, MixedRealityInputAction inputAction, float pressAmount);

        /// <summary>
        /// Raise Input Pressed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="pressAmount"></param>
        void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, float pressAmount);

        #endregion Input Pressed

        #region Input Up

        /// <summary>
        /// Raise the input up event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputUp(IMixedRealityInputSource source, MixedRealityInputAction inputAction);

        /// <summary>
        /// Raise the input up event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction);

        #endregion Input Up

        #region Input Position Changed

        /// <summary>
        /// Raise the 1st degree of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, float position);

        /// <summary>
        /// Raise the 1st degree of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, float position);

        /// <summary>
        /// Raise the 2 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, Vector2 position);

        /// <summary>
        /// Raise the 2 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Vector2 position);

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, Vector3 position);

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        void RaisePositionInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Vector3 position);

        #endregion Input Position Changed

        #region Input Rotation Changed

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="rotation"></param>
        void RaiseRotationInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, Quaternion rotation);

        /// <summary>
        /// Raise the 3 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="rotation"></param>
        void RaiseRotationInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Quaternion rotation);

        #endregion Input Rotation Changed

        #region Input Pose Changed

        /// <summary>
        /// Raise the 6 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="inputData"></param>
        void RaisePoseInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, MixedRealityPose inputData);

        /// <summary>
        /// Raise the 6 degrees of freedom input event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="handedness"></param>
        /// <param name="inputAction"></param>
        /// <param name="inputData"></param>
        void RaisePoseInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, MixedRealityPose inputData);

        #endregion Input Pose Changed

        #endregion Generic Input Events

        #region Generic Gesture Events

        /// <summary>
        /// Raise the Gesture Started Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        void RaiseGestureStarted(IMixedRealityController controller, MixedRealityInputAction action);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, Vector2 inputData);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, Vector3 inputData);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, Quaternion inputData);

        /// <summary>
        /// Raise the Gesture Updated Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, MixedRealityPose inputData);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, Vector2 inputData);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, Vector3 inputData);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, Quaternion inputData);

        /// <summary>
        /// Raise the Gesture Completed Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="inputData"></param>
        void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, MixedRealityPose inputData);

        /// <summary>
        /// Raise the Gesture Canceled Event.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        void RaiseGestureCanceled(IMixedRealityController controller, MixedRealityInputAction action);

        #endregion

        #region Speech Keyword Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputAction"></param>
        /// <param name="confidence"></param>
        /// <param name="phraseDuration"></param>
        /// <param name="phraseStartTime"></param>
        /// <param name="text"></param>
        void RaiseSpeechCommandRecognized(IMixedRealityInputSource source, MixedRealityInputAction inputAction, RecognitionConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, string text);

        #endregion Speech Keyword Events

        #region Dictation Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dictationHypothesis"></param>
        /// <param name="dictationAudioClip"></param>
        void RaiseDictationHypothesis(IMixedRealityInputSource source, string dictationHypothesis, AudioClip dictationAudioClip = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dictationResult"></param>
        /// <param name="dictationAudioClip"></param>
        void RaiseDictationResult(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dictationResult"></param>
        /// <param name="dictationAudioClip"></param>
        void RaiseDictationComplete(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dictationResult"></param>
        /// <param name="dictationAudioClip"></param>
        void RaiseDictationError(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null);

        #endregion Dictation Events

        #endregion Input Events
    }
}