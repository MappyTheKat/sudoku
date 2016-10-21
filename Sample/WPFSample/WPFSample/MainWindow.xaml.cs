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

namespace WPFSample
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            xButtonBelow.Click += xButton_Pressed;

            this.Loaded += MainWindow_Loaded;
            this.xTextBoxTextInput.TextChanged += xTextBoxTextInputChanged;
        }

        void xTextBoxTextInputChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            xTextBlockHelloworld.Text = textbox.Text;

        }

        void MainWindow_Loaded(object sender, EventArgs e)
        {
            xTextBlockHelloworld.Text = "Loaded";
        }

        void xButton_Pressed(object sender, EventArgs e)
        {
            xTextBlockHelloworld.Text = "hahahahahahaha";

            string ret = "";

            ret = // invalid dummy puzzle Data!"
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n";

            ret = // valid original dummy puzzle data!
                "6 9 3 7 2 4 5 1 8\n" +
                "1 2 7 8 5 9 3 6 4\n" +
                "4 8 5 3 1 6 9 2 7\n" +
                "5 6 9 4 8 2 7 3 1\n" +
                "3 7 8 5 6 1 4 9 2\n" +
                "2 1 4 9 7 3 8 5 6\n" +
                "8 4 2 1 3 5 6 7 9\n" +
                "7 5 6 2 9 8 1 4 3\n" +
                "9 3 1 6 4 7 2 8 5\n";

            ret = // valid punched dummy puzzle data!
                "0 0 0 0 2 4 5 1 8\n" +
                "0 2 7 8 5 9 3 6 4\n" +
                "0 0 0 3 1 0 9 2 7\n" +
                "5 6 9 4 8 2 7 3 1\n" +
                "3 7 8 5 6 1 4 9 2\n" +
                "0 0 0 9 7 3 8 5 6\n" +
                "8 4 2 1 3 5 6 7 9\n" +
                "7 5 6 2 9 8 0 4 0\n" +
                "9 3 1 6 4 7 0 0 0\n";

            ret = // sample puzzle
                "5 3 0 0 7 0 0 0 0\n" +
                "6 0 0 1 9 5 0 0 0\n" +
                "0 9 8 0 0 0 0 6 0\n" +
                "8 0 0 0 6 0 0 0 3\n" +
                "4 0 0 8 0 3 0 0 1\n" +
                "7 0 0 0 2 0 0 0 6\n" +
                "0 6 0 0 0 0 2 8 0\n" +
                "0 0 0 4 1 9 0 0 5\n" +
                "0 0 0 0 8 0 0 7 9";

            ret = // 0000000
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0\n" +
                "0 0 0 0 0 0 0 0 0";

            Board board;
            board = new Board(ret);

            solve_backTrack(board);
            Thread.Sleep(1);
        }

        void solve_backTrack(Board board)
        {
            Console.WriteLine("hello, Cruel World!");
            Console.WriteLine("valid board?: " + board.isValid());
            module_backtrack mb = new module_backtrack(board);
            mb.solve();
            mb.clone_to_original();
            Console.WriteLine(mb.original.to_string());
            Console.WriteLine("valid : " + mb.original.isValid());
            Console.WriteLine("complete : " + mb.original.isComplete());
            return;
        }

    }

    public class Board
    {
        public int[,] boardData; // 0 은 빈칸
        public int gridSize; // 작은 네모칸의 사이즈

        public Board(string str)
        {
            string[] parsed = str.Trim().Replace(" \n", " ").Split();
            if (parsed.Length != 81)
            {
                throw new System.ArgumentException("Parsing Error, Length failed.");
            }

            boardData = new int[9, 9];
            gridSize = 9;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    boardData[i, j] = Int32.Parse(parsed[i * 9 + j]);
                }
            }
        }

        public string to_string()
        // 내부 판정보 string 반환
        {
            string a = "";
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    a += boardData[i, j] + " ";
                }
                a += "\n";
            }
            return a;
        }

        public void solve()
        {
            Console.WriteLine("solving started");
            module_backtrack mb = new module_backtrack(this);
            mb.solve();
            Console.WriteLine("Solving completed");
        }

        public bool isValid()
        {
            //check wether board is valid

            //row validate
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 1; j <= gridSize; j++)
                {
                    int cnt = 0;
                    for (int k = 0; k < gridSize; k++)
                    {
                        if (boardData[i, k] == j)
                            cnt++;
                    }
                    if (cnt > 1)
                        return false;
                }
            }

            //column validate
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 1; j <= gridSize; j++)
                {
                    int cnt = 0;
                    for (int k = 0; k < gridSize; k++)
                    {
                        if (boardData[k, i] == j)
                            cnt++;
                    }
                    if (cnt > 1)
                        return false;
                }
            }

            //grid validate
            int width = (int)Math.Sqrt(gridSize);
            int row, col;
            for (int i = 0; i < gridSize; i++)
            {
                row = (i / width) * width;
                col = (i % width) * width;
                for (int j = 1; j <= gridSize; j++)
                {
                    int cnt = 0;
                    for (int k = 0; k < gridSize; k++)
                    {
                        if (boardData[row + (k / width), col + (k % width)] == j)
                            cnt++;
                    }
                    if (cnt > 1)
                        return false;
                }
            }

            return true;
        }

        public bool isComplete()
        // check whether board is completed
        {
            if (!isValid())
                return false;

            int sum = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    sum += boardData[i, j];
                }
            }
            return sum == (gridSize + 1) * gridSize / 2 * gridSize;
        }
    }

    public class module_backtrack
    {
        public Board original;
        Board copied;
        int gridSize;
        int cnt = 0;
        public module_backtrack(Board b)
        {
            original = b;
            copied = new Board(b.to_string());
            gridSize = copied.gridSize;
        }

        public void clone_to_original()
        {
            original = copied;
        }


        private int count_zero()
        //copied 내부의 0 개수 반환
        {
            int c = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (copied.boardData[i, j] == 0)
                        c++;
                }
            }
            return c;
        }

        public bool solve()
        {
            if (!original.isValid())
            {
                Console.WriteLine("unsolvable puzzle: original data Invalid!");
                return false;
            }
            int zero = count_zero();
            return backtrack(zero);
        }

        public bool backtrack(int n)
        {
            int i, j, k;
            if (n == 0)
                return copied.isComplete();

            for (i = 0; i < gridSize; i++)
            {
                for (j = 0; j < gridSize; j++)
                {
                    cnt++;
                    if (copied.boardData[i, j] == 0)
                    {
                        for (k = 1; k <= gridSize; k++)
                        {
                            copied.boardData[i, j] = k;
                            if (copied.isValid() && backtrack(n - 1))
                            {
                                //Console.WriteLine(cnt);
                                return true;
                            }
                        }
                        copied.boardData[i, j] = 0;
                        return false;
                    }
                    else
                    {
                        continue;
                    }                    
                }
            }
            //Console.WriteLine(cnt);
            return false;
        }
    }
}
