using System.Collections.Generic;

namespace NKMCore
{
    public class Stat
    {
        private readonly StatType _type;
        public readonly List<Modifier> Modifiers = new List<Modifier>();
        public readonly int BaseValue;

        public event Delegates.IntInt StatChanged;

        public int RealValue { get; private set; }
        public int Value
        {
            get
            {
                int modifier = 0;
                Modifiers.ForEach(m => modifier += m.Value);
                return RealValue + modifier;
            }
            set
            {
                int oldValue = RealValue;
                RealValue = value;
                if (_type == StatType.HealthPoints && Value > BaseValue) Value = BaseValue;
                StatChanged?.Invoke(oldValue, RealValue);
            }
        }

        public Stat(StatType type, int baseValue)
        {
            _type = type;
            BaseValue = baseValue;
            Value = BaseValue;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Modifier
    {
        public int Value;
        public Modifier(int value)
        {
            Value = value;
        }
    }

    public enum StatType
    {
        HealthPoints,
        AttackPoints,
        BasicAttackRange,
        Speed,
        PhysicalDefense,
        MagicalDefense,
        Shield,
    }
}