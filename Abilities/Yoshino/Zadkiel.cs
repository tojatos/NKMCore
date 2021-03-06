﻿using System.Linq;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Abilities.Yoshino
{
    public class Zadkiel : Ability, IEnableable, IClickable
    {
        public override string Name { get; } = "Zadkiel";
        protected override int Cooldown { get; } = 3;
        public override AbilityType Type { get; } = AbilityType.Normal;

        private const int StunDuration = 1;
        private const int HitDurability = 3;
        private const int TimeDurability = 5;

        private int _currentHitRemains;
        private int _currentTimeDurability;

        public Zadkiel(Game game) : base(game)
        {
            OnAwake += () =>
            {
                ParentCharacter.BeforeBeingBasicAttacked += (character, damage) =>
                {
                    if(!IsEnabled) return;
                    if (!character.IsEnemyFor(Owner)) return;
                    damage.Value = 0;
                    character.Effects.Add(new Stun(Game, StunDuration, character, Name));
                    _currentHitRemains--;
                    if (_currentHitRemains <= 0) Disable();
                };
                Active.Phase.PhaseFinished += () =>
                {
                    if(!IsEnabled) return;
                    _currentTimeDurability--;
                    if (_currentTimeDurability <= 0) Disable();
                };
                Validator.ToCheck.Add(() => !IsEnabled);
            };
        }

        public override string GetDescription()
        {
            string desc =
$@"{ParentCharacter.Name} osłania się swoim aniołem tworząc zbroję pochłaniającą podstawowe ataki przeciwników.
Każdy wróg, który zaatakuje {ParentCharacter.Name} zostanie ogłuszony na {StunDuration} turę.
Zbroja niszczy się po otrzymaniu {HitDurability} ciosów bądź po {TimeDurability} fazach.
Wraz ze zniszczeniem zbroi, uaktywnia się umiejętność bierna {ParentCharacter.Name}.
Czas odnowienia: {Cooldown} (po zakończeniu efektu)";

            if (IsEnabled) desc +=
$@"
Uderzeń do zniszczenia: {_currentHitRemains}
Pozostały czas trwania: {_currentTimeDurability}";
            return desc;

        }

        public void Click()
        {
            ParentCharacter.TryToTakeTurn();

            IsEnabled = true;
            _currentHitRemains = HitDurability;
            _currentTimeDurability = TimeDurability;
            Finish(0);
        }

        private void Disable()
        {
            IsEnabled = false;

            var runablePassive = ParentCharacter.Abilities.SingleOrDefault(a => a.Type == AbilityType.Passive && a is IRunnable) as IRunnable;
            runablePassive?.Run();

            CurrentCooldown = Cooldown;
        }

        public bool IsEnabled { get; private set; }
    }
}
