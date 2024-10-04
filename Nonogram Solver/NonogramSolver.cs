using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram_Solver
{
    internal class NonogramSolver
    {
        private int [][] cols;
        private int [][] rows;
        private int[,] board;
          


        public NonogramSolver(int[][] cols, int[][] rows)
        {
            this.rows = rows;
            this.cols = cols;
            board = new int[rows.GetLength(0), cols.GetLength(0)];
            Fill2DArrayWithValue(board, 0);
            
        }

        public void Solve()
        {
            IsSolvable();

            do
            {
                for (int i = 0; i < rows.GetLength(0); i++)
                {
                    int[,] combinations = CalculateCombinations(rows[i], cols.GetLength(0));
                    FillRowOnBoard(combinations, i);
                }
                for (int i = 0; i < cols.GetLength(0); i++)
                {
                    int[,] combinations = CalculateCombinations(cols[i], rows.GetLength(0));
                    FillColumnOnBoard(combinations, i);
                }

            } while (IsSolved());
        }

        public void Paint()
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if(board[i, j] == 1)
                    {
                        Console.Write("██");
                    }
                    else if(board[i, j] == -1)
                    {
                        Console.Write("  ");
                    }
                }
                Console.WriteLine();
            }
        }

        public void Save()
        {
            using (var sw = new StreamWriter("Output.txt"))
            {
                for(int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        if (board[i, j] == 1)
                        {
                            sw.Write("#");
                        }
                        else if (board[i, j] == -1)
                        {
                            sw.Write("  ");
                        }
                    }
                    sw.Write("\n");
                }
                sw.Flush();
                sw.Close();
            }
        }

        private void IsSolvable()
        {
            int sumOfRows = 0;
            int sumOfCols = 0;

            foreach (int[] innerArray in rows)
            {
                foreach (int element in innerArray)
                {
                    sumOfRows += element;
                }
            }

            foreach (int[] innerArray in cols)
            {
                foreach (int element in innerArray)
                {
                    sumOfCols += element;
                }
            }

            if (sumOfRows != sumOfCols)
            {
                Console.WriteLine("Nonogram is not solvable!");
                Environment.Exit(0);
            }
            
        }

        private bool IsSolved()
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if(board[i, j] == 0)
                    {
                        return true;
                    }

                }
            }
            return false;

        }

        private int [,] CalculateCombinations(int[] array, int length)
        {
            long k = array.Count();
            long blanks = length - array.Sum() - (array.Count() - 1);
            long n = k + blanks;

            long numberOfCombinations = CalculateNumberOfCombinations(n, k);

            int[,] combinationsWithElements = new int[numberOfCombinations, k];
            int[,] combinations = new int[numberOfCombinations, length];

            List<int> elements = new List<int>();
            for(int i = 0; i < n; i++)
            {
                elements.Add(i);
            }

            var result = GetCombinationsWithElements(elements, k);
            int x = 0, y = 0;
            foreach (var perm in result)
            {
                foreach (var c in perm)
                {
                    combinationsWithElements[x,y] = c;
                    y++;
                }
                y = 0;
                x++;
            }
            x = 0;
            y = 0;

            combinations = TransformCombinations(combinationsWithElements, array, length);

            
            return combinations;
           
        }

        private long CalculateNumberOfCombinations(long n, long k)
        {
            long numerator = Factorial(n);
            long denominator = Factorial(k) * Factorial(n-k);
            return numerator / denominator;
        }

        private long Factorial(long number)
        {
            if (number <= 1)
                return 1;
            else
                return number * Factorial(number - 1);
        }

        private IEnumerable<IEnumerable<T>> GetCombinationsWithElements <T>(IEnumerable<T> items, long count)
        {
            int i = 0;
            foreach(var item in items)
            {
                if(count == 1)
                {
                    yield return new T[] { item };
                }
                else
                {
                    foreach (var result in GetCombinationsWithElements(items.Skip(i + 1), count - 1))
                        yield return new T[] { item }.Concat(result);
                }
                ++i;
            }
        }

        private int[,] TransformCombinations(int[,] combinationsWithElements, int[] rowOrCol, int length)
        {
            int[,] combinations = new int[combinationsWithElements.GetLength(0), length];
            Fill2DArrayWithValue(combinations, -1);

            for(int i = 0; i < combinationsWithElements.GetLength(0); i++)
            {
                int index = combinationsWithElements[i, 0];
                for (int j = 0; j < combinationsWithElements.GetLength(1); j++)
                {
                    index = Fill(combinations, rowOrCol[j], index, i);
                    if (j + 1 < combinationsWithElements.GetLength(1))
                    {
                        index = index + combinationsWithElements[i, j + 1] - combinationsWithElements[i, j];
                    }
                }
            }

            return combinations;
        }

        private int Fill(int[,] combinations, int element, int index, int i)
        {
            
            for(int j = 0; j< element; j++ )
            {
                combinations[i, index] = 1;
                index++;
            }

            return index;
        }

        private void Fill2DArrayWithValue(int[,] array, int value)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = value;
                }
            }
        }

        private void FillRowOf2DArrayWithValue(int[,] array, int value, int row)
        {
            for(int i = 0; i < array.GetLength(1); i++)
            {
                array[row,i] = value;
            }
        }

        private void FillArrayWithValue(int[] array, int value)
        {
            for(int i = 0; i < array.GetLength(0); i++)
            {
                array[i] = value;
            }
        }

        private void FillRowOnBoard(int[,] combinations, int row)
        {
            int flag = 0;
            int combRemoved = 0;
            int numberOfCombinations;
            int[] sumOfCombinations;

            
            for (int i = 0; i<board.GetLength(1); i++)
            {
                if (board[row,i] == 0)
                {
                    continue;
                }
                else
                {
                    flag = 1;
                }
            }

            if (flag == 1)
            {
                for (int i = 0; i < combinations.GetLength(0); i++)
                {
                    for (int j = 0; j < combinations.GetLength(1); j++)
                    {
                        if (board[row, j] == 0)
                        {
                            continue;
                        }
                        else if (combinations[i, j] != board[row, j])
                        {
                            FillRowOf2DArrayWithValue(combinations, 0, i);
                            combRemoved++;
                            break;
                        }
                    }
                }
            }

            sumOfCombinations = new int[combinations.GetLength(1)];

            sumOfCombinations = GetSumOfCombinations(combinations);
            numberOfCombinations = combinations.GetLength(0) - combRemoved; 


            for(int i = 0; i < sumOfCombinations.GetLength(0); i++)
            {
                if (sumOfCombinations[i] == numberOfCombinations)
                {
                    board[row, i] = 1;
                }
                else if(sumOfCombinations[i] == -(numberOfCombinations))
                {
                    board[row, i] = -1;
                }
            }

        }

        private void FillColumnOnBoard(int[,] combinations, int col)
        {
            int flag = 0;
            int combRemoved = 0;
            int numberOfCombinations;
            int[] sumOfCombinations;

            for (int i = 0; i < board.GetLength(0); i++)
            {
                if (board[i, col] == 0)
                {
                    continue;
                }
                else
                {
                    flag = 1;
                }
            }

            if (flag == 1)
            {
                for (int i = 0; i < combinations.GetLength(0); i++)
                {
                    for (int j = 0; j < combinations.GetLength(1); j++)
                    {
                        if (board[j, col] == 0)
                        {
                            continue;
                        }
                        else if (combinations[i, j] != board[j, col])
                        {
                            FillRowOf2DArrayWithValue(combinations, 0, i);
                            combRemoved++;
                            break;
                        }
                    }
                }

            }

            sumOfCombinations = new int[combinations.GetLength(1)];

            sumOfCombinations = GetSumOfCombinations(combinations);
            numberOfCombinations = combinations.GetLength(0) - combRemoved;

            for (int i = 0; i < sumOfCombinations.GetLength(0); i++)
            {
                if (sumOfCombinations[i] == numberOfCombinations)
                {
                    board[i, col] = 1;
                }
                else if (sumOfCombinations[i] == -(numberOfCombinations))
                {
                    board[i, col] = -1;
                }
            }

        }

        private int[] GetSumOfCombinations(int[,] array)
        {
            int[] sumOfCombinations = new int[array.GetLength(1)];
            FillArrayWithValue(sumOfCombinations,0);
            for(int i = 0; i < array.GetLength(1); i++)
            {
                for(int j = 0; j < array.GetLength(0); j++)
                {
                    sumOfCombinations[i] = sumOfCombinations[i] + array[j, i];
                }
            }

            return sumOfCombinations;
        }
    }
}
