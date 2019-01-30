using XRTK.Attributes;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.Providers.Controllers;
using UnityEngine;

namespace XRTK.SDK.UX.Controllers
{
    public class WindowsMixedRealityControllerVisualizer : DefaultMixedRealityControllerVisualizer
    {
        [Prefab]
        [SerializeField]
        private GameObject touchpadTouchVisualizer = null;

        /// <summary>
        /// 
        /// </summary>
        public GameObject TouchpadTouchVisualizer
        {
            get => touchpadTouchVisualizer;
            set => touchpadTouchVisualizer = value;
        }

        private readonly Quaternion inverseRotation = Quaternion.Euler(0f, 180f, 0f);

        private GameObject home;
        private Transform homePressed;
        private Transform homeUnpressed;
        private GameObject menu;
        private Transform menuPressed;
        private Transform menuUnpressed;
        private GameObject grasp;
        private Transform graspPressed;
        private Transform graspUnpressed;
        private GameObject thumbstickPress;
        private Transform thumbstickPressed;
        private Transform thumbstickUnpressed;
        private GameObject thumbstickX;
        private Transform thumbstickXMin;
        private Transform thumbstickXMax;
        private GameObject thumbstickY;
        private Transform thumbstickYMin;
        private Transform thumbstickYMax;
        private GameObject select;
        private Transform selectPressed;
        private Transform selectUnpressed;
        private GameObject touchpadPress;
        private Transform touchpadPressed;
        private Transform touchpadUnpressed;
        private GameObject touchpadTouchX;
        private Transform touchpadTouchXMin;
        private Transform touchpadTouchXMax;
        private GameObject touchpadTouchY;
        private Transform touchpadTouchYMin;
        private Transform touchpadTouchYMax;
        private GameObject pointingPose;

        // These values are used to determine if a button's state has changed.
        private bool wasGrasped;
        private bool wasMenuPressed;
        private bool wasHomePressed;
        private bool wasThumbstickPressed;
        private bool wasTouchpadPressed;
        private bool wasTouchpadTouched;
        private Vector2 lastThumbstickPosition;
        private Vector2 lastTouchpadPosition;
        private double lastSelectPressedAmount;

        public override IMixedRealityController Controller
        {
            get => base.Controller;
            set
            {
                GetTransformData(transform);
                base.Controller = value;
            }
        }

        private void GetTransformData(Transform _transform)
        {
            for (int i = 0; i < _transform.childCount; i++)
            {
                var child = _transform.GetChild(i);

                // Animation bounds are named in two pairs:
                // pressed/unpressed and min/max. There is also a value
                // transform, which is the transform to modify to
                // animate the interactions. We also look for the
                // touch transform, in order to spawn the touchpadTouched
                // visualizer.
                switch (child.name.ToLower())
                {
                    case "pointing_pose":
                        pointingPose = child.gameObject;
                        PoseDriver = null;
                        pointingPose.transform.parent = transform;
                        transform.GetChild(0).parent = pointingPose.transform;
                        PoseDriver = pointingPose.transform;
                        transform.localPosition = Vector3.zero;
                        transform.localRotation = Quaternion.identity;
                        break;
                    case "pressed":
                        switch (child.parent.name.ToLower())
                        {
                            case "home":
                                homePressed = child;
                                break;
                            case "menu":
                                menuPressed = child;
                                break;
                            case "grasp":
                                graspPressed = child;
                                break;
                            case "select":
                                selectPressed = child;
                                break;
                            case "thumbstick_press":
                                thumbstickPressed = child;
                                break;
                            case "touchpad_press":
                                touchpadPressed = child;
                                break;
                        }
                        break;
                    case "unpressed":
                        switch (child.parent.name.ToLower())
                        {
                            case "home":
                                homeUnpressed = child;
                                break;
                            case "menu":
                                menuUnpressed = child;
                                break;
                            case "grasp":
                                graspUnpressed = child;
                                break;
                            case "select":
                                selectUnpressed = child;
                                break;
                            case "thumbstick_press":
                                thumbstickUnpressed = child;
                                break;
                            case "touchpad_press":
                                touchpadUnpressed = child;
                                break;
                        }
                        break;
                    case "min":
                        switch (child.parent.name.ToLower())
                        {
                            case "thumbstick_x":
                                thumbstickXMin = child;
                                break;
                            case "thumbstick_y":
                                thumbstickYMin = child;
                                break;
                            case "touchpad_touch_x":
                                touchpadTouchXMin = child;
                                break;
                            case "touchpad_touch_y":
                                touchpadTouchYMin = child;
                                break;
                        }
                        break;
                    case "max":
                        switch (child.parent.name.ToLower())
                        {
                            case "thumbstick_x":
                                thumbstickXMax = child;
                                break;
                            case "thumbstick_y":
                                thumbstickYMax = child;
                                break;
                            case "touchpad_touch_x":
                                touchpadTouchXMax = child;
                                break;
                            case "touchpad_touch_y":
                                touchpadTouchYMax = child;
                                break;
                        }
                        break;
                    case "value":
                        switch (child.parent.name.ToLower())
                        {
                            case "home":
                                home = child.gameObject;
                                break;
                            case "menu":
                                menu = child.gameObject;
                                break;
                            case "grasp":
                                grasp = child.gameObject;
                                break;
                            case "select":
                                select = child.gameObject;
                                break;
                            case "thumbstick_press":
                                thumbstickPress = child.gameObject;
                                break;
                            case "thumbstick_x":
                                thumbstickX = child.gameObject;
                                break;
                            case "thumbstick_y":
                                thumbstickY = child.gameObject;
                                break;
                            case "touchpad_press":
                                touchpadPress = child.gameObject;
                                break;
                            case "touchpad_touch_x":
                                touchpadTouchX = child.gameObject;
                                break;
                            case "touchpad_touch_y":
                                touchpadTouchY = child.gameObject;
                                break;
                        }
                        break;
                }

                GetTransformData(child);
            }
        }

