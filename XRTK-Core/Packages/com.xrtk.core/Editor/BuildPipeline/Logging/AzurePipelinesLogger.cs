// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Editor.BuildPipeline.Logging
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/scripts/logging-commands
    /// </summary>
    public class AzurePipelinesLogger : AbstractCILogger
    {
        public override string Error => "##vso[task.logissue type=error;]";

        public override string Warning => "##vso[task.logissue type=warning;]";
    }
}
