using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku.Modules;

namespace Sudoku
{
    public class Generator
    {
        Board initBoard;
        int gridSize;
 
        public Generator(int size)
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
            gridSize = size;
        }

        public Board generate(int holeNumber)
        {
            Board newBoard = initBoard.Copy();
            BacktrackingModule bm;
            const int NUM_MAX = 12;
            //lasvegas algorithms.
            List<int> selected = new List<int>();
            Random r = new Random();
            int numberSelect;
            int inputPosition;
            do
            {
                do
                {
                    numberSelect = NUM_MAX;
                    selected.Clear();
                    newBoard = initBoard.Copy();
                    while (numberSelect > 0)
                    {
                        inputPosition = r.Next(1, gridSize * gridSize);
                        if (!selected.Contains(inputPosition))
                        {
                            selected.Add(inputPosition);
                            newBoard.boardData[inputPosition / 9, inputPosition % 9] = r.Next(1, 9);
                            numberSelect--;
                        }
                    }

                }
                while (!newBoard.isValid());
                bm = new BacktrackingModule(newBoard);
            }
            while (!bm.solve());
            
            //digging holes


            return bm.GetSolution();
        }
    }
}
