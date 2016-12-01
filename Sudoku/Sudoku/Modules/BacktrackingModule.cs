using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Sudoku.Modules
{
    public class BacktrackingModule : ISolver
    {
        public Board original;
        public Board copied = null;
        int gridSize;

        public BacktrackingModule(Board b)
        {
            original = b;
            gridSize = b.gridSize;
        }

        public bool solve()
        {
            if (!original.isValid())
            {
                Console.WriteLine("unsolvable puzzle: original data Invalid!");
                return false;
            }
            copied = new Board(original.ToString());
            return backtrack(copied.count_zero());
        }

        public Board GetSolution()
        {
            return copied.Copy();
        }

        bool backtrack(int n)
        {
            int i, j, k;
            if (n == 0)
                return copied.isComplete();

            for (i = 0; i < gridSize; i++)
            {
                for (j = 0; j < gridSize; j++)
                {
                    if (copied.boardData[i, j] == 0)
                    {
                        for (k = 1; k <= gridSize; k++)
                        {
                            copied.boardData[i, j] = k;
                            if (copied.isValid() && backtrack(n - 1))
                            {
                                return true;
                            }
                        }
                        copied.boardData[i, j] = 0;
                        return false;
                    }
                }
            }
            return false;
        }

        // Implementing ISolver interface
        public bool Solve()
        {
            //not implemented.. 
            return false;
        }
    }
}
