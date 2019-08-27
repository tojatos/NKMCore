using System;
using System.Collections.Generic;
using System.Linq;

namespace NKMCore
{
    public class SelectableAction
    {
        private readonly GameType _type;
        private readonly ISelectable _selectable;
        public event Delegates.StringString AfterAction;
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

        public void OpenSelectable(int selectableId, bool force = false) =>
                Act(Action.Types.OpenSelectable,
                    $"{selectableId}",
                    () => _selectable.OpenSelectable(selectableId),
                    force);

        public void CloseSelectable(int selectableId, List<int> selectedIds, bool force = false) =>
                Act(Action.Types.CloseSelectable,
                    $"{selectableId}:{string.Join(":", selectedIds)}",
                    () => _selectable.CloseSelectable(selectableId, selectedIds),
                    force);

        public static string Serialize(string actionType, string serializedContent)
            => string.Join(";", $"ACTION {actionType}", serializedContent);
        private void Act(string actionType, string serializedContent, System.Action action, bool force)
        {
            string serializedMessage = Serialize(actionType, serializedContent);
            if (_type == GameType.Multiplayer && !force)
                MultiplayerAction?.Invoke(serializedMessage);
            else
            {
                action();
                AfterAction?.Invoke(actionType, serializedContent);
            }
        }
    }
}