using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Modules
{
    public class SingleModule : ISolver
    {
        Board original;
        Board copied;
        int gridSize;

        public SingleModule(Board board)
        {
            original = board.Copy();
            copied = original.Copy();
            gridSize = copied.gridSize;
        }

        public bool Solve()
        {
            //naked single
            for(int i = 0; i < gridSize; i++)
            {
                for(int j = 0; j < gridSize; j++)
                {
                    //implement this...
                }
            }
            //hidden single

            return false;
        }

        
    }
}
