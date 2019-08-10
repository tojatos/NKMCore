using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using NKMCore.Extensions;
using NKMCore.Templates;
using TMPro.EditorUtilities;

namespace NKMCore
{
    /// <summary>
    /// This class is responsible for assigning characters to players
    /// </summary>
    public abstract class CharacterPick
    {
	    protected readonly Game _game;
	    protected readonly GamePreparerDependencies _preparerDependencies;

	    protected CharacterPick(Game game, GamePreparerDependencies preparerDependencies)
        {
            _game = game;
            _preparerDependencies = preparerDependencies;
        }

	    public static CharacterPick Create(Game game, GamePreparerDependencies preparerDependencies)
	    {
		    switch (preparerDependencies.PickType)
		    {
			    case PickType.Blind:
				    return new BlindPick(game, preparerDependencies);
			    case PickType.Draft:
				    return new DraftPick(game, preparerDependencies);
			    case PickType.AllRandom:
				    return new AllRandom(game, preparerDependencies);
			    default:
				    throw new ArgumentOutOfRangeException();
		    }
	    }

        public virtual async Task BindCharactersToPlayers()
        {
	        throw new NotImplementedException();
        }

        protected async Task PickCharacters(List<Character> charactersToPick, int numberOfCharactersToPick, GamePlayer player, Game game)
		{
			await SelectAndWait(new SelectableProperties<Character>
			{
				ToSelect = charactersToPick,
				ConstraintOfSelection = list => list.Count == numberOfCharactersToPick,
				OnSelectFinish = list => player.Characters.AddRange(list.Select(c => CharacterFactory.Create(game, c.Name))),
				SelectionTitle = $"Wybór postaci - {player.Name}",
			});
		}
		
		private void AllRandom(Game game)
		{
		}

		protected async Task SelectAndWait(SelectableProperties<Character> props)
		{
			bool isSelected= false;
			props.OnSelectFinish += list => isSelected = true;
			_preparerDependencies.Selectable.Select(props);
			Func<bool> picked = () => isSelected;
			await picked.WaitToBeTrue();
		}
        
    }
}