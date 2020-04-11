// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Devices
{
    /// <summary>
    ///  An operation that the device applies to an input value. For example, an "invert" Processor inverts a floating-point value.
    /// </summary>
    public class InputProcessor : BaseMixedRealityProfile { }

    /// <inheritdoc />
    /// <typeparam name="T"></typeparam>
    public abstract class InputProcessor<T> : InputProcessor where T : struct
    {
        /// <summary>
        /// An operation that the device applies to an raw input value to get a usable value. For example, an "invert" Processor inverts a floating-point value.
        /// </summary>
        /// <param name="value"></param>
        public abstract void Process(ref T value);
    }
}