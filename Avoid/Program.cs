// This file creates a new instance of the game.
// To see more comments, open Game1 and all the other files

using System;

namespace Avoid
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}
