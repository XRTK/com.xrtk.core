// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Extensions
{
    /// <summary>
    /// Casts an enum to an int using pointers.
    /// Source: https://stackoverflow.com/questions/1189144/c-sharp-non-boxing-conversion-of-generic-enum-to-int
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public static class EnumExtensions<TEnum> where TEnum : unmanaged, Enum
    {
        /// <summary>
        /// Converts a <see cref="TEnum"/> into a <see cref="TResult"/> through pointer cast.
        /// Does not throw if the sizes don't match, clips to smallest data-type instead.
        /// So if <see cref="TEnum"/> is smaller than <see cref="TResult"/>
        /// bits that cannot be captured within <see cref="TEnum"/>'s size will be clipped.
        /// </summary>
        public static TResult To<TResult>(TEnum value) where TResult : unmanaged
        {
            unsafe
            {
                if (sizeof(TResult) > sizeof(TEnum))
                {
                    // We might be spilling in the stack by taking more bytes than value provides,
                    // alloc the largest data-type and 'cast' that instead.
                    TResult o = default;
                    *((TEnum*)&o) = value;
                    return o;
                }

                return *(TResult*)&value;
            }
        }

        /// <summary>
        /// Converts a <see cref="TSource"/> into a <see cref="TEnum"/> through pointer cast.
        /// Does not throw if the sizes don't match, clips to smallest data-type instead.
        /// So if <see cref="TEnum"/> is smaller than <see cref="TSource"/>
        /// bits that cannot be captured within <see cref="TEnum"/>'s size will be clipped.
        /// </summary>
        public static TEnum From<TSource>(TSource value) where TSource : unmanaged
        {
            unsafe
            {
                if (sizeof(TEnum) > sizeof(TSource))
                {
                    // We might be spilling in the stack by taking more bytes than value provides,
                    // alloc the largest data-type and 'cast' that instead.
                    TEnum o = default;
                    *((TSource*)&o) = value;
                    return o;
                }

                return *(TEnum*)&value;
            }
        }
    }

    public static class EnumExtensions
    {
        /// <summary>
        /// Checks if the <see cref="value"/> contains the <see cref="flag"/>
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value to check.</param>
        /// <param name="flag">The flag to check.</param>
        /// <returns>True, if the <see cref="value"/> contains the <see cref="flag"/>.</returns>
        public static bool HasFlags<TEnum>(this TEnum value, TEnum flag) where TEnum : unmanaged, Enum
        {
            var intValue = EnumExtensions<TEnum>.To<int>(value);
            var intFlag = EnumExtensions<TEnum>.To<int>(flag);
            return (intValue & intFlag) == intFlag;
        }

        /// <summary>
        /// Sets a flag for an enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value to modify.</param>
        /// <param name="flag">The flag to set.</param>
        /// <returns>The modified <see cref="value"/>.</returns>
        public static TEnum SetFlags<TEnum>(this TEnum value, TEnum flag) where TEnum : unmanaged, Enum
        {
            var intValue = EnumExtensions<TEnum>.To<int>(value);
            var intFlag = EnumExtensions<TEnum>.To<int>(flag);
            return EnumExtensions<TEnum>.From(intValue | intFlag);
        }

        /// <summary>
        /// Unsets a flag for an enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value to modify.</param>
        /// <param name="flag">The flag to unset.</param>
        /// <returns>The modified <see cref="value"/>.</returns>
        public static TEnum UnsetFlags<TEnum>(this TEnum value, TEnum flag) where TEnum : unmanaged, Enum
        {
            var intValue = EnumExtensions<TEnum>.To<int>(value);
            var intFlag = EnumExtensions<TEnum>.To<int>(flag);
            return EnumExtensions<TEnum>.From(intValue & ~intFlag);
        }

        /// <summary>
        /// Toggles a flag for an enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value to modify.</param>
        /// <param name="flag"></param>
        /// <returns>The modified <see cref="value"/>.</returns>
        public static TEnum ToggleFlags<TEnum>(this TEnum value, TEnum flag) where TEnum : unmanaged, Enum
        {
            var intValue = EnumExtensions<TEnum>.To<int>(value);
            var intFlag = EnumExtensions<TEnum>.To<int>(flag);
            return EnumExtensions<TEnum>.From(intValue ^ intFlag);
        }
    }
}
