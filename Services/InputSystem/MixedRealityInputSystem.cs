// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Input;
using XRTK.Extensions;
using XRTK.Interfaces.InputSystem;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services.InputSystem.Sources;
using XRTK.Utilities;

namespace XRTK.Services.InputSystem
{
    /// <summary>
    /// The Mixed Reality Toolkit's specific implementation of the <see cref="IMixedRealityInputSystem"/>
    /// </summary>
    public class MixedRealityInputSystem : BaseEventSystem, IMixedRealityInputSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile"></param>
        public MixedRealityInputSystem(MixedRealityInputSystemProfile profile) : base(profile)
        {
            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionRulesProfile == null)
            {
                throw new Exception("The Input system is missing the required Input Action Rules Profile!");
            }

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile == null)
            {
                throw new Exception("The Input system is missing the required Pointer Profile!");
            }

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile.GazeProviderType?.Type == null)
            {
                throw new Exception("The Input system is missing the required GazeProviderType!");
            }
        }

        /// <inheritdoc />
        public event Action InputEnabled;

        /// <inheritdoc />
        public event Action InputDisabled;

        /// <inheritdoc />
        public HashSet<IMixedRealityInputSource> DetectedInputSources { get; } = new HashSet<IMixedRealityInputSource>();

        /// <inheritdoc />
        public HashSet<IMixedRealityController> DetectedControllers { get; } = new HashSet<IMixedRealityController>();

        private IMixedRealityFocusProvider focusProvider = null;

        /// <inheritdoc />
        public IMixedRealityFocusProvider FocusProvider => focusProvider ?? (focusProvider = MixedRealityToolkit.GetService<IMixedRealityFocusProvider>());

        /// <inheritdoc />
        public IMixedRealityGazeProvider GazeProvider { get; private set; }

        private readonly Stack<GameObject> modalInputStack = new Stack<GameObject>();
        private readonly Stack<GameObject> fallbackInputStack = new Stack<GameObject>();

        /// <inheritdoc />
        public bool IsInputEnabled => disabledRefCount <= 0;

        private int disabledRefCount;

        private SourceStateEventData sourceStateEventData;
        private SourcePoseEventData<TrackingState> sourceTrackingEventData;
        private SourcePoseEventData<Vector2> sourceVector2EventData;
        private SourcePoseEventData<Vector3> sourcePositionEventData;
        private SourcePoseEventData<Quaternion> sourceRotationEventData;
        private SourcePoseEventData<MixedRealityPose> sourcePoseEventData;

        private FocusEventData focusEventData;

        private InputEventData inputEventData;
        private MixedRealityPointerEventData pointerEventData;

        private InputEventData<float> floatInputEventData;
        private InputEventData<Vector2> vector2InputEventData;
        private InputEventData<Vector3> positionInputEventData;
        private InputEventData<Quaternion> rotationInputEventData;
        private InputEventData<MixedRealityPose> poseInputEventData;

        private SpeechEventData speechEventData;
        private DictationEventData dictationEventData;

        private MixedRealityInputActionRulesProfile CurrentInputActionRulesProfile { get; } = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionRulesProfile;

        #region IMixedRealityManager Implementation

        /// <inheritdoc />
        /// <remarks>
        /// Input system is critical, so should be processed before all other managers
        /// </remarks>
        public override uint Priority => 1;

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            bool addedComponents = false;

            if (!Application.isPlaying)
            {
                var standaloneInputModules = UnityEngine.Object.FindObjectsOfType<StandaloneInputModule>();

                var cameraTransform = CameraCache.Main.transform;
                cameraTransform.position = Vector3.zero;
                cameraTransform.rotation = Quaternion.identity;

                if (standaloneInputModules.Length == 0)
                {
                    CameraCache.Main.gameObject.EnsureComponent<StandaloneInputModule>();
                    addedComponents = true;
                }
                else
                {
                    bool raiseWarning;

                    if (standaloneInputModules.Length == 1)
                    {
                        raiseWarning = standaloneInputModules[0].gameObject != CameraCache.Main.gameObject;
                    }
                    else
                    {
                        raiseWarning = true;
                    }

                    if (raiseWarning)
                    {
                        Debug.LogWarning("Found an existing Standalone Input Module in your scene. The Mixed Reality Input System requires only one, and must be found on the main camera.");
                    }
                }
            }
            else
            {
                var eventSystem = EventSystem.current;
                sourceStateEventData = new SourceStateEventData(eventSystem);

                sourceTrackingEventData = new SourcePoseEventData<TrackingState>(eventSystem);
                sourceVector2EventData = new SourcePoseEventData<Vector2>(eventSystem);
                sourcePositionEventData = new SourcePoseEventData<Vector3>(eventSystem);
                sourceRotationEventData = new SourcePoseEventData<Quaternion>(eventSystem);
                sourcePoseEventData = new SourcePoseEventData<MixedRealityPose>(eventSystem);

                focusEventData = new FocusEventData(eventSystem);

                inputEventData = new InputEventData(eventSystem);
                pointerEventData = new MixedRealityPointerEventData(eventSystem);

                floatInputEventData = new InputEventData<float>(eventSystem);
                vector2InputEventData = new InputEventData<Vector2>(eventSystem);
                positionInputEventData = new InputEventData<Vector3>(eventSystem);
                rotationInputEventData = new InputEventData<Quaternion>(eventSystem);
                poseInputEventData = new InputEventData<MixedRealityPose>(eventSystem);

                speechEventData = new SpeechEventData(eventSystem);
                dictationEventData = new DictationEventData(eventSystem);
            }

            if (!addedComponents)
            {
                CameraCache.Main.gameObject.EnsureComponent<StandaloneInputModule>();
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            GazeProvider = CameraCache.Main.gameObject.EnsureComponent(MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile.GazeProviderType.Type) as IMixedRealityGazeProvider;
        }

        /// <inheritdoc />
        public override void Enable()
        {
            InputEnabled?.Invoke();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            GazeProvider = null;

            if (!Application.isPlaying)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                var component = CameraCache.Main.GetComponent<IMixedRealityGazeProvider>() as Component;

                if (component != null)
                {
                    UnityEngine.Object.DestroyImmediate(component);
                }

                var inputModule = CameraCache.Main.GetComponent<StandaloneInputModule>();

                if (inputModule != null)
                {
                    UnityEngine.Object.DestroyImmediate(inputModule);
                }
            }

            InputDisabled?.Invoke();
        }

        #endregion IMixedRealityManager Implementation

        #region IEventSystemManager Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            if (disabledRefCount > 0)
            {
                return;
            }

            Debug.Assert(eventData != null);
            var baseInputEventData = ExecuteEvents.ValidateEventData<BaseInputEventData>(eventData);
            Debug.Assert(baseInputEventData != null);
            Debug.Assert(!baseInputEventData.used);

            if (baseInputEventData.InputSource == null)
            {
                Debug.LogError($"Failed to find an input source for {baseInputEventData}");
                return;
            }

            // Send the event to global listeners
            base.HandleEvent(eventData, eventHandler);

            if (baseInputEventData.used)
            {
                // All global listeners get a chance to see the event,
                // but if any of them marked it used,
                // we stop the event from going any further.
                return;
            }

            if (baseInputEventData.InputSource.Pointers == null)
            {
                Debug.LogError($"InputSource {baseInputEventData.InputSource.SourceName} doesn't have any registered pointers! Input Sources without pointers should use the GazeProvider's pointer as a default fallback.");
                return;
            }

            var modalEventHandled = false;

            // Get the focused object for each pointer of the event source
            for (int i = 0; i < baseInputEventData.InputSource.Pointers.Length; i++)
            {
                var focusedObject = FocusProvider?.GetFocusedObject(baseInputEventData.InputSource.Pointers[i]);

                // Handle modal input if one exists
                if (modalInputStack.Count > 0 && !modalEventHandled)
                {
                    var modalInput = modalInputStack.Peek();

                    if (modalInput != null)
                    {
                        modalEventHandled = true;

                        // If there is a focused object in the hierarchy of the modal handler, start the event bubble there
                        if (focusedObject != null && focusedObject.transform.IsChildOf(modalInput.transform))
                        {
                            ExecuteEvents.ExecuteHierarchy(focusedObject, baseInputEventData, eventHandler);

                            if (baseInputEventData.used)
                            {
                                return;
                            }
                        }
                        // Otherwise, just invoke the event on the modal handler itself
                        else
                        {
                            ExecuteEvents.ExecuteHierarchy(modalInput, baseInputEventData, eventHandler);

                            if (baseInputEventData.used)
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("ModalInput GameObject reference was null!\nDid this GameObject get destroyed?");
                    }
                }

                // If event was not handled by modal, pass it on to the current focused object
                if (focusedObject != null)
                {
                    ExecuteEvents.ExecuteHierarchy(focusedObject, baseInputEventData, eventHandler);

                    if (baseInputEventData.used)
                    {
                        return;
                    }
                }
            }

            // If event was not handled by the focused object, pass it on to any fallback handlers
            if (fallbackInputStack.Count > 0)
            {
                var fallbackInput = fallbackInputStack.Peek();

                if (fallbackInput != null)
                {
                    ExecuteEvents.ExecuteHierarchy(fallbackInput, baseInputEventData, eventHandler);

                    if (baseInputEventData.used)
                    {
                        // return;
                    }
                }
            }
        }

        /// <summary>
        /// Register a <see cref="GameObject"/> to listen to events that will receive all input events, regardless
        /// of which other <see cref="GameObject"/>s might have handled the event beforehand.
        /// </summary>
        /// <remarks>Useful for listening to events when the <see cref="GameObject"/> is currently not being raycasted against by the <see cref="FocusProvider"/>.</remarks>
        /// <param name="listener">Listener to add.</param>
        public override void Register(GameObject listener)
        {
            base.Register(listener);
        }

        /// <summary>
        /// Unregister a <see cref="GameObject"/> from listening to input events.
        /// </summary>
        /// <param name="listener"></param>
        public override void Unregister(GameObject listener)
        {
            base.Unregister(listener);
        }

        #endregion IEventSystemManager Implementation

        #region Input Disabled Options

        /// <summary>
        /// Push a disabled input state onto the input manager.
        /// While input is disabled no events will be sent out and the cursor displays
        /// a waiting animation.
        /// </summary>
        public void PushInputDisable()
        {
            ++disabledRefCount;

            if (disabledRefCount == 1)
            {
                InputDisabled?.Invoke();

                if (GazeProvider != null)
                {
                    GazeProvider.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Pop disabled input state. When the last disabled state is 
        /// popped off the stack input will be re-enabled.
        /// </summary>
        public void PopInputDisable()
        {
            --disabledRefCount;
            Debug.Assert(disabledRefCount >= 0, "Tried to pop more input disable than the amount pushed.");

            if (disabledRefCount == 0)
            {
                InputEnabled?.Invoke();

                if (GazeProvider != null)
                {
                    GazeProvider.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Clear the input disable stack, which will immediately re-enable input.
        /// </summary>
        public void ClearInputDisableStack()
        {
            bool wasInputDisabled = disabledRefCount > 0;
            disabledRefCount = 0;

            if (wasInputDisabled)
            {
                InputEnabled?.Invoke();

                if (GazeProvider != null)
                {
                    GazeProvider.Enabled = true;
                }
            }
        }

        #endregion Input Disabled Options

        #region Modal Input Options

        /// <summary>
        /// Push a game object into the modal input stack. Any input handlers
        /// on the game object are given priority to input events before any focused objects.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        public void PushModalInputHandler(GameObject inputHandler)
        {
            modalInputStack.Push(inputHandler);
        }

        /// <summary>
        /// Remove the last game object from the modal input stack.
        /// </summary>
        public void PopModalInputHandler()
        {
            if (modalInputStack.Count > 0)
            {
                modalInputStack.Pop();

            }
        }

        /// <summary>
        /// Clear all modal input handlers off the stack.
        /// </summary>
        public void ClearModalInputStack()
        {
            modalInputStack.Clear();
        }

        #endregion Modal Input Options

        #region Fallback Input Handler Options

        /// <summary>
        /// Push a game object into the fallback input stack. Any input handlers on
        /// the game object are given input events when no modal or focused objects consume the event.
        /// </summary>
        /// <param name="inputHandler">The input handler to push</param>
        public void PushFallbackInputHandler(GameObject inputHandler)
        {
            fallbackInputStack.Push(inputHandler);
        }

        /// <summary>
        /// Remove the last game object from the fallback input stack.
        /// </summary>
        public void PopFallbackInputHandler()
        {
            fallbackInputStack.Pop();
        }

        /// <summary>
        /// Clear all fallback input handlers off the stack.
        /// </summary>
        public void ClearFallbackInputStack()
        {
            fallbackInputStack.Clear();
        }

        #endregion Fallback Input Handler Options

        #region IMixedRealityController Utilities

        /// <inheritdoc />
        public bool TryGetController(IMixedRealityInputSource inputSource, out IMixedRealityController controller)
        {
            foreach (IMixedRealityController mixedRealityController in DetectedControllers)
            {
                if (inputSource.SourceId == mixedRealityController.InputSource.SourceId)
                {
                    controller = mixedRealityController;
                    return true;
                }
            }

            controller = null;
            return false;
        }

        #endregion IMixedRealityController Utilities

        #region Input Events

        #region Input Source Events

        /// <inheritdoc />
        public uint GenerateNewSourceId()
        {
            var newId = (uint)UnityEngine.Random.Range(1, int.MaxValue);

            foreach (var inputSource in DetectedInputSources)
            {
                if (inputSource.SourceId == newId)
                {
                    return GenerateNewSourceId();
                }
            }

            return newId;
        }

        /// <inheritdoc />
        public IMixedRealityInputSource RequestNewGenericInputSource(string name, IMixedRealityPointer[] pointers = null) => new BaseGenericInputSource(name, pointers);

        #region Input Source State Events

        /// <inheritdoc />
        public void RaiseSourceDetected(IMixedRealityInputSource source, IMixedRealityController controller = null)
        {
            // Create input event
            sourceStateEventData.Initialize(source, controller);

            Debug.Assert(!DetectedInputSources.Contains(source), $"{source.SourceName} has already been registered with the Input Manager!");

            DetectedInputSources.Add(source);

            if (controller != null)
            {
                DetectedControllers.Add(controller);
            }

            FocusProvider?.OnSourceDetected(sourceStateEventData);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceDetectedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySourceStateHandler> OnSourceDetectedEventHandler =
            delegate (IMixedRealitySourceStateHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceDetected(casted);
            };

        /// <inheritdoc />
        public void RaiseSourceLost(IMixedRealityInputSource source, IMixedRealityController controller = null)
        {
            // Create input event
            sourceStateEventData.Initialize(source, controller);

            Debug.Assert(DetectedInputSources.Contains(source), $"{source.SourceName} was never registered with the Input Manager!");

            DetectedInputSources.Remove(source);

            if (controller != null)
            {
                DetectedControllers.Remove(controller);
            }

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceStateEventData, OnSourceLostEventHandler);

            FocusProvider?.OnSourceLost(sourceStateEventData);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySourceStateHandler> OnSourceLostEventHandler =
            delegate (IMixedRealitySourceStateHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourceStateEventData>(eventData);
                handler.OnSourceLost(casted);
            };

        #endregion Input Source State Events

        #region Input Source Pose Events

        /// <inheritdoc />
        public void RaiseSourceTrackingStateChanged(IMixedRealityInputSource source, IMixedRealityController controller, TrackingState state)
        {
            // Create input event
            sourceTrackingEventData.Initialize(source, controller, state);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceTrackingEventData, OnSourceTrackingChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySourcePoseHandler> OnSourceTrackingChangedEventHandler =
            delegate (IMixedRealitySourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<TrackingState>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseSourcePositionChanged(IMixedRealityInputSource source, IMixedRealityController controller, Vector2 position)
        {
            // Create input event
            sourceVector2EventData.Initialize(source, controller, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceVector2EventData, OnSourcePoseVector2ChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySourcePoseHandler> OnSourcePoseVector2ChangedEventHandler =
            delegate (IMixedRealitySourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<Vector2>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseSourcePositionChanged(IMixedRealityInputSource source, IMixedRealityController controller, Vector3 position)
        {
            // Create input event
            sourcePositionEventData.Initialize(source, controller, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePositionEventData, OnSourcePositionChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySourcePoseHandler> OnSourcePositionChangedEventHandler =
            delegate (IMixedRealitySourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<Vector3>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseSourceRotationChanged(IMixedRealityInputSource source, IMixedRealityController controller, Quaternion rotation)
        {
            // Create input event
            sourceRotationEventData.Initialize(source, controller, rotation);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourceRotationEventData, OnSourceRotationChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySourcePoseHandler> OnSourceRotationChangedEventHandler =
            delegate (IMixedRealitySourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<Quaternion>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseSourcePoseChanged(IMixedRealityInputSource source, IMixedRealityController controller, MixedRealityPose position)
        {
            // Create input event
            sourcePoseEventData.Initialize(source, controller, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(sourcePoseEventData, OnSourcePoseChangedEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySourcePoseHandler> OnSourcePoseChangedEventHandler =
            delegate (IMixedRealitySourcePoseHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SourcePoseEventData<MixedRealityPose>>(eventData);
                handler.OnSourcePoseChanged(casted);
            };

        #endregion Input Source Pose Events

        #endregion Input Source Events

        #region Focus Events

        /// <inheritdoc />
        public void RaisePreFocusChanged(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            focusEventData.Initialize(pointer, oldFocusedObject, newFocusedObject);

            // Raise Focus Events on the old and new focused objects.
            if (oldFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(oldFocusedObject, focusEventData, OnPreFocusChangedHandler);
            }

            if (newFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(newFocusedObject, focusEventData, OnPreFocusChangedHandler);
            }

            // Raise Focus Events on the pointers cursor if it has one.
            if (pointer.BaseCursor != null)
            {
                try
                {
                    // When shutting down a game, we can sometime get old references to game objects that have been cleaned up.
                    // We'll ignore when this happens.
                    ExecuteEvents.ExecuteHierarchy(pointer.BaseCursor.GameObjectReference, focusEventData, OnPreFocusChangedHandler);
                }
                catch (Exception)
                {
                    // ignored.
                }
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusChangedHandler> OnPreFocusChangedHandler =
            delegate (IMixedRealityFocusChangedHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnBeforeFocusChange(casted);
            };

        /// <inheritdoc />
        public void RaiseFocusChanged(IMixedRealityPointer pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            focusEventData.Initialize(pointer, oldFocusedObject, newFocusedObject);

            // Raise Focus Events on the old and new focused objects.
            if (oldFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(oldFocusedObject, focusEventData, OnFocusChangedHandler);
            }

            if (newFocusedObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(newFocusedObject, focusEventData, OnFocusChangedHandler);
            }

            // Raise Focus Events on the pointers cursor if it has one.
            if (pointer.BaseCursor != null)
            {
                try
                {
                    // When shutting down a game, we can sometime get old references to game objects that have been cleaned up.
                    // We'll ignore when this happens.
                    ExecuteEvents.ExecuteHierarchy(pointer.BaseCursor.GameObjectReference, focusEventData, OnFocusChangedHandler);
                }
                catch (Exception)
                {
                    // ignored.
                }
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusChangedHandler> OnFocusChangedHandler =
            delegate (IMixedRealityFocusChangedHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnFocusChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseFocusEnter(IMixedRealityPointer pointer, GameObject focusedObject)
        {
            focusEventData.Initialize(pointer);

            ExecuteEvents.ExecuteHierarchy(focusedObject, focusEventData, OnFocusEnterEventHandler);
            ExecuteEvents.ExecuteHierarchy(focusedObject, focusEventData, ExecuteEvents.selectHandler);

            if (FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicEventData))
            {
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicEventData, ExecuteEvents.pointerEnterHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusHandler> OnFocusEnterEventHandler =
            delegate (IMixedRealityFocusHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnFocusEnter(casted);
            };

        /// <inheritdoc />
        public void RaiseFocusExit(IMixedRealityPointer pointer, GameObject unfocusedObject)
        {
            focusEventData.Initialize(pointer);

            ExecuteEvents.ExecuteHierarchy(unfocusedObject, focusEventData, OnFocusExitEventHandler);
            ExecuteEvents.ExecuteHierarchy(unfocusedObject, focusEventData, ExecuteEvents.deselectHandler);

            if (FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicEventData))
            {
                ExecuteEvents.ExecuteHierarchy(unfocusedObject, graphicEventData, ExecuteEvents.pointerExitHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusHandler> OnFocusExitEventHandler =
            delegate (IMixedRealityFocusHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
                handler.OnFocusExit(casted);
            };

        #endregion Focus Events

        #region Pointers

        #region Pointer Down

        private static readonly ExecuteEvents.EventFunction<IMixedRealityPointerHandler> OnPointerDownEventHandler =
            delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<MixedRealityPointerEventData>(eventData);
                handler.OnPointerDown(casted);
            };

        /// <inheritdoc />
        public void RaisePointerDown(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, IMixedRealityInputSource inputSource = null)
        {
            // Create input event
            pointerEventData.Initialize(pointer, inputAction, inputSource);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(pointerEventData, OnPointerDownEventHandler);

            var focusedObject = pointer.Result.CurrentPointerTarget;

            if (focusedObject != null &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicInputEventData, ExecuteEvents.pointerDownHandler);
            }
        }

        #endregion Pointer Down

        #region Pointer Click

        private static readonly ExecuteEvents.EventFunction<IMixedRealityPointerHandler> OnInputClickedEventHandler =
            delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<MixedRealityPointerEventData>(eventData);
                handler.OnPointerClicked(casted);
            };

        /// <inheritdoc />
        public void RaisePointerClicked(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, int count, IMixedRealityInputSource inputSource = null)
        {
            // Create input event
            pointerEventData.Initialize(pointer, inputAction, inputSource, count);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(pointerEventData, OnInputClickedEventHandler);

            // NOTE: In Unity UI, a "click" happens on every pointer up, so we have RaisePointerUp call the pointerClickHandler.
        }

        #endregion Pointer Click

        #region Pointer Up

        private static readonly ExecuteEvents.EventFunction<IMixedRealityPointerHandler> OnPointerUpEventHandler =
            delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<MixedRealityPointerEventData>(eventData);
                handler.OnPointerUp(casted);
            };

        /// <inheritdoc />
        public void RaisePointerUp(IMixedRealityPointer pointer, MixedRealityInputAction inputAction, IMixedRealityInputSource inputSource = null)
        {
            // Create input event
            pointerEventData.Initialize(pointer, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(pointerEventData, OnPointerUpEventHandler);

            var focusedObject = pointer.Result.CurrentPointerTarget;

            if (focusedObject != null &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicInputEventData, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicInputEventData, ExecuteEvents.pointerClickHandler);

                graphicInputEventData.Clear();
            }
        }

        #endregion Pointer Up

        /// <inheritdoc />
        public void RaisePointerScroll(IMixedRealityPointer pointer, MixedRealityInputAction scrollAction, Vector2 scrollDelta, IMixedRealityInputSource inputSource = null)
        {
            vector2InputEventData.Initialize(inputSource ?? pointer.InputSourceParent, Handedness.None, scrollAction, scrollDelta);

            HandleEvent(vector2InputEventData, OnTwoDoFInputChanged);

            var focusedObject = pointer.Result.CurrentPointerTarget;

            if (focusedObject != null &&
                FocusProvider.TryGetSpecificPointerGraphicEventData(pointer, out var graphicInputEventData))
            {
                graphicInputEventData.scrollDelta = scrollDelta;
                ExecuteEvents.ExecuteHierarchy(focusedObject, graphicInputEventData, ExecuteEvents.scrollHandler);
            }
        }

        #endregion Pointers

        #region Generic Input Events

        #region Input Down

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler> OnInputDownEventHandler =
            delegate (IMixedRealityInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputDown(casted);
            };

        /// <inheritdoc />
        public void RaiseOnInputDown(IMixedRealityInputSource source, MixedRealityInputAction inputAction)
        {
            RaiseOnInputDown(source, Handedness.None, inputAction);
        }

        /// <inheritdoc />
        public void RaiseOnInputDown(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            inputAction = ProcessRules(inputAction, true);

            // Create input event
            inputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputDownEventHandler);
        }

        #endregion Input Down

        #region Input Pressed

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, MixedRealityInputAction inputAction)
        {
            RaiseOnInputPressed(source, Handedness.None, inputAction);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            inputAction = ProcessRules(inputAction, true);

            // Create input event
            floatInputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(floatInputEventData, SingleAxisInputEventHandler);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, MixedRealityInputAction inputAction, float pressAmount)
        {
            RaiseOnInputPressed(source, Handedness.None, inputAction, pressAmount);
        }

        /// <inheritdoc />
        public void RaiseOnInputPressed(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, float pressAmount)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            inputAction = ProcessRules(inputAction, pressAmount);

            // Create input event
            floatInputEventData.Initialize(source, handedness, inputAction, pressAmount);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(floatInputEventData, SingleAxisInputEventHandler);
        }

        #endregion Input Pressed

        #region Input Up

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler> OnInputUpEventHandler =
            delegate (IMixedRealityInputHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnInputUp(casted);
            };

        /// <inheritdoc />
        public void RaiseOnInputUp(IMixedRealityInputSource source, MixedRealityInputAction inputAction)
        {
            RaiseOnInputUp(source, Handedness.None, inputAction);
        }

        /// <inheritdoc />
        public void RaiseOnInputUp(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            inputAction = ProcessRules(inputAction, false);

            // Create input event
            inputEventData.Initialize(source, handedness, inputAction);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(inputEventData, OnInputUpEventHandler);
        }

        #endregion Input Up

        #region Input Position Changed

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler<float>> SingleAxisInputEventHandler =
            delegate (IMixedRealityInputHandler<float> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<float>>(eventData);
                handler.OnInputChanged(casted);
            };

        /// <inheritdoc />
        public void RaisePositionInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, float inputPosition)
        {
            RaisePositionInputChanged(source, Handedness.None, inputAction, inputPosition);
        }

        /// <inheritdoc />
        public void RaisePositionInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, float inputPosition)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            inputAction = ProcessRules(inputAction, inputPosition);

            // Create input event
            floatInputEventData.Initialize(source, handedness, inputAction, inputPosition);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(floatInputEventData, SingleAxisInputEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler<Vector2>> OnTwoDoFInputChanged =
            delegate (IMixedRealityInputHandler<Vector2> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector2>>(eventData);
                handler.OnInputChanged(casted);
            };

        /// <inheritdoc />
        public void RaisePositionInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, Vector2 inputPosition)
        {
            RaisePositionInputChanged(source, Handedness.None, inputAction, inputPosition);
        }

        /// <inheritdoc />
        public void RaisePositionInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Vector2 inputPosition)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            inputAction = ProcessRules(inputAction, inputPosition);

            // Create input event
            vector2InputEventData.Initialize(source, handedness, inputAction, inputPosition);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(vector2InputEventData, OnTwoDoFInputChanged);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler<Vector3>> OnPositionInputChanged =
            delegate (IMixedRealityInputHandler<Vector3> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnInputChanged(casted);
            };

        /// <inheritdoc />
        public void RaisePositionInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, Vector3 position)
        {
            RaisePositionInputChanged(source, Handedness.None, inputAction, position);
        }

        /// <inheritdoc />
        public void RaisePositionInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Vector3 position)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            inputAction = ProcessRules(inputAction, position);

            // Create input event
            positionInputEventData.Initialize(source, handedness, inputAction, position);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnPositionInputChanged);
        }

        #endregion Input Position Changed

        #region Input Rotation Changed

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler<Quaternion>> OnRotationInputChanged =
            delegate (IMixedRealityInputHandler<Quaternion> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Quaternion>>(eventData);
                handler.OnInputChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseRotationInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, Quaternion rotation)
        {
            RaiseRotationInputChanged(source, Handedness.None, inputAction, rotation);
        }

        /// <inheritdoc />
        public void RaiseRotationInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, Quaternion rotation)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            inputAction = ProcessRules(inputAction, rotation);

            // Create input event
            rotationInputEventData.Initialize(source, handedness, inputAction, rotation);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(positionInputEventData, OnRotationInputChanged);
        }

        #endregion Input Rotation Changed

        #region Input Pose Changed

        private static readonly ExecuteEvents.EventFunction<IMixedRealityInputHandler<MixedRealityPose>> OnPoseInputChanged =
            delegate (IMixedRealityInputHandler<MixedRealityPose> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<MixedRealityPose>>(eventData);
                handler.OnInputChanged(casted);
            };

        /// <inheritdoc />
        public void RaisePoseInputChanged(IMixedRealityInputSource source, MixedRealityInputAction inputAction, MixedRealityPose inputData)
        {
            RaisePoseInputChanged(source, Handedness.None, inputAction, inputData);
        }

        /// <inheritdoc />
        public void RaisePoseInputChanged(IMixedRealityInputSource source, Handedness handedness, MixedRealityInputAction inputAction, MixedRealityPose inputData)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            inputAction = ProcessRules(inputAction, inputData);

            // Create input event
            poseInputEventData.Initialize(source, handedness, inputAction, inputData);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(poseInputEventData, OnPoseInputChanged);
        }

        #endregion Input Pose Changed

        #endregion Generic Input Events

        #region Gesture Events

        private static readonly ExecuteEvents.EventFunction<IMixedRealityGestureHandler> OnGestureStarted =
            delegate (IMixedRealityGestureHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnGestureStarted(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureStarted(IMixedRealityController controller, MixedRealityInputAction action)
        {
            Debug.Assert(DetectedInputSources.Contains(controller.InputSource));

            action = ProcessRules(action, true);
            inputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action);
            HandleEvent(inputEventData, OnGestureStarted);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityGestureHandler> OnGestureUpdated =
            delegate (IMixedRealityGestureHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnGestureUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action)
        {
            Debug.Assert(DetectedInputSources.Contains(controller.InputSource));
            action = ProcessRules(action, true);
            inputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action);
            HandleEvent(inputEventData, OnGestureUpdated);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityGestureHandler<Vector2>> OnGestureVector2PositionUpdated =
            delegate (IMixedRealityGestureHandler<Vector2> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector2>>(eventData);
                handler.OnGestureUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, Vector2 inputData)
        {
            Debug.Assert(DetectedInputSources.Contains(controller.InputSource));
            action = ProcessRules(action, inputData);
            vector2InputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(vector2InputEventData, OnGestureVector2PositionUpdated);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityGestureHandler<Vector3>> OnGesturePositionUpdated =
            delegate (IMixedRealityGestureHandler<Vector3> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnGestureUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, Vector3 inputData)
        {
            Debug.Assert(DetectedInputSources.Contains(controller.InputSource));
            action = ProcessRules(action, inputData);
            positionInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(positionInputEventData, OnGesturePositionUpdated);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityGestureHandler<Quaternion>> OnGestureRotationUpdated =
            delegate (IMixedRealityGestureHandler<Quaternion> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Quaternion>>(eventData);
                handler.OnGestureUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, Quaternion inputData)
        {
            Debug.Assert(DetectedInputSources.Contains(controller.InputSource));
            action = ProcessRules(action, inputData);
            rotationInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(rotationInputEventData, OnGestureRotationUpdated);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityGestureHandler<MixedRealityPose>> OnGesturePoseUpdated =
            delegate (IMixedRealityGestureHandler<MixedRealityPose> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<MixedRealityPose>>(eventData);
                handler.OnGestureUpdated(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureUpdated(IMixedRealityController controller, MixedRealityInputAction action, MixedRealityPose inputData)
        {
            Debug.Assert(DetectedInputSources.Contains(controller.InputSource));
            action = ProcessRules(action, inputData);
            poseInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(poseInputEventData, OnGesturePoseUpdated);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityGestureHandler> OnGestureCompleted =
            delegate (IMixedRealityGestureHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnGestureCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action)
        {
            Debug.Assert(DetectedInputSources.Contains(controller.InputSource));
            action = ProcessRules(action, false);
            inputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action);
            HandleEvent(inputEventData, OnGestureCompleted);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityGestureHandler<Vector2>> OnGestureVector2PositionCompleted =
            delegate (IMixedRealityGestureHandler<Vector2> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector2>>(eventData);
                handler.OnGestureCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, Vector2 inputData)
        {
            Debug.Assert(DetectedInputSources.Contains(controller.InputSource));
            action = ProcessRules(action, inputData);
            vector2InputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(vector2InputEventData, OnGestureVector2PositionCompleted);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityGestureHandler<Vector3>> OnGesturePositionCompleted =
            delegate (IMixedRealityGestureHandler<Vector3> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Vector3>>(eventData);
                handler.OnGestureCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, Vector3 inputData)
        {
            Debug.Assert(DetectedInputSources.Contains(controller.InputSource));
            action = ProcessRules(action, inputData);
            positionInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(positionInputEventData, OnGesturePositionCompleted);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityGestureHandler<Quaternion>> OnGestureRotationCompleted =
            delegate (IMixedRealityGestureHandler<Quaternion> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<Quaternion>>(eventData);
                handler.OnGestureCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, Quaternion inputData)
        {
            Debug.Assert(DetectedInputSources.Contains(controller.InputSource));
            action = ProcessRules(action, inputData);
            rotationInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(rotationInputEventData, OnGestureRotationCompleted);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityGestureHandler<MixedRealityPose>> OnGesturePoseCompleted =
            delegate (IMixedRealityGestureHandler<MixedRealityPose> handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData<MixedRealityPose>>(eventData);
                handler.OnGestureCompleted(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureCompleted(IMixedRealityController controller, MixedRealityInputAction action, MixedRealityPose inputData)
        {
            Debug.Assert(DetectedInputSources.Contains(controller.InputSource));
            action = ProcessRules(action, inputData);
            poseInputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action, inputData);
            HandleEvent(poseInputEventData, OnGesturePoseCompleted);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityGestureHandler> OnGestureCanceled =
            delegate (IMixedRealityGestureHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<InputEventData>(eventData);
                handler.OnGestureCanceled(casted);
            };

        /// <inheritdoc />
        public void RaiseGestureCanceled(IMixedRealityController controller, MixedRealityInputAction action)
        {
            Debug.Assert(DetectedInputSources.Contains(controller.InputSource));
            action = ProcessRules(action, false);
            inputEventData.Initialize(controller.InputSource, controller.ControllerHandedness, action);
            HandleEvent(inputEventData, OnGestureCanceled);
        }

        #endregion Gesture Events

        #region Speech Keyword Events

        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpeechHandler> OnSpeechKeywordRecognizedEventHandler =
            delegate (IMixedRealitySpeechHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<SpeechEventData>(eventData);
                handler.OnSpeechKeywordRecognized(casted);
            };

        /// <inheritdoc />
        public void RaiseSpeechCommandRecognized(IMixedRealityInputSource source, MixedRealityInputAction inputAction, RecognitionConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, string text)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            // Create input event
            speechEventData.Initialize(source, inputAction, confidence, phraseDuration, phraseStartTime, text);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(speechEventData, OnSpeechKeywordRecognizedEventHandler);
        }

        #endregion Speech Keyword Events

        #region Dictation Events

        private static readonly ExecuteEvents.EventFunction<IMixedRealityDictationHandler> OnDictationHypothesisEventHandler =
            delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationHypothesis(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationHypothesis(IMixedRealityInputSource source, string dictationHypothesis, AudioClip dictationAudioClip = null)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            // Create input event
            dictationEventData.Initialize(source, dictationHypothesis, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationHypothesisEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityDictationHandler> OnDictationResultEventHandler =
            delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationResult(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationResult(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationResultEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityDictationHandler> OnDictationCompleteEventHandler =
            delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationComplete(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationComplete(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationCompleteEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityDictationHandler> OnDictationErrorEventHandler =
            delegate (IMixedRealityDictationHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<DictationEventData>(eventData);
                handler.OnDictationError(casted);
            };

        /// <inheritdoc />
        public void RaiseDictationError(IMixedRealityInputSource source, string dictationResult, AudioClip dictationAudioClip = null)
        {
            Debug.Assert(DetectedInputSources.Contains(source));

            // Create input event
            dictationEventData.Initialize(source, dictationResult, dictationAudioClip);

            // Pass handler through HandleEvent to perform modal/fallback logic
            HandleEvent(dictationEventData, OnDictationErrorEventHandler);
        }

        #endregion Dictation Events

        #endregion Input Events

        #region Rules

        private static MixedRealityInputAction ProcessRules_Internal<T1, T2>(MixedRealityInputAction inputAction, T1[] inputActionRules, T2 criteria) where T1 : struct, IInputActionRule<T2>
        {
            for (int i = 0; i < inputActionRules.Length; i++)
            {
                if (inputActionRules[i].BaseAction == inputAction && inputActionRules[i].Criteria.Equals(criteria))
                {
                    if (inputActionRules[i].RuleAction == inputAction)
                    {
                        Debug.LogError("Input Action Rule cannot be the same as the rule's Base Action!");
                        return inputAction;
                    }

                    if (inputActionRules[i].BaseAction.AxisConstraint != inputActionRules[i].RuleAction.AxisConstraint)
                    {
                        Debug.LogError("Input Action Rule doesn't have the same Axis Constraint as the Base Action!");
                        return inputAction;
                    }

                    return inputActionRules[i].RuleAction;
                }
            }

            return inputAction;
        }

        private MixedRealityInputAction ProcessRules(MixedRealityInputAction inputAction, bool criteria)
        {
            if (CurrentInputActionRulesProfile != null && CurrentInputActionRulesProfile.InputActionRulesDigital?.Length > 0)
            {
                return ProcessRules_Internal(inputAction, CurrentInputActionRulesProfile.InputActionRulesDigital, criteria);
            }

            return inputAction;
        }

        private MixedRealityInputAction ProcessRules(MixedRealityInputAction inputAction, float criteria)
        {
            if (CurrentInputActionRulesProfile != null && CurrentInputActionRulesProfile.InputActionRulesSingleAxis?.Length > 0)
            {
                return ProcessRules_Internal(inputAction, CurrentInputActionRulesProfile.InputActionRulesSingleAxis, criteria);
            }

            return inputAction;
        }

        private MixedRealityInputAction ProcessRules(MixedRealityInputAction inputAction, Vector2 criteria)
        {
            if (CurrentInputActionRulesProfile != null && CurrentInputActionRulesProfile.InputActionRulesDualAxis?.Length > 0)
            {
                return ProcessRules_Internal(inputAction, CurrentInputActionRulesProfile.InputActionRulesDualAxis, criteria);
            }

            return inputAction;
        }

        private MixedRealityInputAction ProcessRules(MixedRealityInputAction inputAction, Vector3 criteria)
        {
            if (CurrentInputActionRulesProfile != null && CurrentInputActionRulesProfile.InputActionRulesVectorAxis?.Length > 0)
            {
                return ProcessRules_Internal(inputAction, CurrentInputActionRulesProfile.InputActionRulesVectorAxis, criteria);
            }

            return inputAction;
        }

        private MixedRealityInputAction ProcessRules(MixedRealityInputAction inputAction, Quaternion criteria)
        {
            if (CurrentInputActionRulesProfile != null && CurrentInputActionRulesProfile.InputActionRulesQuaternionAxis?.Length > 0)
            {
                return ProcessRules_Internal(inputAction, CurrentInputActionRulesProfile.InputActionRulesQuaternionAxis, criteria);
            }

            return inputAction;
        }

        private MixedRealityInputAction ProcessRules(MixedRealityInputAction inputAction, MixedRealityPose criteria)
        {
            if (CurrentInputActionRulesProfile != null && CurrentInputActionRulesProfile.InputActionRulesPoseAxis?.Length > 0)
            {
                return ProcessRules_Internal(inputAction, CurrentInputActionRulesProfile.InputActionRulesPoseAxis, criteria);
            }

            return inputAction;
        }

        #endregion Rules
    }
}
