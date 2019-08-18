using System;
using System.Collections.Generic;

namespace NKMCore
{
    public static class SelectionConstraint
    {
        public static readonly Func<List<int>, bool> Single = list => list.Count == 1;
    }
}