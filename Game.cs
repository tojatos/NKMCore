using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore
{
    public class Game
    {
        public GameDependencies Dependencies { get; private set; }

        public List<GamePlayer> Players;
        public List<Character> Characters => Players.SelectMany(p => p.Characters).ToList();
        private List<Ability> Abilities => Characters.SelectMany(c => c.Abilities).ToList();
        public readonly Active Active;
        public HexMap HexMap;
        public readonly Action Action;
        public readonly Console Console;
        public readonly NKMRandom Random;
        public static IDbConnection Conn;
        public ISelectable Selectable { get; private set; }
        public SelectableAction SelectableAction;

        public bool IsReplay => Dependencies.GameLog != null;
        public Game(GameDependencies gameDependencies)
        {
            Active = new Active(this);
            Action = new Action(this);
            Console = new Console(this);
            Random = new NKMRandom();
            Init(gameDependencies);
        }

        private void Init(GameDependencies gameDependencies)
        {
            Dependencies = gameDependencies;
            Selectable = gameDependencies.Selectable;
            SelectableAction = gameDependencies.SelectableAction;
            Conn = gameDependencies.Connection;

            Players = new List<GamePlayer>(gameDependencies.Players);
            HexMap = gameDependencies.HexMap;

            Random.OnValueGet += (name, value) => Console.GameLog($"RNG: {name}; {value}");
            Action.AfterAction += (type, _) =>
            {
                if (type == Action.Types.BasicAttack || type == Action.Types.BasicMove)
                    Active.Select(Active.Character);
            };
            Console.AddTriggersToEvents(Active.Turn);
        }
        /// <summary>
        /// Get a copy of every character in the game
        /// </summary>
        public static List<Character> GetMockCharacters()
        {
            List<string> names = Conn.GetCharacterNames();
            var toReturn = new List<Character>();
            int len = names.Count;
            for (int i = 1; i <= len; ++i)
            {
                toReturn.Add(CharacterFactory.Create(null, names[i-1], -i));
            }

            return toReturn;
        }

        public event Delegates.AbilityD AfterAbilityCreation;
        public event Delegates.CharacterD AfterCharacterCreation;
        public event Delegates.AbilityD AfterAbilityInit;
        public event Delegates.CharacterD AfterCharacterInit;
        public event Delegates.Void OnFinish;
        public void InvokeAfterCharacterCreation(Character c) => AfterCharacterCreation?.Invoke(c);
        public void InvokeAfterAbilityCreation(Ability a) => AfterAbilityCreation?.Invoke(a);

        public void Start()
        {
            //This needs to be checked before TakeTurns(), because we attach to TurnStarted event
            if (!Dependencies.PlaceAllCharactersRandomlyAtStart)
            {
                //TODO: maybe remove these triggers after first phase? Because this if will be checked every time turn is started
                Active.Turn.TurnStarted += async player =>
                {
                    if (!IsEveryCharacterPlacedInTheFirstPhase) await TryToPlaceCharacter();
                };
                Active.AfterCancelPlacingCharacter += async () =>
                {
                    if (!IsEveryCharacterPlacedInTheFirstPhase) await TryToPlaceCharacter();
                };
            }

            Abilities.ForEach(Init);
            AfterAbilityCreation += Init;

            Characters.ForEach(Init);
            AfterCharacterCreation += Init;

            TakeTurns();
            if (Dependencies.PlaceAllCharactersRandomlyAtStart)
            {
                PlaceAllCharactersRandomlyOnSpawns();
                if (Active.Phase.Number == 0) Active.Phase.Finish();
            }
        }

        private void Init(Character c)
        {
            AddTriggersToEvents(c);
            AfterCharacterInit?.Invoke(c);
        }
        private void Init(Ability a)
        {
            a.Awake();
            Console.AddTriggersToEvents(a);
            AfterAbilityInit?.Invoke(a);
        }

        private async Task TryToPlaceCharacter()
        {
            List<Character> charactersToPlace = Active.GamePlayer.Characters.Where(c => !c.IsOnMap && c.IsAlive).ToList();
            if (!charactersToPlace.Any() || Active.SelectedCharacterToPlace != null) return;
            Character pickedCharacter = null;
            int id = Dependencies.SelectableManager.Register(new SelectableProperties
            {
                WhatIsSelected = SelectableProperties.Type.Character,
                IdsToSelect = charactersToPlace.Select(c => c.ID).ToList(),
                ConstraintOfSelection = SelectionConstraint.Single,
                OnSelectFinish = list =>
                {
                    Active.PrepareToPlaceCharacter(Active.GamePlayer.GetSpawnPoints(this)
                        .FindAll(c => c.IsFreeToStand));
                    Active.SelectedCharacterToPlace = Active.GamePlayer.Characters.Single(c => c.ID == list[0]);
                    pickedCharacter = Active.SelectedCharacterToPlace;
                },
                SelectionTitle = "Wystaw postać",
                Instant = true,
            });
            SelectableAction.OpenSelectable(id);
            Func<bool> placed = () => pickedCharacter?.IsOnMap == true;
            await placed.WaitToBeTrue();
        }
        private void PlaceAllCharactersRandomlyOnSpawns() => Players.ForEach(p => p.Characters.ForEach(c => TryPlacingOnRandomSpawnCell(p, c)));
        /// <summary>
        /// Infinite loop that manages Turns and Phases
        /// </summary>
        private async void TakeTurns()
        {
            while (true)
            {
                foreach (GamePlayer player in Players)
                {
                    if(player.IsEliminated) continue;
                    await TakeTurn(player);
                }

                if (Players.Count(p => !p.IsEliminated) <= 1)
                {
                    FinishGame();
                    return;
                }

                if (!IsEveryCharacterPlacedInTheFirstPhase) continue;
                if(EveryCharacterTookActionInPhase) Active.Phase.Finish();
            }
        }
        private void FinishGame() => OnFinish?.Invoke();
        private bool EveryCharacterTookActionInPhase => Players.All(p => p.Characters.Where(c => c.IsAlive).All(c => c.TookActionInPhaseBefore));
        private bool IsEveryCharacterPlacedInTheFirstPhase => !(Active.Phase.Number == 0 && Players.Any(p => p.Characters.Any(c => !c.IsOnMap && c.IsAlive)));
        /// <summary>
        /// Start a turn and wait for player to end it
        /// </summary>
        private async Task TakeTurn(GamePlayer player)
        {
            Active.Turn.Start(player);
            Func<bool> isTurnDone = () => Active.Turn.IsDone;
            await isTurnDone.WaitToBeTrue();
        }

        private void TryPlacingOnRandomSpawnCell(GamePlayer p, Character c)
        {
            HexCell spawnPoint = p.GetSpawnPoints(this).FindAll(cell => Active.CanPlace(c, cell)).GetNKMRandom(Random);
            if (spawnPoint == null) return;

            HexMap.Place(c, spawnPoint);
        }

        private void AddTriggersToEvents(Character character)
        {
            Console.AddTriggersToEvents(character);

            character.JustBeforeFirstAction += () => Active.Turn.CharacterThatTookActionInTurn = character;
            character.OnDeath += () =>
            {
                HexMap.RemoveFromMap(character);
                character.DeathTimer = 0;
                if (Active.Character == character) Active.Deselect();
            };
            character.HealthPoints.StatChanged += () =>
            {
                if (character.IsAlive) return;
                character.InvokeOnDeath();
            };
            character.OnDeath += () => character.Effects.Clear();

            Active.Turn.TurnFinished += other =>
            {
                if (other != character) return;
                character.HasFreeAttackUntilEndOfTheTurn = false;
                character.HasFreeMoveUntilEndOfTheTurn = false;
                character.HasFreeUltimatumAbilityUseUntilEndOfTheTurn = false;
            };

            Active.Phase.PhaseFinished += () =>
            {
                if (character.IsOnMap)
                {
                    character.Refresh();
                }

                if (!character.IsAlive)
                {
                    character.DeathTimer++;
                }
            };
        }
    }
}
