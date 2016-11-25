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
            var cb = copied.boardData;
            var ob = original.boardData;
            
            List<int>[,] map = drawMap(cb);
            if (hasSingle(map))
            {
                fillSingle();
                if (copied.isComplete())
                    return true;
                return Solve();
            }

            

            return false;
        }

        private bool hasSingle(List<int>[,] map)
        {
            throw new NotImplementedException();
        }

        private void fillSingle()
        {
            throw new NotImplementedException();
        }

        private List<int>[,] drawMap(int[,] cb)
        {
            throw new NotImplementedException();
        }
    }
}
