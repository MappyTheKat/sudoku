using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Modules
{
    public class BacktrackingModule : ISolver
    {
        Board original;
        Board copied;
        int gridSize;

        public BacktrackingModule(Board b)
        {
            original = b;
            copied = new Board(b.ToString());
            gridSize = copied.gridSize;
        }

        private int count_zero()
        //copied 내부의 0 개수 반환
        {
            int c = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (copied.boardData[i, j] == 0)
                        c++;
                }
            }
            return c;
        }

        public void clone_to_original()
        {
            original = copied.Clone();
        }

        public bool solve()
        {
            if (!original.isValid())
            {
                Console.WriteLine("unsolvable puzzle: original data Invalid!");
                return false;
            }
            int zero = count_zero();
            return backtrack(zero);
        }

        public bool backtrack(int n)
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
                    else
                    {
                        continue;
                    }
                }
            }
            return false;
        }

        // Implementing ISolver interface
        public void Solve()
        {
            
        }
    }
}
