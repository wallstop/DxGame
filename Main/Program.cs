#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
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
            DxGameArguments gameArguments = parseArgs(args);

            using (var game = new DXGame(gameArguments))
                game.Run();
        }

        static DxGameArguments parseArgs(string[] args)
        {
            DxGameArguments parsedArguments = new DxGameArguments();
            parsedArguments.IsServer = true;

            bool parsedSuccessfully = true;
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].Contains("server") && i + 1 < args.Length)
                {
                    parsedSuccessfully = true;
                    parsedArguments.IsServer = false;
                    parsedArguments.ServerIp = args[i + 1];
                    parsedArguments.ServerPort = 8000;
                    break;
                }
                else
                {
                    parsedSuccessfully = false;
                }
            }

            if (!parsedSuccessfully)
            {
                Console.WriteLine(String.Format("Could not properly parse {0}", args));
                Console.WriteLine("Expects --server [ip address], without brackets");
                Environment.Exit(1);
            }
            return parsedArguments;

        }
    }
#endif
}
