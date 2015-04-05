namespace DXGame.Core
{
    public enum MutatePriority
    {
        High,
        Medium,
        Low
    }

    public abstract class PropertyMutator<T>
    {
        public virtual string Name { get; protected set; }
        public abstract T Mutate(T input);
    }
}