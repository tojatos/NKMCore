using System.Collections.Generic;
using NKMCore.Abilities.Asuna;
using NKMCore.Abilities.Roronoa_Zoro;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore
{
    public class Console
    {
        public readonly List<ConsoleLine> LoggedLines = new List<ConsoleLine>();
        public List<ConsoleLine> NonDebugLines => LoggedLines.FindAll(t => t.IsDebug == false);

        private readonly CommandExecutor _commandExecutor;

        public delegate void StringBoolD(string text, bool isDebug);
        public event StringBoolD AfterAddLog;

        public Console(Game game)
        {
            _commandExecutor = new CommandExecutor(game);
            _commandExecutor.OnLog += Log;
        }

        public void ExecuteCommand(string command) => _commandExecutor.Execute(command);

        private void Log(string text) => AddLog(text);
        public void DebugLog(string text) => AddLog(text, true);
        private void AddLog(string text, bool isDebug = false)
        {
            LoggedLines.Add(new ConsoleLine
            {
                Text = text,
                IsDebug = isDebug,
            });
            AfterAddLog?.Invoke(text, isDebug);
        }
        public void AddTriggersToEvents(Character character)
        {
            character.AfterAttack += (targetCharacter, damage) => Log(
                $"{character.FormattedFirstName()} atakuje {targetCharacter.FormattedFirstName()}, zadając <color=red><b>{damage.Value}</b></color> obrażeń!");
            character.AfterHeal += (targetCharacter, value) =>
                Log(targetCharacter != character
                    ? $"{character.FormattedFirstName()} ulecza {targetCharacter.FormattedFirstName()} o <color=blue><b>{value}</b></color> punktów życia!"
                    : $"{character.FormattedFirstName()} ulecza się o <color=blue><b>{value}</b></color> punktów życia!");

            character.OnDeath += () => Log($"{character.FormattedFirstName()} umiera!");
        }

        public void AddTriggersToEvents(Ability ability)
        {
            switch (ability)
            {
                case SwordDance dance:
                    dance.OnBlock += attackingCharacter =>
                        Log($"{ability.ParentCharacter.FormattedFirstName()} blokuje atak {attackingCharacter.FormattedFirstName()}!");
                    break;
                case LackOfOrientation orientation:
                    orientation.AfterGettingLost += () =>
                        Log($"{ability.ParentCharacter.FormattedFirstName()}: Cholera, znowu się zgubili?");
                    break;
            }
        }
    }

    public class ConsoleLine
    {
        public string Text;
        public bool IsDebug;
        public override string ToString() => Text;
    }
}