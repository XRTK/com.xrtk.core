// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace XRTK.Editor.Utilities.SymbolicLinks
{
    [Serializable]
    public class SymbolicLink
    {
        [SerializeField]
        private string sourceRelativePath;

        public string SourceRelativePath
        {
            get => sourceRelativePath;
            internal set => sourceRelativePath = value;
        }

        [SerializeField]
        private string targetRelativePath;

        public string TargetRelativePath
        {
            get => targetRelativePath;
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