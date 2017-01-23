using DxCore.Core.Messaging;
using WallNetCore.Validate;

namespace MapEditorLibrary.Core.Messaging
{
    public class SaveMapRequest : Message
    {
        public string FilePath { get; }

        public SaveMapRequest(string filePath)
        {
            Validate.Hard.IsNotNullOrDefault(filePath);
            FilePath = filePath;
        }
    }
}