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
        Board SolBoard;
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

        public static bool threeseconds()
        {
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine("i counted three seconds...");
            return true;
        }

        public void generate(int numOfGenerate, int hn)
        {
            Console.WriteLine("digging {0}", hn);
            string str = "";
            string retstr = "";
            //implement difficulty here.
            for (int i = 0; i < numOfGenerate; i++)
            {
                str = generateString(hn);
                PresentBoard(this, new PresentBoardArgs(str));
                retstr += str + "\n";
            }
            endGenerate();
            if(numOfGenerate > 1)
            {
                //save to file
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "generated";
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;
                    System.IO.File.WriteAllText(filename, retstr);
                }
            }
        }

        public string generateString(int holeNumber)
            // generate n boards...
        {
            int NUM_MAX = gridSize * 3 - 6; // number of random generated numbers
            status = 0;
            List<int> selected = new List<int>();
            Random r = new Random();
            bool solved = false;
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
                bool DONE = false;
                var t0 = new System.Threading.Thread(() => {
                    threeseconds();
                    if (DONE == false)
                    {
                        sv.killSolver();
                        DONE = true;
                    }
                });
                var t1 = new System.Threading.Thread(() =>
                {
                    solved = sv.solve(2, true);
                    Console.WriteLine("solved in three seconds...");
                    DONE = true;
                });
                t0.Start();
                t1.Start();
                while (!DONE)
                {
                    System.Threading.Thread.Sleep(200);
                }
                t0.Abort();
                t1.Abort();
            }
            while (!solved);

            Console.WriteLine("digging hole initiated");
            status = 1; // DO NOT PRESENT WHILE THIS REGION

            Board Digged = sv.solution.Copy();
            bool[,] DONOTDIG = new bool[gridSize, gridSize];
            //holeNumber = r.Next(54, 73); // 9x9 Normal difficulty (temporal), we should implement difficulty later
            sv = null;
            int[] shuffledArr = new int[gridSize * gridSize];

            for (int i = 0; i < gridSize * gridSize; i++)
            {
                DONOTDIG[i / gridSize, i % gridSize] = false;
                shuffledArr[i] = i;
            }

            // shuffling size array...
            int n = shuffledArr.Length;
            while (n > 1)
            {
                int k = r.Next(n--);
                int temp = shuffledArr[n];
                shuffledArr[n] = shuffledArr[k];
                shuffledArr[k] = temp;
            }

            //lets dig holes!
            int cnt = gridSize * gridSize - 1;
            while (holeNumber > 0)
            {
                if (cnt == 0)
                    break;
                int digRow = shuffledArr[cnt] / gridSize;
                int digCol = shuffledArr[cnt--] % gridSize;
                int diggedNumber = Digged.getBoard(digRow, digCol);

                if (DONOTDIG[digRow, digCol])
                    continue;

                bool nope = false;
                holeNumber--;
                for (int i = 1; i <= gridSize; i++)
                {
                    if (i == diggedNumber)
                        continue;
                    Digged.setBoard(digRow, digCol, i);
                    sv = new Solver(Digged);
                    if (sv.solve(2, true))
                    {
                        DONOTDIG[digRow, digCol] = true;
                        Digged.setBoard(digRow, digCol, diggedNumber);
                        nope = true;
                        holeNumber++;
                        break;
                    }
                }
                if (!nope)
                {
                    DONOTDIG[digRow, digCol] = true;
                    //Console.WriteLine("digging #{0}: {1}, {2}", holeNumber, digRow, digCol);
                    Digged.setBoard(digRow, digCol, 0);
                    newBoard = Digged.Copy();
                }
            }
            SolBoard = Digged.Copy();
            Console.WriteLine("generating complete");
            Console.WriteLine(Digged.ToString());
            PresentBoard(this, new PresentBoardArgs(Digged.ToString()));
            return Digged.ToString();
        }

        public void endGenerate()
        {
            if(sv != null)
                sv.killSolver();
            GenerateEnded(this, new EventArgs());
        }
    }
}
