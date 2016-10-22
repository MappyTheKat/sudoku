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
            this.Loaded += MainWindow_Loaded;
        }


        void MainWindow_Loaded(object sender, EventArgs e)
        {
            xComboBoxSelectPuzzleSize.SelectionChanged += xComboBoxGridsizeSelectChanged;
        }

        void xComboBoxGridsizeSelectChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                var value = comboBox.SelectedItem as ComboBoxItem;
                int gridsize = 3;
                int.TryParse(value.Content.ToString().Substring(0, 1), out gridsize); // Selection의 첫글자만 파싱..
                GenerateGrid(gridsize);
            }
        }

        void GenerateGrid(int size)
        {
            // 그리드 초기화
            xGridPuzzleBoard.Children.Clear();
            xGridPuzzleBoard.ColumnDefinitions.Clear();
            xGridPuzzleBoard.RowDefinitions.Clear();
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
                    textGrid.Children.Add(new TextBox() { Text = "0", FontSize = 30, TextAlignment = TextAlignment.Center});
                    border.Child = textGrid;
                    xGridPuzzleBoard.Children.Add(border);
                    border.SetValue(Grid.ColumnProperty, j);
                    border.SetValue(Grid.RowProperty, k);
                }
            }
        }

        void xButton_Pressed(object sender, EventArgs e)
        {
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
                "0 9 3 7 2 4 5 1 8\n" +
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
            Console.WriteLine(board.to_string());
            Console.WriteLine("valid : " + board.isValid());
            Console.WriteLine("complete : " + board.isComplete());
            return;
        }

    }
}
