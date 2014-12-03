namespace DXGame.Core.Components.Basic
{
    public abstract class InitializableComponent : Component
    {
        public InitializableComponent(GameObject parent = null)
            : base(parent)
        {
        }

        public abstract bool Initialize();
    }
}