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
            
            List<int>[,] map = drawMap();
            //printDrawMap(map);

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

        private void printDrawMap(List<int>[,] map)
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (map[i, j].Count != 0)
                    {
                        Console.Write("i: {0} j: {1} ==>", i, j);
                        foreach (var ele in map[i, j])
                            Console.Write(" {0}", ele);
                        Console.WriteLine();
                    }
                }
            }
        }

        private bool fillSingle(List<int>[,] map)
        {
            // Naked Single
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
            if(x != -1)
            {
                copied.setBoard(x, y, map[x, y][0]);
                return true;
            }

            // Hidden single
            
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    foreach(int target in map[i, j])
                    {
                        int[] found = { 0, 0, 0 };
                        int width = (int)Math.Sqrt(gridSize);
                        int offseti = (i / width) * width;
                        int offsetj = (j / width) * width;
                        for(int k = 0; k < gridSize; k++)
                        {
                            //row find
                            if (map[i, k].IndexOf(target) != -1)
                            {
                                found[0]++;
                            }
                            //col find
                            if(map[k, j].IndexOf(target) != -1)
                            {
                                found[1]++;
                            }
                            //grid find
                            if(map[offseti + k / width, offsetj + k % width].IndexOf(target) != -1)
                            {
                                found[2]++;
                            }
                        }
                        if (found[0] == 1 || found[1] == 1 || found[2] == 1)
                        {
                            //Console.WriteLine("found at {0}, {1} -> {2}", i, j, target);
                            copied.setBoard(i, j, target);
                            return true;
                        }
                    }
                }
            }

            return false;           
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
                r[i] = copied.getRow(i).ToList<int>();
                c[i] = copied.getCol(i).ToList<int>();
                g[i / gridWidth, i % gridWidth] = copied.getGrid((i / gridWidth) * gridWidth, (i % gridWidth) * gridWidth).ToList<int>();
            }

            List<int> total = new List<int>();
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (copied.getBoard(i, j) != 0)
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