        #region IMixedRealityInputHandler Implementation

        public override void OnInputDown(InputEventData eventData)
        {
            if (eventData.SourceId != Controller?.InputSource.SourceId) { return; }
        }

        public override void OnInputUp(InputEventData eventData)
        {
            if (eventData.SourceId != Controller?.InputSource.SourceId) { return; }
        }

        public override void OnInputChanged(InputEventData<float> eventData)
        {
            if (eventData.SourceId != Controller?.InputSource.SourceId) { return; }
        }

        public override void OnInputChanged(InputEventData<Vector2> eventData)
        {
            if (eventData.SourceId != Controller?.InputSource.SourceId) { return; }
        }

        public override void OnInputChanged(InputEventData<Vector3> eventData)
        {
            if (eventData.SourceId != Controller?.InputSource.SourceId) { return; }
        }

        public override void OnInputChanged(InputEventData<Quaternion> eventData)
        {
            if (eventData.SourceId != Controller?.InputSource.SourceId) { return; }
        }

        public override void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.SourceId != Controller?.InputSource.SourceId) { return; }

            if (!UseSourcePoseData &&
                PoseAction == eventData.MixedRealityInputAction)
            {
                IsTracked = true;
                TrackingState = TrackingState.Tracked;

                if (PoseDriver != null)
                {
                    PoseDriver.localPosition = eventData.InputData.Position;
                    PoseDriver.localRotation = eventData.InputData.Rotation * inverseRotation;
                }
            }
        }

        #endregion IMixedRealityInputHandler Implementation

        private void AnimateGrasp(bool isGrasped)
        {
            if (grasp != null && graspPressed != null && graspUnpressed != null && isGrasped != wasGrasped)
            {
                SetLocalPositionAndRotation(grasp, isGrasped ? graspPressed : graspUnpressed);
                wasGrasped = isGrasped;
            }
        }

        private void AnimateMenu(bool isMenuPressed)
        {
            if (menu != null && menuPressed != null && menuUnpressed != null && isMenuPressed != wasMenuPressed)
            {
                SetLocalPositionAndRotation(menu, isMenuPressed ? menuPressed : menuUnpressed);
                wasMenuPressed = isMenuPressed;
            }
        }

        private void AnimateHome(bool isHomePressed)
        {
            if (home != null && homePressed != null && homeUnpressed != null && isHomePressed != wasHomePressed)
            {
                SetLocalPositionAndRotation(home, isHomePressed ? homePressed : homeUnpressed);
                wasHomePressed = isHomePressed;
            }
        }

        private void AnimateSelect(float newSelectPressedAmount)
        {
            if (select != null && selectPressed != null && selectUnpressed != null && !newSelectPressedAmount.Equals((float)lastSelectPressedAmount))
            {
                select.transform.localPosition = Vector3.Lerp(selectUnpressed.localPosition, selectPressed.localPosition, newSelectPressedAmount);
                select.transform.localRotation = Quaternion.Lerp(selectUnpressed.localRotation, selectPressed.localRotation, newSelectPressedAmount);
                lastSelectPressedAmount = newSelectPressedAmount;
            }
        }

        private void AnimateThumbstick(bool isThumbstickPressed, Vector2 newThumbstickPosition)
        {
            if (thumbstickPress != null && thumbstickPressed != null && thumbstickUnpressed != null && isThumbstickPressed != wasThumbstickPressed)
            {
                SetLocalPositionAndRotation(thumbstickPress, isThumbstickPressed ? thumbstickPressed : thumbstickUnpressed);
                wasThumbstickPressed = isThumbstickPressed;
            }

            if (thumbstickX != null && thumbstickY != null && thumbstickXMin != null && thumbstickXMax != null && thumbstickYMin != null && thumbstickYMax != null && newThumbstickPosition != lastThumbstickPosition)
            {
                Vector2 thumbstickNormalized = (newThumbstickPosition + Vector2.one) * 0.5f;

                thumbstickX.transform.localPosition = Vector3.Lerp(thumbstickXMin.localPosition, thumbstickXMax.localPosition, thumbstickNormalized.x);
                thumbstickX.transform.localRotation = Quaternion.Lerp(thumbstickXMin.localRotation, thumbstickXMax.localRotation, thumbstickNormalized.x);

                thumbstickY.transform.localPosition = Vector3.Lerp(thumbstickYMax.localPosition, thumbstickYMin.localPosition, thumbstickNormalized.y);
                thumbstickY.transform.localRotation = Quaternion.Lerp(thumbstickYMax.localRotation, thumbstickYMin.localRotation, thumbstickNormalized.y);

                lastThumbstickPosition = newThumbstickPosition;
            }
        }

        private void AnimateTouchpad(bool isTouchpadPressed, bool isTouchpadTouched, Vector2 newTouchpadPosition)
        {
            if (touchpadPress != null && touchpadPressed != null && touchpadUnpressed != null && isTouchpadPressed != wasTouchpadPressed)
            {
                SetLocalPositionAndRotation(touchpadPress, isTouchpadPressed ? touchpadPressed : touchpadUnpressed);
                wasTouchpadPressed = isTouchpadPressed;
            }

            if (TouchpadTouchVisualizer != null && isTouchpadTouched != wasTouchpadTouched)
            {
                TouchpadTouchVisualizer.SetActive(isTouchpadTouched);
                wasTouchpadTouched = isTouchpadTouched;
            }

            if (touchpadTouchX != null && touchpadTouchY != null && touchpadTouchXMin != null && touchpadTouchXMax != null && touchpadTouchYMin != null && touchpadTouchYMax != null && newTouchpadPosition != lastTouchpadPosition)
            {
                Vector2 touchpadNormalized = (newTouchpadPosition + Vector2.one) * 0.5f;

                touchpadTouchX.transform.localPosition = Vector3.Lerp(touchpadTouchXMin.localPosition, touchpadTouchXMax.localPosition, touchpadNormalized.x);
                touchpadTouchX.transform.localRotation = Quaternion.Lerp(touchpadTouchXMin.localRotation, touchpadTouchXMax.localRotation, touchpadNormalized.x);

                touchpadTouchY.transform.localPosition = Vector3.Lerp(touchpadTouchYMax.localPosition, touchpadTouchYMin.localPosition, touchpadNormalized.y);
                touchpadTouchY.transform.localRotation = Quaternion.Lerp(touchpadTouchYMax.localRotation, touchpadTouchYMin.localRotation, touchpadNormalized.y);

                lastTouchpadPosition = newTouchpadPosition;
            }
        }

        private static void SetLocalPositionAndRotation(GameObject buttonGameObject, Transform newTransform)
        {
            buttonGameObject.transform.localPosition = newTransform.localPosition;
            buttonGameObject.transform.localRotation = newTransform.localRotation;
        }
    }
}