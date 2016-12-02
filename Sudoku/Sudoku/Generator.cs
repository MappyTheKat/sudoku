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
        int status;
 
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
            if(sv != null && status == 0)
            {
                return sv.getPresentBoard();
            }
            return newBoard;
        }

        public void generate(int holeNumber)
        {
            int NUM_MAX = gridSize * 3 - 6; // number of random generated numbers
            status = 0;
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
            Board Digged = sv.solution.Copy();
            status = 1;
            sv = null;
            holeNumber = r.Next(54, 68); // 9x9 Normal difficulty (temporal), we should implement difficulty later
            //do {
            while (holeNumber > 0)
            {
                int digRow = r.Next(0, gridSize);
                int digCol = r.Next(0, gridSize);
                int diggedNumber = Digged.getBoard(digRow, digCol);

                if (Digged.getBoard(digRow, digCol) == 0)
                    continue;

                bool nope = false;
                holeNumber--;
                for(int i = 1; i <= gridSize; i++)
                {
                    if (i == diggedNumber)
                        continue;
                    Digged.setBoard(digRow, digCol, i);
                    sv = new Solver(Digged);
                    if (sv.solve(2))
                    {
                        Digged.setBoard(digRow, digCol, diggedNumber);
                        nope = true;
                        holeNumber++;
                        break;
                    }
                }
                if (!nope)
                {
                    Console.WriteLine("digging #{0}: {1}, {2}", holeNumber, digRow, digCol);
                    Digged.setBoard(digRow, digCol, 0);
                    newBoard = Digged.Copy();
                }
                    

            }
            //}
            //while (!Digged.isValid());

            Console.WriteLine("generating complete");
            Console.WriteLine(Digged.ToString());
            PresentBoard(this, new PresentBoardArgs(Digged.ToString()));
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
