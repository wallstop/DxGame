using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxGameUtils.Core
{
    public class FontFamilyEnumerator
    {
        /* Taken from https://msdn.microsoft.com/en-us/library/0yf5t4e8%28v=vs.110%29.aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-1, excuse shit code pls */
        public static void EnumerateFonts()
        {
            FontFamily fontFamily = new FontFamily("Arial");
            Font font = new Font(
               fontFamily,
               8,
               FontStyle.Regular,
               GraphicsUnit.Point);
            RectangleF rectF = new RectangleF(10, 10, 500, 500);
            SolidBrush solidBrush = new SolidBrush(Color.Black);

            string familyName;
            string familyList = "";
            FontFamily[] fontFamilies;

            InstalledFontCollection installedFontCollection = new InstalledFontCollection();

            // Get the array of FontFamily objects.
            fontFamilies = installedFontCollection.Families;

            // The loop below creates a large string that is a comma-separated 
            // list of all font family names. 

            int count = fontFamilies.Length;
            for (int j = 0; j < count; ++j)
            {
                familyName = fontFamilies[j].Name;
                familyList = familyList + familyName;
                familyList = familyList + ",  ";
            }

            Console.WriteLine(familyList);
            Console.ReadLine();
        }
    }
}
