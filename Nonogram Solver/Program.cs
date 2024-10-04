using System;
using System.Collections.Generic;
using static System.Linq.Enumerable;

namespace Nonogram_Solver
{
     class Program
    {
        public static void Main()
        {
            string userInput;
            string rawColumns = "";
            string rawRows = "";
            Console.WriteLine("NONOGRAM SOLVER");
            Console.WriteLine("----------------");
            Console.WriteLine();
            Console.Write("Ako zelite ucitati podatke iz tekstualne datoteke napisite 'd', a ako zelite unjeti podatke napisite 'w': ");
            Console.WriteLine();

            do
            {
                userInput = Console.ReadLine();
                if(userInput == "" || userInput != "d" && userInput != "w")
                {
                    Console.WriteLine("Unesite ispravno slovo!");
                }
            }
            while (userInput != "d" && userInput != "w");

            if(userInput == "d")
            {
                Console.Write("Unesite ime tekstualne datoteke: ");
                string textFileName = Console.ReadLine();
                rawRows = File.ReadLines(textFileName+".txt").First();
                rawColumns = File.ReadLines(textFileName+".txt").Last();

            }
            else if(userInput == "w")
            {
                Console.WriteLine("Upisi redove. (redove odvajati sa ';', a brojeve sa ',')");
                rawRows = Console.ReadLine();
                Console.WriteLine("Upisi stupce. (stupce odvajati sa ';', a brojeve sa ',')");
                rawColumns = Console.ReadLine();

            }


            int[][] columns = rawColumns.Split(';').Select(x => x.Split(',').Select(y => int.Parse(y)).ToArray()).ToArray();
            int[][] rows = rawRows.Split(';').Select(x => x.Split(',').Select(y => int.Parse(y)).ToArray()).ToArray();

            NonogramSolver solver = new NonogramSolver(columns, rows);

            solver.Solve();
            solver.Paint();
            solver.Save();



        }

       
    }
}