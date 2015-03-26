﻿#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

#endregion

namespace DXGame.Main
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
        static void Main(string [] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            using (var game = DxGame.Instance)
                game.Run();
        }
    }
#endif
}
