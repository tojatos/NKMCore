﻿using System;
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
			Action.AfterAction += str =>
			{
				if (str == Action.Types.BasicAttack || str == Action.Types.BasicMove) Active.Select(Active.Character);
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
			for (var i = 1; i <= len; ++i)
			{
				toReturn.Add(CharacterFactory.Create(null, names[i-1], -i));
			}

			return toReturn;
		}

		public event Delegates.AbilityD AfterAbilityCreation;
		public event Delegates.CharacterD AfterCharacterCreation;
		public event Delegates.AbilityD AfterAbilityInit;
		public event Delegates.CharacterD AfterCharacterInit;
		public void InvokeAfterCharacterCreation(Character c) => AfterCharacterCreation?.Invoke(c);
		public void InvokeAfterAbilityCreation(Ability a) => AfterAbilityCreation?.Invoke(a);
		
		public void Start()
		{
			if (!Dependencies.PlaceAllCharactersRandomlyAtStart || Dependencies.Type == GameType.Multiplayer) //TODO: checking for multiplayer is for tests only
			{
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
			if (Dependencies.PlaceAllCharactersRandomlyAtStart && Dependencies.Type != GameType.Multiplayer)
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
				ConstraintOfSelection = list => list.Count == 1,
				OnSelectFinish = list =>
				{
					Active.PrepareToPlaceCharacter(Active.GamePlayer.GetSpawnPoints(this)
						.FindAll(c => c.IsFreeToStand));
					Active.SelectedCharacterToPlace = Active.GamePlayer.Characters.Single(c => c.ID == list[0]);
					pickedCharacter = Active.SelectedCharacterToPlace;
				},
				SelectionTitle = "Wystaw postać",
			});
			SelectableAction.OpenSelectable(id);
			Func<bool> placed = () => pickedCharacter?.IsOnMap == true;
			await placed.WaitToBeTrue();
		}
		private void PlaceAllCharactersRandomlyOnSpawns() => Players.ForEach(p => p.Characters.ForEach(c => TrySpawningOnRandomCell(p, c)));
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
		private static void FinishGame()
		{
			//TODO
		}
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

		private void TrySpawningOnRandomCell(GamePlayer p, Character c)
		{
			HexCell spawnPoint = p.GetSpawnPoints(this).FindAll(cell => Active.CanSpawn(c, cell)).GetRandom();
			if (spawnPoint == null) return;

			//Spawner.Instance.Spawn(HexMapDrawer.Instance.SelectDrawnCell(spawnPoint), c);
			Action.PlaceCharacter(c, spawnPoint);
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
					character.HasUsedBasicAttackInPhaseBefore = false;
					character.HasUsedBasicMoveInPhaseBefore = false;
					character.HasUsedNormalAbilityInPhaseBefore = false;
					character.HasUsedUltimatumAbilityInPhaseBefore = false;
					character.TookActionInPhaseBefore = false;
				}

				if (!character.IsAlive)
				{
					character.DeathTimer++;
				}
			};
		}

		public event Delegates.Cell AfterCellSelect;
		public void TouchCell(HexCell touchedCell)
        {
            Active.SelectedCell = touchedCell;
	        AfterCellSelect?.Invoke(touchedCell);
            if (Active.SelectedCharacterToPlace != null)
            {
	            if(!Active.GamePlayer.GetSpawnPoints(this).Contains(touchedCell)) return;
                Action.PlaceCharacter(Active.SelectedCharacterToPlace, touchedCell);
	            if (Active.Phase.Number != 0) return;
	            Action.FinishTurn();
            }
            else if (Active.HexCells?.Contains(touchedCell) == true)
            {
                if (Active.AbilityToUse != null)
                {
	                //It is important to check in that order, in case ability uses multiple interfaces!
	                if(Active.AbilityToUse is IUseableCharacter && !touchedCell.IsEmpty)
		                Action.UseAbility((IUseableCharacter)Active.AbilityToUse, touchedCell.FirstCharacter);
	                else if(Active.AbilityToUse is IUseableCell)
		                Action.UseAbility((IUseableCell)Active.AbilityToUse, touchedCell);
	                else if(Active.AbilityToUse is IUseableCellList)
                        Action.UseAbility((IUseableCellList)Active.AbilityToUse, Active.AirSelection.IsEnabled ? Active.AirSelection.HexCells : Active.HexCells);
                }
                else if (Active.Character != null)
                {
                    if(!touchedCell.IsEmpty && Active.Character.CanBasicAttack(touchedCell.FirstCharacter))
                        Action.BasicAttack(Active.Character, touchedCell.FirstCharacter);
                    else if(touchedCell.IsFreeToStand && Active.Character.CanBasicMove(touchedCell) && Active.MoveCells.Last() == touchedCell)
                        Action.BasicMove(Active.Character, Active.MoveCells);
                }
            }
            else
            {
                if (Active.AbilityToUse != null) return;

                if (!touchedCell.IsEmpty)
                    Action.Select(touchedCell.FirstCharacter);
	            else 
	                Action.Deselect();
            }
        }
	}
}
