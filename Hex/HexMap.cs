using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Hex
{
    public class HexMap
    {
        public readonly List<HexCell> Cells;
        public readonly List<HexCell.TileType> SpawnPoints;
        public readonly string Name;

        public HexMap (string name, List<HexCell> cells, List<HexCell.TileType> spawnPoints)
        {
            Name = name;
            Cells = cells;
            SpawnPoints = spawnPoints;
        }

        public HexMap Clone() => new HexMap(Name, new List<HexCell>(Cells), new List<HexCell.TileType>(SpawnPoints));

        public int MaxCharactersPerPlayer => Cells.Count(c => c.Type == SpawnPoints[0]);
        public int MaxPlayers => SpawnPoints.Count;

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