using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku.Modules;

namespace Sudoku
{
    public class Board : ICloneable
    {
        int[,] boardData; // 0 은 빈칸, boardData[row, column]
        public int gridSize = 0; // 작은 네모칸의 사이즈
        int width; // 한 그리드의 사이즈
        bool valid = true;
        int[][] rows;
        int[][] cols;
        int[][] grids;

        public void setBoard(int i, int j, int value)
        {
            boardData[i, j] = value;
            rows[i][j] = value;
            cols[j][i] = value;
            grids[(i / width) * width + j / width][(i % width) * width + j % width] = value;
            var r = getRow(i);
            var c = getCol(j);
            var g = getGrid(i, j);
            if (valid)
            {
                if (count_number(r, value) > 1 || count_number(c, value) > 1 || count_number(g, value) > 1)
                    valid = false;
            }
            else
                checkValid();
        }

        public int getBoard(int i, int j)
        {
            return boardData[i, j];
        }

        private void checkValid()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 1; j <= gridSize; j++)
                {
                    if (count_number(rows[i], j) > 1
                        || count_number(cols[i], j) > 1
                        || count_number(grids[i], j) > 1)
                    {
                        valid = false;
                        return;
                    }
                }
            }
            valid = true;
        }

        public Board(string str)
        {
            int[] BoardSize = {9, 16, 25, 36, 49};
            string[] parsed = str.Trim().Replace("\n", "").Split();
            
            for(int i = 0; i < BoardSize.Length; i++)
            {
                if(parsed.Length == BoardSize[i] * BoardSize[i]) {
                    gridSize = BoardSize[i];
                    break;
                }
            }
            if (gridSize == 0)
            {
                throw new System.ArgumentException("Parsing Error, Length failed.");
            }
            width = (int)Math.Sqrt(gridSize);
            boardData = new int[gridSize, gridSize];
            //boardData init
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    boardData[i, j] = Int32.Parse(parsed[i * gridSize + j]);
                }
            }

            //rows and cols init.
            rows = new int[gridSize][];
            cols = new int[gridSize][];
            grids = new int[gridSize][];
            for (int i = 0; i < gridSize; i++)
            {
                rows[i] = new int[gridSize];
                cols[i] = new int[gridSize];
                grids[i] = new int[gridSize];
                int offseti = (i / 3) * width;
                int offsetj = (i % 3) * width;
                for (int j = 0; j < gridSize; j++)
                {
                    rows[i][j] = boardData[i, j];
                    cols[i][j] = boardData[j, i];
                    grids[i][j] = boardData[offseti + j / width, offsetj + j % width];
                }
            }


            //check wether board is valid
            checkValid();
        }

        public override string ToString()
        // 내부 판정보 string 반환
        {
            string a = "";
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    a += boardData[i, j] + " ";
                }
                a += "\n";
            }
            return a;
        }

        public bool isValid()
        {
            return valid;
        }

        public bool isComplete()
        // check whether board is completed
        {
            return count_zero() == 0 && isValid();
        }

        public int count_zero()
        //copied 내부의 0 개수 반환
        {
            int c = 0;
            foreach (int i in boardData)
            {
                if (i == 0)
                    c++;
            }
            return c;
        }

        private int count_number(int[] a, int f)
            // get으로 불러들인 row안의 f 갯수 반환
        {
            int cnt = 0;
            foreach(int i in a)
            {
                if (i == f)
                    cnt++;
            }
            return cnt;
        }

        public int[] getCol(int n)
            // grid x grid 의 nth col list 반환
        {
            return cols[n];
        }

        public int[] getRow(int n)
            // nth row 반환
        {
            return rows[n];
        }

        public int[] getGrid(int i, int j)
            // (sqrt(gridsize) * sqrt(gridsize) 그리드에서 (n, m)th 그리드 반환
        {
            return grids[(i / width) * width + j / width];
        }

        public Board Copy()
        {
            return ((ICloneable)this).Clone() as Board;
        }

        object ICloneable.Clone()
        {
            return new Board(ToString());
        }
    }
}
