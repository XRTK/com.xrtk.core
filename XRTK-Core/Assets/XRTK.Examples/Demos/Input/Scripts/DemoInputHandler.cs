using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Services;
using UnityEngine;
using XRTK.SDK.Input.Handlers;

namespace XRTK.Examples.Demos.Input
{
    public class DemoInputHandler : BaseInputHandler,
            IMixedRealitySourceStateHandler,
            IMixedRealityInputHandler<float>,
            IMixedRealityInputHandler<Vector2>,
            IMixedRealityInputHandler<Vector3>,
            IMixedRealityInputHandler<Quaternion>,
            IMixedRealityInputHandler<MixedRealityPose>,
            IMixedRealityGestureHandler<Vector3>,
            IMixedRealityGestureHandler<Quaternion>,
            IMixedRealityGestureHandler<MixedRealityPose>
    {
        [SerializeField]
        [Tooltip("The action that will be used for selecting objects.")]
        private MixedRealityInputAction selectAction = MixedRealityInputAction.None;

        [SerializeField]
        [Tooltip("The action that will move the camera forward, back, left, and right.")]
        private MixedRealityInputAction movementAction = MixedRealityInputAction.None;

        [SerializeField]
        [Tooltip("The action that will pivot the camera on it's axis.")]
        private MixedRealityInputAction rotateAction = MixedRealityInputAction.None;

        [SerializeField]
        [Tooltip("The action that will move the camera up or down vertically.")]
        private MixedRealityInputAction heightAction = MixedRealityInputAction.None;

        private Vector3 newPosition = Vector3.zero;

        private Vector3 newRotation = Vector3.zero;

        #region Monobehaviour Implementation

        private void Awake()
        {
            Debug.Log($"[Awake] Is MRTK initialized? {MixedRealityToolkit.Instance != null}");
            Debug.Log($"[Awake] Is Input System initialized? {MixedRealityToolkit.InputSystem != null}");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Debug.Log($"[OnEnable] Is MRTK initialized? {MixedRealityToolkit.Instance != null}");
            Debug.Log($"[OnEnable] Is Input System initialized? {MixedRealityToolkit.InputSystem != null}");
        }

        protected override void Start()
        {
            base.Start();

            Debug.Log($"[Start] Is MRTK initialized? {MixedRealityToolkit.Instance != null}");
            Debug.Log($"[Start] Is Input System initialized? {MixedRealityToolkit.InputSystem != null}");

            if (MixedRealityToolkit.InputSystem != null)
            {
                foreach (var inputSource in MixedRealityToolkit.InputSystem.DetectedInputSources)
                {
                    Debug.Log($"[OnSourceDetected] {inputSource.SourceName}");
                }
            }
        }

        #endregion Monobehaviour Implementation

        #region IMixedRealityInputHandler Implementation

        public void OnInputUp(InputEventData eventData)
        {
            Debug.Log($"[OnInputUp] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description}");
        }

        public void OnInputDown(InputEventData eventData)
        {
            Debug.Log($"[OnInputDown] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description}");
        }

        public void OnInputChanged(InputEventData<float> eventData)
        {
            // Debug.Log($"[OnInputChanged] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description} | {eventData.InputData}");

            if (eventData.MixedRealityInputAction == heightAction)
            {
                newPosition.x = 0f;
                newPosition.y = eventData.InputData;
                newPosition.z = 0f;
                gameObject.transform.position += newPosition;
            }
        }

        public void OnInputChanged(InputEventData<Vector2> eventData)
        {
            // Debug.Log($"[OnInputChanged] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description} | {eventData.InputData}");

            if (eventData.MixedRealityInputAction == movementAction)
            {
                newPosition.x = eventData.InputData.x;
                newPosition.y = 0f;
                newPosition.z = eventData.InputData.y;
                gameObject.transform.position += newPosition;
            }
            else if (eventData.MixedRealityInputAction == rotateAction)
            {
                newRotation.x = eventData.InputData.x;
                newRotation.y = eventData.InputData.y;
            }
        }

        public void OnInputChanged(InputEventData<Vector3> eventData)
        {
            // Debug.Log($"[OnInputChanged] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description} | {eventData.InputData}");
        }

        public void OnInputChanged(InputEventData<Quaternion> eventData)
        {
            // Debug.Log($"[OnInputChanged] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description} | {eventData.InputData}");
        }

        public void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            // Debug.Log($"[OnInputChanged] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description} | {eventData.InputData}");
        }

        #endregion IMixedRealityInputHandler Implementation

        #region IMixedRealitySourceStateHandler Implementation

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            Debug.Log($"[OnSourceDetected] {eventData.InputSource.SourceName}");
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            Debug.Log($"[OnSourceLost] {eventData.InputSource.SourceName}");
        }

        #endregion IMixedRealitySourceStateHandler Implementation

        public void OnGestureStarted(InputEventData eventData)
        {
            Debug.Log($"[OnGestureStarted] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description}");
        }

        public void OnGestureUpdated(InputEventData eventData)
        {
            Debug.Log($"[OnGestureUpdated] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description}");
        }

        public void OnGestureCompleted(InputEventData eventData)
        {
            Debug.Log($"[OnGestureCompleted] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description}");
        }

        public void OnGestureUpdated(InputEventData<Vector3> eventData)
        {
            Debug.Log($"[OnGestureUpdated] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description} | {eventData.InputData}");
        }

        public void OnGestureCompleted(InputEventData<Vector3> eventData)
        {
            Debug.Log($"[OnGestureCompleted] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description} | {eventData.InputData}");
        }

        public void OnGestureUpdated(InputEventData<Quaternion> eventData)
        {
            Debug.Log($"[OnGestureUpdated] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description} | {eventData.InputData}");
        }

        public void OnGestureCompleted(InputEventData<Quaternion> eventData)
        {
            Debug.Log($"[OnGestureCompleted] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description} | {eventData.InputData}");
        }

        public void OnGestureUpdated(InputEventData<MixedRealityPose> eventData)
        {
            Debug.Log($"[OnGestureUpdated] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description} | {eventData.InputData}");
        }

        public void OnGestureCompleted(InputEventData<MixedRealityPose> eventData)
        {
            Debug.Log($"[OnGestureCompleted] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description} | {eventData.InputData}");
        }

        public void OnGestureCanceled(InputEventData eventData)
        {
            Debug.Log($"[OnGestureCanceled] {eventData.InputSource.SourceName} | {eventData.MixedRealityInputAction.Description}");
        }
    }
}