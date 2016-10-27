using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class Generator
    {
        Board initBoard;
        Generator(int size)
        {
            string init = "";
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    init += "0 ";
                }
            }
            initBoard = new Sudoku.Board(init);

        }

        public Board generate()
        {
            Board newBoard = initBoard.Copy();

            //lasvegas algorithms.
            int[] visited;
            int numberSelect = 12;

            return newBoard;
        }
    }
}
