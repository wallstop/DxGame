using DxCore.Core.Messaging;
using DxCore.Core.Utils.Validate;

namespace MapEditorLibrary.Core.Messaging
{
    public class ErrorNotification : Message
    {
        public string Message { get; }

        public ErrorNotification(string message)
        {
            Validate.Hard.IsNotNullOrDefault(message);
            Validate.Hard.IsNotEmpty(message);
            Message = message;
        }
    }
}
