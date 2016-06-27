using System;
using DxCore.Core.Messaging;
using DxCore.Core.Utils.Validate;

namespace MapEditorLibrary.Core.Messaging
{
    public class LoadMapRequest : Message
    {
        public string FilePath { get; }

        public LoadMapRequest(string filePath)
        {
            Validate.Hard.IsNotNullOrDefault(filePath);
            Validate.Hard.IsTrue(new Uri(filePath).IsFile);
            FilePath = filePath;
        }
    }
}
