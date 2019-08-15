using System;
using System.Collections.Generic;
using System.Linq;

namespace NKMCore
{
    public class SelectableAction
    {
        private readonly GameType _type;
        private readonly ISelectable _selectable;
        public event Delegates.String MultiplayerAction;
        
        public SelectableAction(GameType type, ISelectable selectable)
        {
            _type = type;
            _selectable = selectable;
        }
        
        public void Make(string actionType, string[] args)
        {
            switch (actionType)
            {
                case Action.Types.OpenSelectable:
                {
                    OpenSelectable(int.Parse(args[0]), true);
                } break;
                case Action.Types.CloseSelectable:
                {
                    int[] ids = args.Select(int.Parse).ToArray();
                    CloseSelectable(ids[0], ids.Skip(1).ToList(), true);
                } break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
            }
        }
        
        public void OpenSelectable(int selectableId, bool force = false)
        {
            if (_type == GameType.Multiplayer && !force)
            {
                MultiplayerAction?.Invoke($"ACTION {Action.Types.OpenSelectable};{selectableId}");
                return;
            }

            _selectable.OpenSelectable(selectableId);
        }
        
        public void CloseSelectable(int selectableId, List<int> selectedIds, bool force = false)
        {
            if (_type == GameType.Multiplayer && !force)
            {
                MultiplayerAction?.Invoke($"ACTION {Action.Types.OpenSelectable};{selectableId}:{string.Join(":", selectedIds)}");
                return;
            }

            _selectable.CloseSelectable(selectableId, selectedIds);
        }
    }
}