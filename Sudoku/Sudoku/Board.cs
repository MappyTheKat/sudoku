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
        public int[,] boardData; // 0 은 빈칸
        public int gridSize = 0; // 작은 네모칸의 사이즈

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

            boardData = new int[gridSize, gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    boardData[i, j] = Int32.Parse(parsed[i * gridSize + j]);
                }
            }
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
            //check wether board is valid
            
            int width = (int) Math.Sqrt(gridSize);
            for(int i = 0; i < gridSize; i++)
            {
                var r = getRow(i);
                var c = getCol(i);
                var g = getGrid(i % width, i / width);
                for(int j = 1; j <= gridSize; j++)
                {
                    if (count_number(r, j) > 1
                        || count_number(c, j) > 1
                        || count_number(g, j) > 1)
                        return false;
                }
            }
            return true;
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

        private int count_number(List<int> a, int f)
        {
            int cnt = 0;
            foreach(int i in a)
            {
                if (i == f)
                    cnt++;
            }
            return cnt;
        }

        public List<int> getCol(int n)
            // grid x grid 의 nth col list 반환
        {
            if (gridSize < n || n < 0)
                return null;
            List<int> ret = new List<int>();
            for(int i = 0; i < gridSize; i++)
            {
                ret.Add(boardData[i, n]);
            }
            return ret;
        }

        public List<int> getRow(int n)
            // nth row 반환
        {
            if (gridSize < n || n < 0)
                return null;
            List<int> ret = new List<int>();
            for(int i = 0; i < gridSize; i++)
            {
                ret.Add(boardData[n, i]);
            }
            return ret;
        }

        public List<int> getGrid(int n, int m)
            // (sqrt(gridsize) * sqrt(gridsize) 그리드에서 (n, m)th 그리드 반환
        {
            int gridWidth = (int) Math.Sqrt(gridSize);
            if (gridWidth < n || n < 0 || gridWidth < m || m < 0)
                return null;
            List<int> ret = new List<int>();
            for(int i = 0; i < gridSize; i++)
            {
                ret.Add(boardData[i / gridWidth + n * gridWidth, i % gridWidth + m * gridWidth]);
            }
            return ret;
        }

        // 기본 생성자. Clone을 구현하기 위해서 만든 것이니 웬만하면 호출하지 맙시다.
        public Board()
        {

        }

        public Board Copy()
        {
            return ((ICloneable)this).Clone() as Board;
        }

        object ICloneable.Clone()
        {
            var board = new Board();
            board.gridSize = this.gridSize;

            board.boardData = new int[this.gridSize, this.gridSize];

            // 내용을 그대로 복사한다.
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    board.boardData[i, j] = this.boardData[i, j];
                }
            }
            return board;
        }
    }
}
