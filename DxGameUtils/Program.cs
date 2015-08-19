using DxGameUtils.Core;

namespace DxGameUtils
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //TextFileToMapImageConverter.Convert("Content/Map/SimpleMap2.txt", "Content/Generated/");
            //FontFamilyEnumerator.EnumerateFonts();
            GridDrawer.DrawGrid("Content/Map/First.jpg");
        }
    }
}