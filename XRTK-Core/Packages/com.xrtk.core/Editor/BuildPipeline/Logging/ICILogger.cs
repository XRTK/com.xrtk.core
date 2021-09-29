// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Editor.BuildPipeline.Logging
{
    public interface ICILogger : ILogHandler
    {
        string Error { get; }

        string Warning { get; }
    }
}
