// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine.XR;

namespace XRTK.Extensions
{
    public static class InputDeviceCharacteristicsExtensions
    {
        public static InputDeviceCharacteristics SetFlag(this InputDeviceCharacteristics value, InputDeviceCharacteristics flag)
        {
            return value | flag;
        }

        public static InputDeviceCharacteristics UnsetFlag(this InputDeviceCharacteristics value, InputDeviceCharacteristics flag)
        {
            return value & (~flag);
        }

        public static InputDeviceCharacteristics ToggleFlag(this InputDeviceCharacteristics value, InputDeviceCharacteristics flag)
        {
            return value ^ flag;
        }

        /// <summary>
        /// Checks if the <see cref="value"/> contains the <see cref="flag"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool HasFlags(this InputDeviceCharacteristics value, InputDeviceCharacteristics flag)
        {
            return (value & flag) == flag;
        }
    }
}