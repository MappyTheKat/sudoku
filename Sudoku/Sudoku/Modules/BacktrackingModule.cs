using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Modules
{
    public class BacktrackingModule : ISolver
    {
        public event EventHandler<PresentArgs> PrintCall;

        Board original;
        Board copied;
        int gridSize;

        public class PresentArgs : EventArgs
        {
            public Tuple<int, int> pos { get; set; }
            public int value { get; set; }

            public PresentArgs(int i, int j, int k)
            {
                pos = new Tuple<int, int>(i, j);
                value = k;
            }
        }

        public BacktrackingModule(Board b)
        {
            original = b;
            copied = new Board(b.ToString());
            //copied = b.Copy();
            gridSize = copied.gridSize;
        }

        public void clone_to_original()
        {
            original = copied.Copy();
        }

        public bool solve()
        {
            if (!original.isValid())
            {
                Console.WriteLine("unsolvable puzzle: original data Invalid!");
                return false;
            }
            int zero = copied.count_zero();
            Console.WriteLine(zero);
            return backtrack(zero);
        }

        public Board GetSolution()
        {
            //clone_to_original();
            return copied;
        }

        public bool backtrack(int n)
        {
            int i, j, k;
            Int64 count = 0;
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
<<<<<<< HEAD
<<<<<<< HEAD
                            if(count++ % 100 == 0)
                                //PrintCall(this, new PresentArgs(i, j, k));
=======
>>>>>>> parent of f2d0cb0... commit for sync
=======
>>>>>>> parent of f2d0cb0... commit for sync
                            if (copied.isValid() && backtrack(n - 1))
                            {
                                return true;
                            }
                        }
                        copied.boardData[i, j] = 0;
                        if (count % 100 == 0)
                            //PrintCall(this, new PresentArgs(i, j, 0));
                        return false;
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
