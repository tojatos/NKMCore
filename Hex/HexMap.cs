﻿using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Hex
{
    public class HexMap
    {
        public readonly List<HexCell> Cells;
        public readonly string Name;
        public int MaxPlayers => Cells.Where(c => c.Type == HexCell.TileType.SpawnPoint).Select(c => c.SpawnNumber).ToList().Distinct().Count();
        public int MaxCharactersPerPlayer => Cells.Where(c => c.Type == HexCell.TileType.SpawnPoint).Select(c => c.SpawnNumber).GroupBy(n => n).Select(n => n.Count()).Min();

        public HexMap (string name, List<HexCell> cells)
        {
            Name = name;
            Cells = cells;
        }

        public HexMap Clone()
        {
            var newCells = new List<HexCell>();
            var newMap = new HexMap(Name, newCells);
            Cells.ForEach(c => newMap.Cells.Add(new HexCell(newMap, c.Coordinates, c.Type, c.SpawnNumber)));
            newMap.Cells.ForEach(c =>
            {
                newMap.Cells.FindAll(w =>
                        Math.Abs(w.Coordinates.X - c.Coordinates.X) <= 1 &&
                        Math.Abs(w.Coordinates.Y - c.Coordinates.Y) <= 1 &&
                        Math.Abs(w.Coordinates.Z - c.Coordinates.Z) <= 1 &&
                        w != c)
                    .ForEach(w => c.SetNeighbor(c.GetDirection(w), w));
            });
            return newMap;
        }

        private readonly Dictionary<Character, HexCell> _charactersOnCells = new Dictionary<Character, HexCell>();

        public void Place(Character character, HexCell cell)
        {
            _charactersOnCells[character] = cell;
            AfterCharacterPlace?.Invoke(character, cell);
        }
        public void Move(Character character, HexCell cell)
        {
            _charactersOnCells[character] = cell;
            AfterMove?.Invoke(character, cell);
        }

        public void Swap(Character firstCharacterToSwap, Character secondCharacterToSwap)
        {
            HexCell c1 = firstCharacterToSwap.ParentCell;
            HexCell c2 = secondCharacterToSwap.ParentCell;
            Move(firstCharacterToSwap, c2);
            Move(secondCharacterToSwap, c1);
        }

        public Delegates.CharacterCell AfterMove;
        public Delegates.CharacterCell AfterCharacterPlace;
        public Delegates.HexCellEffectD AfterCellEffectCreate;
        public Delegates.HexCellEffectD AfterCellEffectRemove;
        public void InvokeAfterCellEffectCreate(HexCellEffect e) => AfterCellEffectCreate?.Invoke(e);
        public void InvokeAfterCellEffectRemove(HexCellEffect e) => AfterCellEffectRemove?.Invoke(e);

        /// <summary>
        /// Removes character from map or does nothing
        /// </summary>
        public void RemoveFromMap(Character character) => _charactersOnCells.Remove(character);

        public HexCell GetCell(Character character) => _charactersOnCells.GetValueOrDefault(character);

        public List<Character> GetCharacters(HexCell cell) =>
            _charactersOnCells.Where(pair => pair.Value == cell).Select(pair => pair.Key).ToList();
    }
}