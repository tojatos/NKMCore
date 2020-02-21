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
        public HexMap HexMap;
        public readonly Action Action;
        public readonly Console Console;
        public readonly NKMRandom Random;
        public SelectableAction SelectableAction;
        private Logger Logger;

        public IGame(GameDependencies gameDependencies)
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
            SelectableAction = gameDependencies.SelectableAction;
            Logger = gameDependencies.Logger;

            Players = new List<GamePlayer>(gameDependencies.Players);
            HexMap = gameDependencies.HexMap;

            Action.AfterAction += (type, _) =>
            {
                if (type == Action.Types.BasicAttack || type == Action.Types.BasicMove)
                    Active.Select(Active.Character);
            };
            Action.AfterAction += (actionType, serializedContent) =>
                Logger.Log(Action.Serialize(actionType, serializedContent));
        }

        public abstract void Start();

        public event Delegates.AbilityD AfterAbilityCreation;
        public event Delegates.CharacterD AfterCharacterCreation;
        public void InvokeAfterCharacterCreation(Character c) => AfterCharacterCreation?.Invoke(c);
        public void InvokeAfterAbilityCreation(Ability a) => AfterAbilityCreation?.Invoke(a);

    }
}