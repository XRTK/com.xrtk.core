using System;
using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.InputSystem
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Input Action Profile", fileName = "MixedRealityInputActionProfile", order = (int)CreateProfileMenuItemIndices.InputActions)]
    public class InputActionProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private MixedRealityInputAction action = MixedRealityInputAction.None;

        public MixedRealityInputAction Action => action;

        [SerializeField]
        private InputActionTrigger[] triggers = new InputActionTrigger[0];
    }

    [Serializable]
    public struct InputActionTrigger
    {
        // TODO Add conditions to raise action

        public Action Action { get; set; }
    }
}