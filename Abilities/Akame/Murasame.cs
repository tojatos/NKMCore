using System.Linq;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Abilities.Akame
{
    public class Murasame : Ability
    {
        private const int PoisonDamage = 1;
        private const int NumberOfPoisonsToKill = 7;
        public Murasame(Game game) : base(game, AbilityType.Passive, "Murasame")
        {
            OnAwake += () =>
            {
                void AddPoisonEffect(Character character)
                {
                    character.Effects.Add(new Poison(Game, ParentCharacter, new Damage(PoisonDamage, DamageType.True), -1, character, Name));
                    int numberOfPoisons = character.Effects
                             .OfType<Poison>()
                             .Count(e => e.Name == Name);
                    if (numberOfPoisons >= NumberOfPoisonsToKill)
                    {
                        ParentCharacter.Attack(this, character, new Damage(999999, DamageType.True)); // TODO: character must die
                    }
                }

                ParentCharacter.AfterBasicAttack += (character, damage) =>
                    AddPoisonEffect(character);
                ParentCharacter.AfterAbilityAttack += (ability, character, damage) =>
                    AddPoisonEffect(character);
            };
        }

        public override string GetDescription() =>
$@"Każdy atak podstawowy lub umiejętność {ParentCharacter.FirstName()} nakłada efekt zatrucia.
Każdy ładunek zadaje {PoisonDamage} obrażeń postaci co fazę.
Po nałożeniu {NumberOfPoisonsToKill} ładunków zatrucia cel ginie. Zatrucie trwa do końca gry.";
    }
}