namespace NKMCore.Templates
{
    public abstract class NKMEntity
    {
        public string Name;
        public int ID;
        public override string ToString() => $"{Name} ({ID})";
    }
}