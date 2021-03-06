﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Sudoku.Modules;
using System.Diagnostics;
using System.Windows.Threading;

namespace Sudoku
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void PresentBoardCallback(string boardstring);
        public delegate void TimerCallback();

        List<Grid> textGridReference = new List<Grid>();
        int gridSize;

        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();

        //private BacktrackingModule bm = null;
        Solver solver;
        Generator gn;
        Thread solvingThread;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1000/60); // Tick 60fps
            dt.Tick += new EventHandler(dispatcherTimer_Tick);
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                xTextBlockElapsedTime.Text = stopWatch.Elapsed.ToString();
                // StopWatch가 돌고 있을때는 문제를 풀고 있을 것이므로 문제의 결과를 받아와서 결과 화면에 적을 수 있어야 한다.
                if(solver != null)
                    PresentBoard(solver.getPresentBoard().ToString());
                if (gn != null)
                    PresentBoard(gn.getPresentBoard().ToString());
            }
            else {
                dt.Stop();
                StatusReady();
            }
        }

        void MainWindow_Loaded(object sender, EventArgs e)
        {
            xComboBoxSelectPuzzleSize.SelectionChanged += xComboBoxGridsizeSelectChanged;
            xComboBoxSelectSampleInput.SelectionChanged += xComboBoxSampleInputChanged;
            xButtonSolveNow.Click += xButtonSolveNowPressed;
            //xButtonRandomGenerate.Click += xButtonRandomGeneratePressed;
            xButtonStopSolve.Click += xButtonStopSolvePressed;
            StatusReady(); // 처음에 한번 초기화.
            xComboBoxSelectSampleInput.SetValue(ComboBox.SelectedIndexProperty, 4);
        }


        private void xButtonRandomGeneratePressed(object sender, RoutedEventArgs e)
        {
            int difficulty = xComboBoxGenerateProblemDifficulty.SelectedIndex;
            int[] clues = { 52, 38, 28, 27, 19 };
            int numGrid = gridSize * gridSize;
            int numGenerate = Int32.Parse(xTextBoxInputProblems.Text);
            StatusRunning("생성중");
            stopWatch.Restart();

            gn = new Generator(gridSize * gridSize);
            gn.PresentBoard += sv_PresentBoard;
            gn.GenerateEnded += gn_GenerateEnded;
            solvingThread = new Thread(() => 
                gn.generate(numGenerate, numGrid * numGrid - clues[difficulty]));
            solvingThread.Start();
            dt.Start();
        }

        private void xButtonStopSolvePressed(object sender, RoutedEventArgs e)
        {
            //만약 solve stop일 경우 solver 중단, gn stop일 경우 gn 중단
            if(solver != null)
                solver.killSolver();
            if (gn != null)
                gn.endGenerate();
            solvingThread.Abort();
        }

        void xComboBoxGridsizeSelectChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                gridSize = 3;
                var value = comboBox.SelectedItem as ComboBoxItem;
                //int gridsize = 3;
                int.TryParse(value.Content.ToString().Substring(0, 1), out gridSize); // Selection의 첫글자만 파싱..
                GenerateGrid(gridSize);
            }
        }

        void xComboBoxSampleInputChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if((int) xComboBoxSelectPuzzleSize.GetValue(ComboBox.SelectedIndexProperty) == 1)
            {
                string data = "" +
                    "0 0 10 0 0 0 0 13 0 6 16 0 0 0 1 0\n" +
                    "6 1 0 9 0 0 0 11 0 0 0 0 10 0 0 4\n" +
                    "0 14 15 5 0 7 10 0 0 0 0 3 0 0 12 6\n" +
                    "8 2 0 0 0 0 14 15 7 0 0 0 13 9 11 0\n" +
                    "0 0 2 0 16 12 9 7 15 0 5 0 3 0 6 0\n" +
                    "0 0 0 10 0 0 5 2 8 0 0 0 0 0 7 0\n" +
                    "12 3 0 0 14 15 13 0 0 7 0 0 8 0 0 1\n" +
                    "0 0 0 7 0 6 1 10 0 0 3 2 9 12 0 5\n" +
                    "0 0 0 0 13 1 0 0 2 0 0 8 0 14 0 0\n" +
                    "0 0 16 0 0 2 0 0 0 9 0 6 0 0 0 0\n" +
                    "0 0 0 0 0 0 0 0 0 11 12 4 0 10 5 0\n" +
                    "0 9 0 14 0 0 0 0 0 10 7 0 16 0 0 2\n" +
                    "10 6 0 0 5 0 0 0 0 8 0 0 7 0 15 9\n" +
                    "0 0 3 11 2 16 0 0 0 12 1 0 6 0 0 0\n" +
                    "13 0 0 0 4 10 0 0 0 0 2 0 0 5 3 8\n" +
                    "0 0 0 0 7 0 0 0 3 16 13 0 1 0 2 0";
                PresentBoard(data);
                return;
            }
            if (comboBox != null)
            {
                string[] data = {
            // SAMPLE MULTITHREADING DATA"
                "0 0 8 6 9 0 0 0 0 4 0 6 0 8 0 0 0 0 0 9 0 0 0 4 0 0 0 0 0 0 0 0 5 0 0 6 1 0 0 0 2 0 4 0 0 0 0 3 9 0 0 0 7 0 0 0 7 4 0 0 0 3 0 2 0 0 0 1 0 5 0 0 0 1 0 0 0 9 0 0 8 ",

            // Evil Level..(very hard)
                "0 0 0 0 0 0 0 0 0\n" +
                "5 0 8 0 0 0 0 0 0\n" +
                "0 0 0 2 0 0 0 0 1\n" +
                "0 0 0 5 0 0 0 9 0\n" +
                "0 0 0 0 0 1 0 0 6\n" +
                "9 0 6 0 0 0 4 0 0\n" +
                "0 3 1 0 0 6 0 0 7\n" +
                "0 0 0 7 2 0 8 0 0\n" +
                "0 8 2 0 9 0 0 0 3\n",

            // single validation
                "0 0 0 0 0 0 5 0 0\n" +
                "1 6 0 9 0 0 0 0 0\n" +
                "0 0 9 0 6 4 0 0 0\n" +
                "0 0 0 0 0 0 0 0 4\n" +
                "4 0 0 0 2 0 1 0 0\n" +
                "0 0 0 3 0 0 0 5 0\n" +
                "0 0 2 0 8 9 0 0 0\n" +
                "0 1 0 2 5 0 0 3 0\n" +
                "7 0 0 1 0 0 0 0 9\n",

            // sample puzzle
                "5 3 0 0 7 0 0 0 0\n" +
                "6 0 0 1 9 5 0 0 0\n" +
                "0 9 8 0 0 0 0 6 0\n" +
                "8 0 0 0 6 0 0 0 3\n" +
                "4 0 0 8 0 3 0 0 1\n" +
                "7 0 0 0 2 0 0 0 6\n" +
                "0 6 0 0 0 0 2 8 0\n" +
                "0 0 0 4 1 9 0 0 5\n" +
                "0 0 0 0 8 0 0 7 9",

            // 0000000
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0" };
                PresentBoard(data[comboBox.SelectedIndex]);
            }
        }

        void GenerateGrid(int size)
        {
            // 그리드 초기화
            xGridPuzzleBoard.Children.Clear();
            xGridPuzzleBoard.ColumnDefinitions.Clear();
            xGridPuzzleBoard.RowDefinitions.Clear();
            textGridReference.Clear();
            int poweredsize = size * size;
            // 크기에 맞는 새로운 퍼즐 그리드를 생성한다.
            for (int i = 0; i < poweredsize; i++)
            {
                xGridPuzzleBoard.ColumnDefinitions.Add(new ColumnDefinition());
                xGridPuzzleBoard.RowDefinitions.Add(new RowDefinition());
            }

            for (int l = 0; l < size; l++)
            {
                for (int m = 0; m < size; m++)
                {
                    var border = new Border { BorderThickness = new Thickness(2), BorderBrush = Brushes.DeepPink };
                    xGridPuzzleBoard.Children.Add(border);
                    border.SetValue(Grid.ColumnProperty, l * size);
                    border.SetValue(Grid.RowProperty, m * size);
                    border.SetValue(Grid.ColumnSpanProperty, size);
                    border.SetValue(Grid.RowSpanProperty, size);
                }
            }

            for (int j = 0; j < poweredsize; j++)
            {
                for (int k = 0; k < poweredsize; k++)
                {
                    var border = new Border() { BorderThickness = new Thickness(1) };
                    var textGrid = new Grid();
                    textGrid.Children.Add(new TextBox() { Text = "", FontSize = 30, TextAlignment = TextAlignment.Center });
                    border.Child = textGrid;
                    textGridReference.Add(textGrid);
                    xGridPuzzleBoard.Children.Add(border);
                    border.SetValue(Grid.ColumnProperty, k);
                    border.SetValue(Grid.RowProperty, j);
                }
            }
        }

        public void xButtonSolveNowPressed(object sender, EventArgs e)
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();
            // 여기를 Task로 띄워서 날려보내야 한다.
            var board = new Board(ParseInputBox());

            solver = new Solver(board);
            solver.SolveEnded += sv_SolveEnded;
            solver.PresentBoard += sv_PresentBoard;

            // 방법 선택. 0 == backtrack, 1 == heuristic
            var m = xComboBoxSelectMethod.SelectedIndex;

            if (xCheckBoxMultithreadEnable.IsChecked.Value)
            {
                solvingThread = new Thread(() => {
                    Console.WriteLine("enabled multithread");
                    solver.solve(m, true);
                });
            }
            else
            {
                solvingThread = new Thread(() => {
                    solver.solve(m);
                });
            }
            StatusRunning("문제 해결중");
            dt.Start();
            solvingThread.Start();
        }

        public void xButtonGetWebSamplePressed(object sender, EventArgs e)
        {
            HttpParser hp = new HttpParser(gridSize);
            int difficulty = xComboBoxSampleDifficulty.SelectedIndex + 1;
            Random r = new Random();
            int num = r.Next() % 10000;
            string ret = "";
            if(gridSize == 3)
                ret = hp.getBoardStringFromURL("http://en.top-sudoku.com/cgi-bin/sudoku/print-a-sudoku-puzzle.cgi?eso=1&niv=" + difficulty + "&num=" + num);
            if(gridSize == 4)
                ret = hp.getBoardStringFromURL("http://en.top-sudoku.com/cgi-bin/hexadoku/print-1-grid-hexadoku.cgi?" + difficulty);
            if(gridSize == 5)
                MessageBox.Show("Implement this...");
            PresentBoard(ret);
        }

        public void xButtonSaveToFilePressed(object sender, EventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "boardstring";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                System.IO.File.WriteAllText(filename, ParseInputBox());
            }
        }

        public void xButtonLoadFromFilePressed(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "boardstring";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                PresentBoard(System.IO.File.ReadAllText(filename));
            }
        }

        public void sv_SolveEnded(object sender, SolveEndedArgs e)
            // solver가 수행을 끝나면 실행하는 이벤트
        {
            if (!stopWatch.IsRunning)
                return;
            stopWatch.Stop(); // StopWatch를 멈춘다.
            xTextBlockElapsedTime.Dispatcher.Invoke(
                new TimerCallback(() => xTextBlockElapsedTime.Text = stopWatch.Elapsed.ToString())
            ); // 경과시간 작성
            
            //board는 solver가 업데이트함.
            MessageBox.Show(e.completed ? "성공" : "실패 " + ":" + e.message);
            solver = null;
        }

        public void gn_GenerateEnded(object sender, EventArgs e)
        {
            stopWatch.Stop(); // StopWatch를 멈춘다.
            xTextBlockElapsedTime.Dispatcher.Invoke(
                new TimerCallback(() => xTextBlockElapsedTime.Text = stopWatch.Elapsed.ToString())
            ); // 경과시간 작성
            gn = null;
            //create file 
        }

        void PresentBoard(string boardString)
        {
            var target = boardString.Trim().Replace("\r\n", " ").Replace("  ", " ").Split();
            var size = textGridReference.Count;
            if (size != target.Length)
            {
                return;
                //throw new Exception("size does not match!");
            }
                
            for (int i = 0; i < size; i++)
            {
                var targetGrid = textGridReference[i];
                if (target[i].CompareTo("0") == 0)
                    target[i] = "";
                targetGrid.Children[0].SetValue(TextBox.TextProperty, target[i]);
            }
        }

        string ParseInputBox()
        {
            var size = xGridPuzzleBoard.Children.Count;
            StringBuilder sb = new StringBuilder();
            foreach (var targetGrid in textGridReference)
            {
                string a = (string)targetGrid.Children[0].GetValue(TextBox.TextProperty);
                if (a.CompareTo("") == 0)
                    a = "0";
                sb.Append(a + " ");
            }
            return sb.ToString();
        }

        void sv_PresentBoard(object sender, PresentBoardArgs e)
        {
            xGridPuzzleBoard.Dispatcher.Invoke(
                new PresentBoardCallback(PresentBoard),
                new object[] { e.boardstring }
            );
        }

        public void StatusReady()
        {
            // 상태를 대기 로 표시한다.
            xTextBlockStatus.Text = "대기";
            xComboBoxSelectPuzzleSize.IsEnabled = true;
            xComboBoxSelectSampleInput.IsEnabled = true;
            xButtonSolveNow.IsEnabled = true;
            xButtonGenerateProblems.IsEnabled = true;
            xButtonStopSolve.IsEnabled = false;
        }

        public void StatusRunning(string message)
        {
            xTextBlockStatus.Text = message;
            xComboBoxSelectPuzzleSize.IsEnabled = false;
            xComboBoxSelectSampleInput.IsEnabled = false;
            xButtonSolveNow.IsEnabled = false;
            xButtonGenerateProblems.IsEnabled = false;
            xButtonStopSolve.IsEnabled = true;
        }
    }
}
