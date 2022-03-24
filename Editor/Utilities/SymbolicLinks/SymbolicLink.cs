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
            SourceRelativePath = sourceRelativePath;
            TargetRelativePath = targetRelativePath;
        }

        [SerializeField]
        private string sourceRelativePath;

        public string SourceRelativePath
        {
            get => sourceRelativePath;
            internal set => sourceRelativePath = value.ForwardSlashes().Replace(SymbolicLinker.ProjectRoot, string.Empty);
        }

        public string SourceAbsolutePath
        {
            get => $"{SymbolicLinker.ProjectRoot}{sourceRelativePath}".ForwardSlashes();
            internal set => sourceRelativePath = value;
        }

        [SerializeField]
        private string targetRelativePath;

        public string TargetRelativePath
        {
            get => targetRelativePath;
            internal set => targetRelativePath = value.ForwardSlashes().Replace(SymbolicLinker.ProjectRoot, string.Empty);
        }
        public string TargetAbsolutePath
        {
            get => $"{SymbolicLinker.ProjectRoot}{targetRelativePath}".ForwardSlashes();
            internal set => targetRelativePath = value;
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
