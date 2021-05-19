// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Extensions;

namespace XRTK.Editor.Utilities.SymbolicLinks
{
    [Serializable]
    public class SymbolicLink
    {
        public SymbolicLink() { }

        public SymbolicLink(string sourceRelativePath, string targetRelativePath)
        {
            this.sourceRelativePath = sourceRelativePath;
            this.targetRelativePath = targetRelativePath;
        }

        [SerializeField]
        private string sourceRelativePath;

        public string SourceRelativePath
        {
            get => sourceRelativePath.ForwardSlashes();
            internal set => sourceRelativePath = value.ForwardSlashes();
        }

        public string SourceAbsolutePath
        {
            get => $"{SymbolicLinker.ProjectRoot}{sourceRelativePath}".ForwardSlashes();
            internal set => sourceRelativePath = value.ForwardSlashes().Replace(SymbolicLinker.ProjectRoot, string.Empty);
        }

        [SerializeField]
        private string targetRelativePath;

        public string TargetRelativePath
        {
            get => targetRelativePath.ForwardSlashes();
            internal set => targetRelativePath = value.ForwardSlashes();
        }
        public string TargetAbsolutePath
        {
            get => $"{SymbolicLinker.ProjectRoot}{targetRelativePath}".ForwardSlashes();
            internal set => targetRelativePath = value.ForwardSlashes().Replace(SymbolicLinker.ProjectRoot, string.Empty);
        }

        [SerializeField]
        private bool isActive;

        public bool IsActive
        {
            get => isActive;
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    SymbolicLinker.RunSync(true);
                }
            }
        }
    }
}
