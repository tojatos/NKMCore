using System.Collections.Generic;
using System.Linq;
using NKMCore.Hex;

namespace NKMCore.Templates
{
    public abstract class IGame
    {
        public GameDependencies Dependencies { get; private set; }

        public List<GamePlayer> Players;
        public List<Character> Characters => Players.SelectMany(p => p.Characters).ToList();
        public List<Ability> Abilities => Characters.SelectMany(c => c.Abilities).ToList();
        public readonly Active Active;
        public readonly HexMap HexMap;
        public readonly Action Action;
        public readonly Console Console;
        public readonly NKMRandom Random;
        private readonly ILogger _logger;
        public SelectableAction SelectableAction;

        protected IGame(GameDependencies gameDependencies)
        {
            Active = new Active(this);
            Action = new Action(this);
            Console = new Console(this);
            Random = new NKMRandom();
            _logger = gameDependencies.Logger;
            HexMap = gameDependencies.HexMap;
            Init(gameDependencies);
        }

        private void Init(GameDependencies gameDependencies)
        {
            Dependencies = gameDependencies;
            SelectableAction = gameDependencies.SelectableAction;

            Players = new List<GamePlayer>(gameDependencies.Players);

            Action.AfterAction += (type, _) =>
            {
                if (type == Action.Types.BasicAttack || type == Action.Types.BasicMove)
                    Active.Select(Active.Character);
            };
            Action.AfterAction += (actionType, serializedContent) =>
                _logger.Log(Action.Serialize(actionType, serializedContent));
        }

        public event Delegates.AbilityD AfterAbilityCreation;
        public event Delegates.CharacterD AfterCharacterCreation;
        public void InvokeAfterCharacterCreation(Character c) => AfterCharacterCreation?.Invoke(c);
        public void InvokeAfterAbilityCreation(Ability a) => AfterAbilityCreation?.Invoke(a);

    }
}