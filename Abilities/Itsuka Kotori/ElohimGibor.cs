using NKMCore.Templates;

namespace NKMCore.Abilities.Itsuka_Kotori
{
    public class ElohimGibor : Ability
    {
        public override string Name { get; } = "Elohim Gibor";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        private const int Percent = 50;
        private int _turnsWithoutBeingHurt;
        private int _amountToHeal;
        private bool _wasDamagedThisTurn;

        public ElohimGibor(Game game) : base(game)
        {
            OnAwake += () =>
            {
                Active.Phase.PhaseFinished += () =>
                {
                    if (_wasDamagedThisTurn)
                    {
                        _wasDamagedThisTurn = false;
                        return;
                    }
                    ++_turnsWithoutBeingHurt;
                    if (_turnsWithoutBeingHurt < 2) return;
                    ParentCharacter.Heal(ParentCharacter, _amountToHeal);
                    _turnsWithoutBeingHurt = 0;
                    _amountToHeal = 0;
                };
                ParentCharacter.AfterBeingDamaged += damage =>
                {
                    _amountToHeal += (int) (damage.Value * Percent / 100f);
                    _turnsWithoutBeingHurt = 0;
                };

            };
        }
        public override string GetDescription() =>
$@"Jeżeli {ParentCharacter.Name} nie otrzyma obrażeń przez 2 tury z rzędu,
regeneruje ona ilość HP równą {Percent}% obrażeń otrzymywanych przez nią od ostatniego uaktywnienia tej umiejętności";

    }
}
