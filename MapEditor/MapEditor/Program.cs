using System;
using DxCore;

namespace MapEditor
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using(DxGame game = new MapEditor())
            {
                game.Run();
            }
        }
    }
#endif
}
