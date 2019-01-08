using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace MineSweeper
{
    public partial class MainWindow : Window
    {
        //Dimension of a game field dimension * dimension
        static int dimension = 9;

        //How many mines to place on a game field
        int minesCount = 10;

        //Tells how many flags you can place
        int flagsCount;

        Random random = new Random();

        //Array of button that will be each game field square
        Button[] square = new Button[(int)Math.Pow(dimension, 2)];

        //2D array that will be actuall game field representation (+2 on each side to avoid OutOfBoundsException)
        public string[,] field = new string[dimension + 2, dimension + 2];

        public MainWindow()
        {
            InitializeComponent();

            flagsCount = minesCount;

            for (int y = 0; y < dimension + 2; y++)
            {
                for (int x = 0; x < dimension + 2; x++)
                {
                    field[x, y] = "X";
                }
            }

            for (int y = 1; y < dimension + 1; y++)
            {
                for (int x = 1; x < dimension + 1; x++)
                {
                    field[x, y] = "";
                }
            }

            //puts randomly mines
            for (int i = 0; i < minesCount; i++)
            {
                int x = random.Next(1, dimension + 1);
                int y = random.Next(1, dimension + 1);

                while (field[x, y] == "M")
                {
                    x = random.Next(1, dimension + 1);
                    y = random.Next(1, dimension + 1);
                }

                field[x, y] = "M";
            }

            for (int y = 1; y < dimension + 1; y++)
            {
                for (int x = 1; x < dimension + 1; x++)
                {
                    if (field[x, y] != "M")
                    {
                        var count = 0;

                        for (int sub_x = x - 1; sub_x <= x + 1; sub_x++)
                        {
                            for (int sub_y = y - 1; sub_y <= y + 1; sub_y++)
                            {
                                if (sub_x == x && sub_y == y) continue;
                                else if (field[sub_x, sub_y] == "M") count++;
                            }
                        }

                        if (count > 0) field[x, y] = count.ToString();
                    }
                }
            }

            Grid gameField = new Grid
            {
                //Width
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            for(int i = 0; i<dimension; i++)
            {
                gameField.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(45) });
                gameField.RowDefinitions.Add(new RowDefinition { Height = new GridLength(45) });
            }

            for(int y = 0; y<dimension; y++)
            {
                for(int x = 0; x<dimension; x++)
                {
                    square[x + dimension * y] = new Button();
                    square[x + dimension * y].Click += Square_Click;
                    square[x + dimension * y].MouseRightButtonDown += Put_Flag;
                    Grid.SetColumn(square[x + dimension * y], x);
                    Grid.SetRow(square[x + dimension * y], y);
                    gameField.Children.Add(square[x + dimension * y]);
                }
            }

            Content = gameField;
        }

        private void Put_Flag(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            int x = Grid.GetColumn(button);
            int y = Grid.GetRow(button);

            if(flagsCount>0)
            {
                if((string)button.Content != "F")
                {
                    button.FontSize = 18;
                    button.FontWeight = FontWeights.Bold;
                    button.Content = "F";
                    flagsCount--;
                }
                else
                {
                    button.Content = "";
                    flagsCount++;
                }
            }
            else if(flagsCount == 0 && (string)button.Content == "F")
            {
                button.Content = "";
                flagsCount++;
            }

        }

        private void Square_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int x = Grid.GetColumn(button);
            int y = Grid.GetRow(button);

            //not mine and not empty
            if(field[x + 1, y + 1] != "M" && !string.IsNullOrEmpty(field[x + 1, y + 1]))
            {
                button.Content = field[x + 1, y + 1];

                if (field[x + 1, y + 1] == "1")
                {
                    button.Foreground = new SolidColorBrush(Colors.Blue);
                }
                else if (field[x + 1, y + 1] == "2")
                {
                    button.Foreground = new SolidColorBrush(Colors.Green);
                }
                else if (field[x + 1, y + 1] == "3")
                {
                    button.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (field[x + 1, y + 1] == "4")
                {
                    button.Foreground = new SolidColorBrush(Colors.Purple);
                }
                else if (field[x + 1, y + 1] == "5")
                {
                    button.Foreground = new SolidColorBrush(Colors.Maroon);
                }
                else if (field[x + 1, y + 1] == "6")
                {
                    button.Foreground = new SolidColorBrush(Colors.Turquoise);
                }
                else if (field[x + 1, y + 1] == "7")
                {
                    button.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (field[x + 1, y + 1] == "8")
                {
                    button.Foreground = new SolidColorBrush(Colors.Gray);
                }

                button.FontSize = 18;
                button.FontWeight = FontWeights.Bold;
                button.IsEnabled = false;
            }
            else if(string.IsNullOrEmpty(field[x + 1, y + 1]))
            {
                button.IsEnabled = false;

                //Queue<Point> emptyCells = new Queue<Point>();

                //emptyCells.Enqueue(new Point { X = x, Y = y });

                //while(emptyCells.Count > 0)
                //{
                //    Point cell = emptyCells.Dequeue();

                //    if (cell.X < 0 || cell.X > dimension - 1) continue;
                //    if (cell.Y < 0 || cell.Y > dimension - 1) continue;

                //    square[(int)cell.X + dimension * (int)cell.Y].IsEnabled = false;

                //    //west
                //    if (string.IsNullOrEmpty(field[(int)cell.X - 1, (int)cell.Y]))
                //    {
                //        emptyCells.Enqueue(new Point { X = (int)cell.X - 1, Y = (int)cell.Y });
                //    }

                //    //east
                //    if (string.IsNullOrEmpty(field[(int)cell.X + 1, (int)cell.Y]))
                //    {
                //        emptyCells.Enqueue(new Point { X = (int)cell.X + 1, Y = (int)cell.Y });
                //    }

                //    //north
                //    if (string.IsNullOrEmpty(field[(int)cell.X, (int)cell.Y - 1]))
                //    {
                //        emptyCells.Enqueue(new Point { X = (int)cell.X, Y = (int)cell.Y - 1});
                //    }

                //    //south
                //    if (string.IsNullOrEmpty(field[(int)cell.X, (int)cell.Y + 1]))
                //    {
                //        emptyCells.Enqueue(new Point { X = (int)cell.X, Y = (int)cell.Y + 1 });
                //    }
                //}

            }
            else
            {
                //show all mines on the game field and game over
                for (int _y = 0; _y<dimension; _y++)
                {
                    for(int _x = 0; _x<dimension; _x++)
                    {
                        square[_x + dimension * _y].Click -= Square_Click;

                        if (field[_x + 1, _y + 1] == "M")
                        {
                            square[_x + dimension * _y].Content = field[_x + 1, _y + 1];
                            square[_x + dimension * _y].FontSize = 18;
                            square[_x + dimension * _y].Background = Brushes.Red;
                            square[_x + dimension * _y].FontWeight = FontWeights.Bold;
                        }
                    }
                }
            }
        }
    }
}
