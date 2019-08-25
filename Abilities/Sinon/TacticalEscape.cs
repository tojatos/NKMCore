﻿using NKMCore.Effects;
using NKMCore.Templates;

namespace NKMCore.Abilities.Sinon
{
    public class TacticalEscape : Ability, IClickable
    {
        private const int SpeedIncrease = 8;
        private const int Duration = 1;

        public TacticalEscape(Game game) : base(game, AbilityType.Normal, "Tactical Escape", 4)
        {
//          Name = "Tactical Escape";
//          Cooldown = 4;
//          CurrentCooldown = 0;
//          Type = AbilityType.Normal;
        }
        public override string GetDescription() => 
$@"Zwiększa szybkość {ParentCharacter.Name} o {SpeedIncrease}.
Czas trwania: {Duration}    Czas odnowienia: {Cooldown}";

        public void Click()
        {
            ParentCharacter.TryToTakeTurn();
            ParentCharacter.Effects.Add(new StatModifier(Game, Duration, SpeedIncrease, ParentCharacter, StatType.Speed, Name));
            Finish();

        }
    }
}
