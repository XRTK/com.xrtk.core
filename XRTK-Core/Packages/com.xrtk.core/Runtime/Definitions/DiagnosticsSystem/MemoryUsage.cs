// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.DiagnosticsSystem
{
    public struct MemoryUsage
    {
        public MemoryUsage(ulong memoryUsage)
        {
            Value = memoryUsage;
        }

        public readonly ulong Value;

        public static implicit operator ulong(MemoryUsage self) => self.Value;
    }
}