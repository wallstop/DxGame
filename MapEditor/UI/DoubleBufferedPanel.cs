using System.Windows.Forms;

namespace MapEditor.UI
{
    /*
        Drawing images to a panel with a high refresh rate causes an extreme amount of vertical tear 
        while the image is redrawn. Double-buffering (DoubleBuffered = true) the enclosing form does
        no good. Additionally, the "SetStyle" method on panels is protected. In order to get a panel
        to be double buffered, we need to inherit from it.

        This class does exactly that.
    */

    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer,
                true);
        }
    }
}