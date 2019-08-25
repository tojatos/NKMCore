using System.Linq;

namespace NKMCore
{
    public class CommandExecutor
    {
        private readonly Game _game;
        private Active Active => _game.Active;

        public event Delegates.String OnLog;
        public CommandExecutor(Game game)
        {
            _game = game;
        }

        public void Execute(string text)
        {
            string[] arguments = text.Split(' ');
            if(arguments.Length == 0) return; //TODO: check for arguments below to avoid IndexOutOfRange, maybe use a library?

            if (new[] { "set", "s" }.Contains(arguments[0]))
            {
                if (new[] { "phase", "p" }.Contains(arguments[1])) Active.Phase.Number = int.Parse(arguments[2]);
                if (new[] { "abilities", "ab" }.Contains(arguments[1]))
                {
                    if (new[] { "free", "f" }.Contains(arguments[2])) _game.Characters.FindAll(c => c.IsOnMap)
                        .ForEach(c => c.Abilities.ForEach(a => a.CurrentCooldown = 0));
                }
                if (Active.Character == null) return;
                if (new[] { "hp", "h" }.Contains(arguments[1])) Active.Character.HealthPoints.Value = int.Parse(arguments[2]);
                if (new[] { "atk", "at", "a" }.Contains(arguments[1])) Active.Character.AttackPoints.Value = int.Parse(arguments[2]);
                if (new[] { "speed", "sp", "s" }.Contains(arguments[1])) Active.Character.Speed.Value = int.Parse(arguments[2]);
                if (new[] { "range", "rang", "r" }.Contains(arguments[1])) Active.Character.BasicAttackRange.Value = int.Parse(arguments[2]);
                if (new[] { "shield", "sh" }.Contains(arguments[1])) Active.Character.Shield.Value = int.Parse(arguments[2]);

            }
            else if (new[] {"get", "g"}.Contains(arguments[0]))
            {
                if (new[] {"character", "c"}.Contains(arguments[1]))
                {
                    if(new[] {"names", "n"}.Contains(arguments[2]))
                        _game.Characters.Select(c => c.ToString()).ToList().ForEach(t => OnLog?.Invoke(t));
                    if(new[] {"actionstate", "a"}.Contains(arguments[2]))
                        _game.Characters.Select(c => $"{c} {c.TookActionInPhaseBefore}").ToList().ForEach(t => OnLog?.Invoke(t));
                }

            }
            else OnLog?.Invoke("Nieznana komenda: " + text);
        }
    }
}