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

namespace Sudoku
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

            ret = // valid dummy puzzle data!
                "6 9 3 7 2 4 5 1 8\n" +
                "1 2 7 8 5 9 3 6 4\n" +
                "4 8 5 3 1 6 9 2 7\n" +
                "5 6 9 4 8 2 7 3 1\n" +
                "3 7 8 5 6 1 4 9 2\n" +
                "2 1 4 9 7 3 8 5 6\n" +
                "8 4 2 1 3 5 6 7 9\n" +
                "7 5 6 2 9 8 1 4 3\n" +
                "9 3 1 6 4 7 2 8 5\n";
            Board board;
            board = new Board(ret);
            
            solve_backTrack(board);
            Thread.Sleep(1);
        }

        void solve_backTrack(Board board)
        {
            Console.WriteLine("hello, Cruel World!");
            board.Print_Board();
            return;
        }

    }

    public class Board
    {
        public int[,] boardData; // 0 은 빈칸
        public int gridSize; // 작은 네모칸의 사이즈

        public Board(string str)
        {
            string[] parsed = str.Trim().Split();
            if(parsed.Length != 81)
            {
                throw new System.ArgumentException("Parsing Error, Length failed.");
            }

            boardData = new int[9, 9];
            gridSize = 9;

            for(int i = 0; i < 9; i++)
            {
                for(int j = 0; j < 9; j++)
                {
                    boardData[i, j] = Int32.Parse(parsed[i * 9 + j]);
                }
            }
        }

        public void Print_Board()
            // 내부 판정보 프린트
        {
            for (int i = 0; i < gridSize; i++)
            {
                for(int j = 0; j < gridSize; j++)
                {
                    Console.Write(boardData[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("valid : " + isValid());
        }

        public bool isValid()
            // board validation
        {
            int sum, ir, ic, jr, jc;

            // 가로줄 확인
            for(int i = 0; i < gridSize; i++)
            {
                sum = 0;
                for(int j = 0; j < gridSize; j++)
                {
                    sum += boardData[i, j];
                }
                if (sum != gridSize * (gridSize + 1) / 2)
                    return false;
            }

            // 세로줄 확인
            for (int i = 0; i < gridSize; i++)
            {
                sum = 0;
                for (int j = 0; j < gridSize; j++)
                {
                    sum += boardData[j, i];
                }
                if (sum != gridSize * (gridSize + 1) / 2)
                    return false;
            }

            int sq = (int) Math.Sqrt(gridSize);
            
            // grid 내부 확인
            for(int i = 0; i < gridSize; i++)
            {
                sum = 0;
                ir = i / 3;
                ic = i % 3;
                for(int j = 0; j < gridSize; j++)
                {
                    jr = j / 3;
                    jc = j % 3;
                    sum += boardData[sq * ir + jr, sq * ic + jc];
                }
                if (sum != gridSize * (gridSize + 1) / 2)
                    return false;
            }
            return true;
        }
    }
}
