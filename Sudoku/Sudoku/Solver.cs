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

        bool multi_enabled;
        List<Board> multi_candidates;

        int gridSize;
        int solvingMethod = 0;

        public Solver(Board b)
        {
            originalBoard = b.Copy();
            solution = b.Copy();
            gridSize = originalBoard.gridSize;
        }

        //override 함수
        public bool solve(int solm)
        {
            return solve(solm, false);
        }

        public bool solve(int solm, bool enable_multithread)
            //solving method 0 : backtraking,  1 : heuristic
            //returns true when solution found...
        {
            multi_enabled = enable_multithread;
            solvingMethod = solm;
            threads = new List<Thread>();
            string message = "";
            bm = null;

            if(!originalBoard.isValid())
            {// 보드가 정상이 아니네용...
                SolveEnded?.Invoke(this, new SolveEndedArgs(solution.isComplete(), "invalid puzzle..."));
                return false;
            }

            if (originalBoard.isComplete())
            {
                //이미 보드는 컴플리트
                message = "이미 완성된 보드입니다.";
                SolveEnded?.Invoke(this, new SolveEndedArgs(solution.isComplete(), message));
                return true;
            }

            if (solvingMethod == 0)
            {
                if (enable_multithread)
                {
                    multi_candidates = createCandidates();
                    bool done = false;
                    int counter = multi_candidates.Count;
                    foreach(Board candy in multi_candidates)
                    {
                        threads.Add(new Thread(() => {
                            if(solveBacktrack(candy))
                                done = true;
                            counter--;
                        }));
                    }
                    foreach (var t in threads)
                        t.Start();
                    while (!done && counter != 0)
                    {
                        //implement job scheduler...
                        Thread.Sleep(30);
                    }
                    foreach (var t in threads)
                        t.Abort();
                }
                else
                {
                    threads.Add(new Thread(SolveBacktrack));
                    threads[0].Start();
                    threads[0].Join();
                }
            }
            if (solvingMethod >= 1)
            {
                //do some hueristic...
                bool done = false;
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
                if (enable_multithread)
                {
                    multi_candidates = createCandidates();
                    bool done = false;
                    int counter = multi_candidates.Count;
                    foreach (Board candy in multi_candidates)
                    {
                        threads.Add(new Thread(() => {
                            if (solveBacktrack(candy))
                                done = true;
                            counter--;
                        }));
                    }
                    foreach (var t in threads)
                        t.Start();
                    while (!done && counter != 0)
                    {
                        //implement job scheduler...
                        Thread.Sleep(30);
                    }
                    foreach (var t in threads)
                        t.Abort();
                }
                else
                {
                    threads.Add(new Thread(SolveBacktrack));
                    threads[0].Start();
                    threads[0].Join();
                }
            }
            Console.WriteLine("solver returned {0}", solution.isComplete());
            SolveEnded?.Invoke(this, new SolveEndedArgs(solution.isComplete(), message));
            return solution.isComplete();
        }

        //create candidates for multithreading.
        private List<Board> createCandidates()
        {
            List<Board> candidates = new List<Board>();

            for(int i = 0; i < gridSize; i++)
            {
                for(int j = 0; j < gridSize; j++)
                {
                    if(originalBoard.getBoard(i, j) == 0)
                    {
                        for(int k = 1; k <= gridSize; k++)
                        {
                            Board candy = originalBoard.Copy();
                            candy.setBoard(i, j, k);
                            if (candy.isValid())
                            {
                                candidates.Add(candy);
                            }
                        }
                        i = gridSize;
                        break;
                    }
                }
            }

            return candidates;
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
            //Console.WriteLine("backtraking initiated.");
            bm = new BacktrackingModule(originalBoard);
            var solved = bm.solve();
            string message = string.Empty;
            if (solved)
            {
                solution = bm.GetSolution().Copy();
                PresentBoard?.Invoke(this, new PresentBoardArgs(solution.ToString()));
            }
            //Console.WriteLine(solution.ToString());
            //Console.WriteLine("valid : " + solution.isValid());
            //Console.WriteLine("complete : " + solution.isComplete());
            return;
        }
        
        //solve backtrack for specific board.
        bool solveBacktrack(Board b)
        {
            //Console.WriteLine("multithreaded-backtraking initiated.");
            BacktrackingModule BM = new BacktrackingModule(b);
            if (bm == null)
                bm = BM;
            var solved = BM.solve();
            string message = string.Empty;
            if (solved)
            {
                bm = BM;
                solution = BM.GetSolution().Copy();
                PresentBoard?.Invoke(this, new PresentBoardArgs(solution.ToString()));
            }
            //Console.WriteLine("valid : " + solution.isValid());
            //Console.WriteLine("complete : " + solution.isComplete());
            return solved;
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
