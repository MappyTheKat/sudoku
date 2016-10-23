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
        public int gridSize; // 작은 네모칸의 사이즈

        public Board(string str)
        {
            string[] parsed = str.Trim().Replace(" \n", "").Split();
            if (parsed.Length != 81)
            {
                throw new System.ArgumentException("Parsing Error, Length failed.");
            }

            boardData = new int[9, 9];
            gridSize = 9;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    boardData[i, j] = Int32.Parse(parsed[i * 9 + j]);
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

        public void solve()
        {
            Console.WriteLine("solving started");
            BacktrackingModule mb = new BacktrackingModule(this);
            mb.solve();
            Console.WriteLine("Solving completed");
        }

        public bool isValid()
        {
            //check wether board is valid

            //row validate
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 1; j <= gridSize; j++)
                {
                    int cnt = 0;
                    for (int k = 0; k < gridSize; k++)
                    {
                        if (boardData[i, k] == j)
                            cnt++;
                    }
                    //Console.WriteLine("row:{0}, checking {1}", i, j);
                    if (cnt != 1)
                        return false;
                }
            }

            //column validate
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 1; j <= gridSize; j++)
                {
                    int cnt = 0;
                    for (int k = 0; k < gridSize; k++)
                    {
                        if (boardData[k, i] == j)
                            cnt++;
                    }
                    if (cnt != 1)
                        return false;
                }
            }

            //grid validate
            int width = (int)Math.Sqrt(gridSize);
            int row, col;
            for (int i = 0; i < gridSize; i++)
            {
                row = (i / width) * width;
                col = (i % width) * width;
                for (int j = 1; j <= gridSize; j++)
                {
                    int cnt = 0;
                    for (int k = 0; k < gridSize; k++)
                    {
                        if (boardData[row + (k / width), col + (k % width)] == j)
                            cnt++;
                    }
                    //Console.WriteLine("row:{0}, col:{1} checking {2} cnt:{3}", row, col, j, cnt);
                    if (cnt != 1)
                        return false;
                }
            }

            return true;
        }

        public bool isComplete()
        // check whether board is completed
        {
            if (!isValid())
                return false;

            int sum = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    sum += boardData[i, j];
                }
            }
            return sum == (gridSize + 1) * gridSize / 2 * gridSize;
        }

        // 기본 생성자. Clone을 구현하기 위해서 만든 것이니 웬만하면 호출하지 맙시다.
        public Board()
        {

        }

        public Board Clone()
        {
            return this.Clone();
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
            // throw new NotImplementedException();
        }
    }
}
