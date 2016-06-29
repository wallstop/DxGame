using System;

namespace Pong
{
#if WINDOWS || LINUX

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using(Pong game = new Pong())
            {
                game.Run();
            }
        }
    }
#endif
}
