﻿using System;
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

        Board originalBoard;
        int gridSize;

        public Solver(Board b)
        {
            originalBoard = b.Copy();
            solution = b.Copy();
            gridSize = originalBoard.gridSize;
        }

        public void solve(int solvingMethod)
            //solving method 0 : backtraking,  1 : heuristic
        {
            List<Thread> threads = new List<Thread>();
            string message = "";
            if (solvingMethod == 0)
                SolveBacktrack();
            if (solvingMethod == 1)
            {
                //do some hueristic...
                bool done = false;
                threads.Add(new Thread(
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
                    }));
                threads.Add(new Thread(
                    () =>
                    {
                        SingleModule sm = new SingleModule(originalBoard);
                        if (sm.Solve())
                            done = true;
                        message = "single completed";
                    }));
                foreach (var t in threads)
                    t.Start();
                while (!done)
                {
                    //implement job scheduler...
                    Console.WriteLine("waiting for completing");
                    Thread.Sleep(30);
                }
                foreach (var t in threads)
                    t.Abort();
            }
            
            SolveEnded(this, new SolveEndedArgs(solution.isComplete(), message));
        }

        public static bool fiveseconds()
        {
            Thread.Sleep(5000);
            Console.WriteLine("i counted five seconds...");
            return true;
        }

        void SolveBacktrack()
        {
            Board board = originalBoard;
            Console.WriteLine("hello, Cruel World!");
            Console.WriteLine("valid board?: " + board.isValid());
            bm = new BacktrackingModule(board);
            var solved = bm.solve();
            string message = string.Empty;
            if (solved)
            {
                board = bm.GetSolution();
                solution = board;
                PresentBoard(this, new PresentBoardArgs(board.ToString()));
            }
            Console.WriteLine(board.ToString());
            Console.WriteLine("valid : " + board.isValid());
            Console.WriteLine("complete : " + board.isComplete());
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