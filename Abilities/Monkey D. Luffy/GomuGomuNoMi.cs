﻿using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Monkey_D._Luffy
{
    public class GomuGomuNoMi : Ability, IClickable, IUseableCell, IEnchantable
    {
        public override string Name { get; } = "Gomu Gomu no Mi";
        protected override int Cooldown { get; } = 2;
        public override AbilityType Type { get; } = AbilityType.Normal;

        private const int Range = 7;
        private const int BazookaRange = 3;
        private const int BazookaDamage = 17;
        private const int BazookaKnockback = 8;
        private const int BazookaCooldown = 3;
        private const int PistolDamage = 15;
        private const int JetBazookaKnockback = 14;
        private const int JetBazookaDamage = 23;
        private const int JetPistolDamage = 19;

        public event Delegates.Void OnClick;
        public event Delegates.Void BeforePistol;
        public event Delegates.Void BeforeRocket;

        public GomuGomuNoMi(Game game) : base(game)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range, SearchFlags.StraightLine);

        public override List<HexCell> GetTargetsInRange() => GetRangeCells().FindAll(c =>
            c.CharactersOnCell.Any(ch => ch.IsEnemyFor(Owner))|| !ParentCharacter.IsGrounded && c.Type == HexCell.TileType.Wall);

        public override string GetDescription() =>
$@"{ParentCharacter.Name} używa umiejętności Gumowego Owocu w zależności od celu:

<i>Wróg w zasięgu {BazookaRange}</i>
<b>Bazooka</b>
{ParentCharacter.Name} wyciąga obie ręce do tyłu, a następnie ciska je do przodu,
zadając przeciwnikowi {BazookaDamage} obrażeń fizycznych i odrzucając go {BazookaKnockback} pól dalej.

<i>Wróg w dalszym zasięgu</i>
<b>Pistol</b>
{ParentCharacter.Name} wyciąga rękę do tyłu, a następnie ciska ją do przodu,
zadając przeciwnikowi {PistolDamage} obrażeń fizycznych.

<i>Ściana</i>
<b>Rocket</b>
{ParentCharacter.Name} łapie się ściany, wybijając się za nią o tyle pól, ile ma do ściany.

Umiejętność <b>{Name}</b> może zostać ulepszona:

<b>Bazooka</b>
Obrażenia: {JetBazookaDamage}
Odrzut: {JetBazookaKnockback}

<b>Pistol</b>
Obrażenia: {JetPistolDamage}

Zasięg: {Range}    Czas odnowienia: {Cooldown} ({BazookaCooldown}, jeżeli użyje Bazooki)";

        public void Click()
        {
            Active.Prepare(this, GetTargetsInRange());
            OnClick?.Invoke();
        }

        public void Use(HexCell cell)
        {
            ParentCharacter.TryToTakeTurn();
            if (cell.Type == HexCell.TileType.Wall) Rocket(cell);
            else
            {
                Character enemy = cell.FirstCharacter;
                if (GetNeighboursOfOwner(BazookaRange).Contains(cell)) Bazooka(enemy);
                else Pistol(enemy);
            }
        }

        private void Pistol(Character enemy)
        {
            BeforePistol?.Invoke();
            ParentCharacter.Attack(this, enemy,new Damage(IsEnchanted ? JetPistolDamage : PistolDamage, DamageType.Physical));
            Finish();
        }

        private void Bazooka(Character enemy)
        {
            ParentCharacter.Attack(this, enemy, new Damage(IsEnchanted?JetBazookaDamage:BazookaDamage, DamageType.Physical));
            if(!enemy.IsAlive) return;
            HexDirection direction = ParentCharacter.ParentCell.GetDirection(enemy.ParentCell);
            ThrowCharacter(enemy, direction, IsEnchanted?JetBazookaKnockback:BazookaKnockback);
            Finish(BazookaCooldown);
        }

        private void Rocket(HexCell cell)
        {
            HexDirection direction = ParentCharacter.ParentCell.GetDirection(cell);
            int distance = ParentCharacter.ParentCell.GetDistance(cell);
            if(distance<=0) return;

            BeforeRocket?.Invoke();
            ThrowCharacter(ParentCharacter, direction, distance*2);
            Finish();
        }

        private static void ThrowCharacter(Character character, HexDirection direction, int distance)
        {
            List<HexCell> line = character.ParentCell.GetLine(direction, distance);
            line.Reverse();
            HexCell targetCell = line.FirstOrDefault(c => c.IsFreeToStand);
            if(targetCell==null) return;
            character.MoveTo(targetCell);
        }


        public bool IsEnchanted { get; set; }
    }
}
