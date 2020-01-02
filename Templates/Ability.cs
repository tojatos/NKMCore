using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Abilities.Bezimienni;
using NKMCore.Abilities.Kurogane_Ikki;
using NKMCore.Extensions;
using NKMCore.Hex;

namespace NKMCore.Templates
{
    public abstract class Ability : NKMEntity
    {
        protected Game Game { get; }

        public new abstract string Name { get; }
        protected abstract int Cooldown { get; }
        public int CurrentCooldown { get; set; }
        public bool CanUseOnGround { get; protected set; } = true;
        protected AbilityUseValidator Validator;
        public abstract AbilityType Type { get; }
        public abstract string GetDescription();

        public Active Active => Game.Active;
        protected NKMRandom Random => Game.Random;
        protected HexMap HexMap => Game.HexMap;
        protected GamePlayer Owner => Game.Players.FirstOrDefault(p => p.Characters.Contains(ParentCharacter));
        public bool CanBeUsed => Validator.AbilityCanBeUsed;
        protected Ability(Game game)
        {
            Game = game;
            CurrentCooldown = 0;
            OnAwake += () =>
            {
                Validator = new AbilityUseValidator(this);
                Active.Phase.PhaseFinished += () =>
                {
                    if (CurrentCooldown > 0) CurrentCooldown--;
                };
                AfterUseFinish += () => ParentCharacter.InvokeAfterAbilityUse(this);
            };
        }

        protected List<HexCell> GetNeighboursOfOwner(int depth, SearchFlags searchFlags = SearchFlags.None, Predicate<HexCell> stopAt = null) =>
            ParentCharacter.ParentCell.GetNeighbors(Owner, depth, searchFlags, stopAt);
        public virtual List<HexCell> GetRangeCells() => new List<HexCell>();
        public virtual List<HexCell> GetTargetsInRange() => new List<HexCell>();

        public Character ParentCharacter => Game.Characters.FirstOrDefault(c => c.Abilities.Contains(this) || c.Abilities.OfType<SwordSteal>().FirstOrDefault()?.CopiedAbility == this);

        protected void Finish() => Finish(Cooldown);

        protected void Finish(int cooldown)
        {
            if (ParentCharacter.Abilities.ContainsType(typeof(AceInTheHole)))
            {
                var ability = ParentCharacter.Abilities.Single(a => a.GetType() == typeof(AceInTheHole)) as AceInTheHole;
                if (ability != null && ability.HasFreeAbility) ability.HasFreeAbility = false;
                else CurrentCooldown = cooldown;

            }
            else CurrentCooldown = cooldown;
            switch (Type)
            {
                case AbilityType.Normal:
                    ParentCharacter.HasUsedNormalAbilityInPhaseBefore = true;
                    break;
                case AbilityType.Ultimatum:
                    ParentCharacter.HasUsedUltimatumAbilityInPhaseBefore = true;
                    break;
                case AbilityType.Passive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Active.CleanAndTrySelecting();
            AfterUseFinish?.Invoke();
        }
        protected void OnFailedUseFinish() => Active.CleanAndTrySelecting();

        protected event Delegates.Void OnAwake;
        public event Delegates.Void AfterUseFinish;
        public void Awake() => OnAwake?.Invoke();

        public virtual void Cancel() => OnFailedUseFinish();

        public enum AbilityType
        {
            Passive,
            Normal,
            Ultimatum
        }
    }
}
