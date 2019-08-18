using System;
using System.Collections.Generic;

namespace NKMCore
{
    public class SelectableProperties
    {
        public Type WhatIsSelected { get; set; }
        public List<int> IdsToSelect { get; set; }
        public Func<List<int>, bool> ConstraintOfSelection { get; set; }
        public Action<List<int>> OnSelectFinish { get; set; }
        public string SelectionTitle { get; set; }
        public bool Instant { get; set; }

        public enum Type
        {
            Character,
        }
    }
}