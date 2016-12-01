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
        public event EventHandler<PresentBoardArgs> PresentBoard;
        public event EventHandler<EventArgs> GenerateEnded;

        Solver sv;
        Board initBoard;
        Board newBoard;
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
            newBoard = initBoard.Copy();
            gridSize = size;
        }

        public Board getPresentBoard()
        {
            if(sv != null)
            {
                return sv.getPresentBoard();
            }
            return newBoard;
        }

        public void generate(int holeNumber)
        {
            int NUM_MAX = gridSize * 3 - 6; // number of random generated numbers

            List<int> selected = new List<int>();
            Random r = new Random();

            //lasvegas algorithms.
            do // while bm has a solution
            {
                sv = null;
                do // while newBoard got valid puzzle
                {
                    int numberSelect = NUM_MAX;
                    selected.Clear();
                    newBoard = initBoard.Copy();
                    while (numberSelect > 0)
                    {
                        int inputPosition = r.Next(1, gridSize * gridSize);
                        if (!selected.Contains(inputPosition))
                        {
                            selected.Add(inputPosition);
                            newBoard.setBoard(inputPosition / gridSize, inputPosition % gridSize, r.Next(1, gridSize));
                            numberSelect--;
                        }
                    }

                }
                while (!newBoard.isValid());
                sv = new Solver(newBoard);
                sv.PresentBoard += PresentBoard;
            }
            while (!sv.solve(2));

            // TODO: implement digging holes here.


            Console.WriteLine("generating complete");
            PresentBoard(this, new PresentBoardArgs(sv.solution.ToString()));
            endGenerate();
        }

        public void endGenerate()
        {
            if(sv != null)
                sv.killSolver();
            GenerateEnded(this, new EventArgs());
        }
    }
}
