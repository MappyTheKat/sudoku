using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Modules
{
    public class SingleModule : ISolver
    {
        public Board original;
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
            //Console.WriteLine("called solve");
            var cb = copied.boardData;
            var ob = original.boardData;
            
            List<int>[,] map = drawMap();
            /*
            for(int i = 0; i < gridSize; i++)
            {
                for(int j = 0; j < gridSize; j++)
                {
                    if (map[i, j].Count != 0) {
                        Console.Write("i: {0} j: {1} ==>", i, j);
                        foreach (var ele in map[i, j])
                            Console.Write(" {0}", ele);
                        Console.WriteLine();
                    }
                }
            }*/

            if (fillSingle(map))
            {
                original = copied.Copy();
                if (original.isComplete())
                {
                    return true;
                }
                return Solve();
            }

            return false;
        }

        private bool fillSingle(List<int>[,] map)
        {
            int x = -1, y = -1;
            for(int i = 0; i < gridSize; i++)
            {
                for(int j = 0; j < gridSize; j++)
                {
                    if (map[i, j].Count == 1)
                    {
                        x = i;
                        y = j;
                        i = gridSize;
                        break;
                    }
                }
            }
            if(x == -1)
                return false;
            copied.boardData[x, y] = map[x, y][0];
            return true;
        }

        private List<int>[,] drawMap()
        {
            int gridWidth = (int)Math.Sqrt(gridSize);
            List<int>[, ] map = new List<int>[gridSize, gridSize];
            List<int>[] r = new List<int>[gridSize];
            List<int>[] c = new List<int>[gridSize];
            List<int>[, ] g = new List<int>[gridWidth, gridWidth];

            for(int i = 0; i < gridSize; i++)
            {
                r[i] = copied.getRow(i);
                c[i] = copied.getCol(i);
                g[i / 3, i % 3] = copied.getGrid(i / 3, i % 3);
            }

            List<int> total = new List<int>();
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (copied.boardData[i, j] != 0)
                    {
                        map[i, j] = new List<int>();
                        continue;
                    }
                    total.Clear();
                    total.AddRange(r[i]);
                    total.AddRange(c[j]);
                    total.AddRange(g[i / gridWidth, j / gridWidth]);
                    map[i, j] = findAvailable(total);
                }
            }

            return map;
        }

        private List<int> findAvailable(List<int> target)
        {
            List<int> ret = new List<int>();
            for(int i = 1; i <= gridSize; i++)
            {
                if (target.IndexOf(i) == -1)
                    ret.Add(i);
            }
            return ret;

        }
    }
}
