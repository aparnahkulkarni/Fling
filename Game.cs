using System;
using System.Collections.Generic;
using ConsoleApplication2;

namespace ConsoleApplication2
{
   
    class Game
    {

        public Board Board;

        public Game()
        {
            Board=new Board();
        }

        public void Initialize()
        {
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 7; j++)
                {
                    var chr = Console.ReadKey();
                    if (chr.KeyChar != 78 && chr.KeyChar >= 65 && chr.KeyChar <= 90 || chr.KeyChar >= 97 && chr.KeyChar <= 122)
                    {
                        if (!Board.Columns.ContainsKey(j))
                        {
                            Board.Columns[j] = new List<Ball>();
                        }

                        var ball = new Ball(chr.KeyChar.ToString(), i, j);

                        Board.AddInRowColumn(ball, i, true);
                        Board.AddInRowColumn(ball, j, false);
                    }
                }
                Console.WriteLine("");
            }
        }

        public int Fling()
        {
            if (Board.Rows.Keys.Count == 0)
            {
                Console.WriteLine("Board not initialized!");
                return 0;
            }
            var solutions = new List<IList<string>>();

            Board.Fling(Board, solutions);
            
            if (solutions.Count==0) Console.WriteLine("No solution found.");
            else
            {
                for (int index = 0; index < solutions.Count; index++)
                {
                    var solution = solutions[index];
                    Console.WriteLine("\nSolution # {0}", index+1);
                    foreach (var level in solution)
                    {
                        Console.WriteLine(level);
                    }
                }
            }
            
            return 0;
        }

        static void Main()
        {
            Game game = new Game();
            game.Initialize();
            game.Fling();
            Console.ReadKey();
        
        }
    }
}
