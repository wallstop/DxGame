using EmptyKeys.UserInterface.Input;

namespace AnimationEditorLibrary.EmptyKeys.Relay
{
    public static class Nop
    {
        public static RelayCommand Instance { get; } = new RelayCommand(_ => { });
    }
}