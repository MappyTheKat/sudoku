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

namespace Sudoku
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Grid> textGridReference = new List<Grid>();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, EventArgs e)
        {
            xComboBoxSelectPuzzleSize.SelectionChanged += xComboBoxGridsizeSelectChanged;
            xComboBoxSelectSampleInput.SelectionChanged += xComboBoxSampleInputChanged;
            xButtonSolveNow.Click += xButtonSolveNowPressed;
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

        void xComboBoxSampleInputChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if(comboBox != null)
            {
                string[] data = {
            // invalid dummy puzzle Data!"
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n" +
                "1 2 3 4 5 6 7 8 9\n",

            // valid original dummy puzzle data!
                "6 9 3 7 2 4 5 1 8\n" +
                "1 2 7 8 5 9 3 6 4\n" +
                "4 8 5 3 1 6 9 2 7\n" +
                "5 6 9 4 8 2 7 3 1\n" +
                "3 7 8 5 6 1 4 9 2\n" +
                "2 1 4 9 7 3 8 5 6\n" +
                "8 4 2 1 3 5 6 7 9\n" +
                "7 5 6 2 9 8 1 4 3\n" +
                "9 3 1 6 4 7 2 8 5\n",

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
                    textGridReference.Add(textGrid);
                    xGridPuzzleBoard.Children.Add(border);
                    border.SetValue(Grid.ColumnProperty, j);
                    border.SetValue(Grid.RowProperty, k);
                }
            }
        }

        void xButtonSolveNowPressed(object sender, EventArgs e)
        {
            Board board = new Board(ParseInputBox());
            SolveBacktrack(board);
            Thread.Sleep(1);
        }

        void PresentBoard(string boardString)
        {
            var target = boardString.Trim().Split();
            var size = textGridReference.Count;
            for (int i = 0; i < size; i++)
            {
                var targetGrid = textGridReference[i];
                targetGrid.Children[0].SetValue(TextBox.TextProperty, target[i]);
            }
        }

        string ParseInputBox()
        {
            var size = xGridPuzzleBoard.Children.Count;
            StringBuilder sb = new StringBuilder();
            foreach(var targetGrid in textGridReference)
            {
                sb.Append(targetGrid.Children[0].GetValue(TextBox.TextProperty) + " ");
            }
            return sb.ToString();
        }

        void SolveBacktrack(Board board)
        {
            Console.WriteLine("hello, Cruel World!");
            Console.WriteLine(board.ToString());
            
            Console.WriteLine("valid : " + board.isValid());
            Console.WriteLine("complete : " + board.isComplete());
            return;
        }

    }
}
