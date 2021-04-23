// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.DiagnosticsSystem
{
    public struct MemoryPeak
    {
        public MemoryPeak(ulong memoryPeak)
        {
            Value = memoryPeak;
        }

        public readonly ulong Value;

        public static implicit operator ulong(MemoryPeak self) => self.Value;
    }
}