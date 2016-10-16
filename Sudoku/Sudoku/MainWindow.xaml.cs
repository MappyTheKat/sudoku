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
            Thread.Sleep(1);
        }
    }
}
