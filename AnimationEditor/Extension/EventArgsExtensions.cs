using System.Windows;

namespace AnimationEditor.Extension
{
    public static class EventArgsExtensions
    {
        public static T ExtractNewValue<T>(this RoutedPropertyChangedEventArgs<object> eventArgs, T defaultValue)
        {
            return (T) (eventArgs.NewValue ?? defaultValue);
        }

        public static T ExtractOldValue<T>(this RoutedPropertyChangedEventArgs<object> eventArgs, T defaultValue)
        {
            return (T) (eventArgs.OldValue ?? defaultValue);
        }
    }
}