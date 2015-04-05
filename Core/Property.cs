namespace DXGame.Core
{
    public sealed class Property<T>
    {
        public readonly T BaseValue;
        public Property(T value) { BaseValue = value; }
    }
}