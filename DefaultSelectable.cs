using System.Collections.Generic;

namespace NKMCore
{
    public class DefaultSelectable : ISelectable
    {
        private SelectableAction _action;

        public void Init(SelectableAction action)
        {
            _action = action;
        }

        public void OpenSelectable(int selectableId) { }

        public void CloseSelectable(int selectableId, List<int> selectedIds) => _action.CloseSelectable(selectableId, selectedIds);
    }
}