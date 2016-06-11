#region Using Statements

using System;
using Babel.Main;
using DxCore;

#endregion

namespace DXGame.Main
{
#if WINDOWS || LINUX
    /// <summary>
    ///     The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            using (var game = new BabelGame())
            {
                game.Run();
            }
        }
    }
#endif
}