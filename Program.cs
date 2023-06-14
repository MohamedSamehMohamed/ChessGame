using System;

namespace ChessGame
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ChessGame game = new ChessGame();
            var c = Console.ReadLine();
            if (c == "S")
            {
                game.Simulate();
            }
            else
            {
                Console.Clear();
                game.AgainstComputer();
            }
        }
    }
}