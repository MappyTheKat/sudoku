using System;
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
        List<Grid> textGridReference = new List<Grid>();
        int gridSize;

        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();

        private BacktrackingModule bm = null;

        private Board board = null;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1); // Tick 1 ms
            dt.Tick += new EventHandler(dispatcherTimer_Tick);
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                xTextBlockElapsedTime.Text = stopWatch.Elapsed.ToString();
                // StopWatch가 돌고 있을때는 문제를 풀고 있을 것이므로 문제의 결과를 받아와서 결과 화면에 적을 수 있어야 한다.
                PresentBoard(board.ToString());
            }
        }

        void MainWindow_Loaded(object sender, EventArgs e)
        {
            xComboBoxSelectPuzzleSize.SelectionChanged += xComboBoxGridsizeSelectChanged;
            xComboBoxSelectSampleInput.SelectionChanged += xComboBoxSampleInputChanged;
            xButtonSolveNow.Click += xButtonSolveNowPressed;
            xButtonRandomGenerate.Click += xButtonRandomGeneratePressed;
            StatusReady(); // 처음에 한번 초기화.
        }

        private void xButtonRandomGeneratePressed(object sender, RoutedEventArgs e)
        {
            Generator a = new Generator(gridSize * gridSize);
            //Console.WriteLine("Generate");
            Board genb = a.generate(0);
            PresentBoard(genb.ToString());
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
            if (comboBox != null)
            {
                string[] data = {
            // invalid dummy puzzle Data!"
                "0 0 0 0 0 0 7 0 0\n" +
                "5 7 8 0 0 0 0 6 0\n" +
                "0 0 0 2 0 0 0 0 1\n" +
                "0 0 0 5 0 0 0 9 0\n" +
                "0 0 0 0 0 1 0 0 6\n" +
                "9 0 6 0 0 0 4 0 0\n" +
                "0 3 1 0 0 6 0 0 7\n" +
                "0 0 0 7 2 0 8 0 0\n" +
                "0 8 2 0 9 0 0 0 3\n",

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

            // valid punched dummy puzzle data!
                "0 0 0 0 2 4 5 1 8\n" +
                "0 2 7 8 5 9 3 6 4\n" +
                "0 0 0 3 1 0 9 2 7\n" +
                "5 6 9 4 8 2 7 3 1\n" +
                "3 7 8 5 6 1 4 9 2\n" +
                "0 0 0 9 7 3 8 5 6\n" +
                "8 4 2 1 3 5 6 7 9\n" +
                "7 5 6 2 9 8 0 4 0\n" +
                "9 3 1 6 4 7 0 0 0\n",

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

            for (int j = 0; j < poweredsize; j++)
            {
                for (int k = 0; k < poweredsize; k++)
                {
                    var border = new Border() { BorderThickness = new Thickness(1) };
                    var textGrid = new Grid();
                    textGrid.Children.Add(new TextBox() { Text = "0", FontSize = 30, TextAlignment = TextAlignment.Center });
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
            board = new Board(ParseInputBox());
            var task = Dispatcher.BeginInvoke(new Action(() => SolveBacktrack(board)));
            task.Completed += SolveCompleted;
            //xTextBlockElapsedTime.Text = stopWatch.Elapsed.ToString();
            //Console.WriteLine("Time elapsed: " + stopWatch.Elapsed.ToString());
        }

        public void SolveCompleted(object sender, EventArgs e)
        {
            stopWatch.Stop(); // StopWatch를 멈춘다.

            xTextBlockElapsedTime.Text = stopWatch.Elapsed.ToString(); // 경과 시간을 적는다.

            string message = string.Empty;
            if (bm.IsSolved)
            {
                board = bm.GetSolution();
                PresentBoard(board.ToString()); // 마지막으로 결과를 다 적고.
            }
            else
            {
                message = ("Failed to solve");
            }
            SolveEnded(board.isComplete(), message);
        }

        void PresentBoard(string boardString)
        {
            var target = boardString.Trim().Replace("\n", " ").Replace("  ", " ").Split();
            var size = textGridReference.Count;
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

        public void SolveBacktrack(Board board)
        {
            Console.WriteLine("hello, Cruel World!");
            Console.WriteLine("valid board?: " + board.isValid());
            bm = new BacktrackingModule(board);
            bm.solve();
            Console.WriteLine(board.ToString());
            Console.WriteLine("valid : " + board.isValid());
            Console.WriteLine("complete : " + board.isComplete());
            return;
        }

        void bm_PrintCall(object sender, BacktrackingModule.PresentArgs e)
        // when a solving module sends a present signal.
        {
            //PresentBoard(e.boardString);
            //Thread.Sleep(1);
            textGridReference[e.pos.Item1 * gridSize + e.pos.Item2].Children[0].SetValue(TextBox.TextProperty, e.value.ToString());
            //Console.WriteLine("it has called...");
            //Environment.Exit(0);
        }

        void SolveEnded(bool isValid, string message)
        {
            MessageBox.Show(isValid ? "성공" : "실패 " + ":" + message);
            StatusReady();
        }

        public void StatusReady()
        {
            // 상태를 대기 로 표시한다.
            xTextBlockStatus.Text = "대기";
            xComboBoxSelectPuzzleSize.IsEnabled = true;
            xComboBoxSelectSampleInput.IsEnabled = true;
            xButtonSolveNow.IsEnabled = true;
            xButtonRandomGenerate.IsEnabled = true;
        }

        public void StatusRunning()
        {
            xTextBlockStatus.Text = "문제 해결중";
            xComboBoxSelectPuzzleSize.IsEnabled = false;
            xComboBoxSelectSampleInput.IsEnabled = false;
            xButtonSolveNow.IsEnabled = false;
            xButtonRandomGenerate.IsEnabled = false;
        }
    }
}
