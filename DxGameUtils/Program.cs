using DxGameUtils.Core;

namespace DxGameUtils
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TextFileToMapImageConverter.Convert("Content/Map/SimpleMap.txt", "Content/Generated/");
        }
    }
}