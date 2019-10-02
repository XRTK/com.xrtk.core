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
    public class InputActionTrigger
    {
        [SerializeField]
        private EqualityOperator eqOperator = EqualityOperator.None;

        public EqualityOperator Operator => eqOperator;

        [SerializeField]
        private MixedRealityInputAction action = MixedRealityInputAction.None;

        public MixedRealityInputAction Action => action;
    }

    public enum EqualityOperator
    {
        None = 0,
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
    }
}