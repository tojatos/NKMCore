using System.Collections.Generic;

namespace NKMCore
{
    public interface ISelectable
    {
        void OpenSelectable(int selectableId);
        void CloseSelectable(int selectableId, List<int> selectedIds);
    }
}