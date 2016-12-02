using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Sudoku.Modules;

namespace Sudoku
{
    class Solver
    {
        
        public event EventHandler<SolveEndedArgs> SolveEnded;
        public event EventHandler<PresentBoardArgs> PresentBoard;
        public BacktrackingModule bm;
        public Board solution;
        public List<Thread> threads;
        Board originalBoard;
        int gridSize;
        int solvingMethod = 0;

        public Solver(Board b)
        {
            originalBoard = b.Copy();
            solution = b.Copy();
            gridSize = originalBoard.gridSize;
        }

        public bool solve(int solm)
            //solving method 0 : backtraking,  1 : heuristic
            //returns true when solution found...
        {
            solvingMethod = solm;
            threads = new List<Thread>();
            string message = "";

            if(!originalBoard.isValid())
            {// 보드가 정상이 아니네용...
                SolveEnded?.Invoke(this, new SolveEndedArgs(solution.isComplete(), "invalid puzzle..."));
                return false;
            }

            if (solvingMethod == 0)
            {
                threads.Add(new Thread(SolveBacktrack));
                threads[0].Start();
                threads[0].Join();
            }
            if (solvingMethod >= 1)
            {
                //do some hueristic...
                bool done = false;
                /*threads.Add(new Thread(
                    () =>
                    {
                        fiveseconds();
                        message = "timed out after " + 5 + " seconds";
                        done = true;
                    }));
                threads.Add(new Thread(
                    () =>
                    {
                        SolveBacktrack();
                        message = "backtracking completed";
                        done = true;
                    }));*/
                threads.Add(new Thread(
                    () =>
                    {
                        SingleModule sm = new SingleModule(originalBoard);
                        sm.Solve();
                        solution = sm.original;
                        originalBoard = sm.original.Copy();
                        PresentBoard?.Invoke(this, new PresentBoardArgs(sm.original.ToString()));
                        done = true;
                        message = "single completed";
                    }));
                foreach (var t in threads)
                    t.Start();
                while (!done)
                {
                    //implement job scheduler...
                    //Console.WriteLine("waiting for completing");
                    Thread.Sleep(30);
                }
                foreach (var t in threads)
                    t.Abort();
            }
            if (solvingMethod == 2)
            {// heuristic + backtrack
                threads.Clear();
                threads.Add(new Thread(SolveBacktrack));
                threads[0].Start();
                threads[0].Join();
            }
            Console.WriteLine("solver returned {0}", solution.isComplete());
            SolveEnded?.Invoke(this, new SolveEndedArgs(solution.isComplete(), message));
            return solution.isComplete();
        }

        public Board getPresentBoard()
        {
            if(solvingMethod == 1 || bm == null)
            {
                return solution;
            }
            return bm.copied;
        }

        public void killSolver()
        {
            foreach(Thread thread in threads)
            {
                thread.Abort();
            }
            PresentBoard?.Invoke(this, new PresentBoardArgs(solution.ToString()));
            SolveEnded?.Invoke(this, new SolveEndedArgs(solution.isComplete(), "중단되었습니다."));
        }

        public static bool fiveseconds()
        {
            Thread.Sleep(5000);
            Console.WriteLine("i counted five seconds...");
            return true;
        }

        void SolveBacktrack()
        {
            Console.WriteLine("backtraking initiated.");
            bm = new BacktrackingModule(originalBoard);
            var solved = bm.solve();
            string message = string.Empty;
            if (solved)
            {
                solution = bm.GetSolution().Copy();
                PresentBoard?.Invoke(this, new PresentBoardArgs(solution.ToString()));
            }
            Console.WriteLine(solution.ToString());
            Console.WriteLine("valid : " + solution.isValid());
            Console.WriteLine("complete : " + solution.isComplete());
            return;
        }
        
    }

    public class SolveEndedArgs : EventArgs
    {
        public bool completed { get; }
        public string message { get; }
        public SolveEndedArgs(bool c, string m)
        {
            completed = c;
            message = m;
        }
    }

    public class PresentBoardArgs : EventArgs
    {
        public string boardstring;
        public PresentBoardArgs(string bs)
        {
            boardstring = bs;
        }
    }
}
