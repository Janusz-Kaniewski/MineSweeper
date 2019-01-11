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
        //Dimension of a game field
        static int dimension_x = 9;
        static int dimension_y = 9;

        //Dimension of a cell
        int cellWidth = 45;
        int cellHeight = 45;

        //How many mines to place on a game field
        static int minesCount = 10;

        //Tells how many flags you can place
        int flagsCount;

        BitmapImage mineBitmap;
        BitmapImage flagBitmap;

        Image[] mine = new Image[minesCount];
        Image[] flag = new Image[minesCount];

        int cells = dimension_x * dimension_y;

        Random random = new Random();

        //Array of button that will be each game field square
        Button[] square = new Button[dimension_x * dimension_y];

        //2D array that will be actuall game field representation (+2 on each side to avoid OutOfBoundsException)
        public string[,] field = new string[dimension_x + 2, dimension_y + 2];

        public MainWindow()
        {
            InitializeComponent();

            mineBitmap = new BitmapImage(new Uri("Images/mine.png", UriKind.Relative));
            flagBitmap = new BitmapImage(new Uri("Images/flag.png", UriKind.Relative));

            for(int i = 0; i<minesCount; i++)
            {
                mine[i] = new Image();
                mine[i].Source = mineBitmap;
                mine[i].Stretch = Stretch.Fill;

                flag[i] = new Image();
                flag[i].Source = flagBitmap;
                flag[i].Stretch = Stretch.Fill;
            }

            flagsCount = minesCount;
            Flags.Content = flagsCount;

            Height = cellHeight * dimension_y + 100;
            Width = cellWidth * dimension_x + 20;

            for (int y = 0; y < dimension_y + 2; y++)
            {
                for (int x = 0; x < dimension_x + 2; x++)
                {
                    field[x, y] = "X";
                }
            }

            for (int y = 1; y < dimension_y + 1; y++)
            {
                for (int x = 1; x < dimension_x + 1; x++)
                {
                    field[x, y] = "";
                }
            }

            //puts randomly mines
            for (int i = 0; i < minesCount; i++)
            {
                int x = random.Next(1, dimension_x + 1);
                int y = random.Next(1, dimension_y + 1);

                while (field[x, y] == "M")
                {
                    x = random.Next(1, dimension_x + 1);
                    y = random.Next(1, dimension_y + 1);
                }

                field[x, y] = "M";
            }

            for (int y = 1; y < dimension_y + 1; y++)
            {
                for (int x = 1; x < dimension_x + 1; x++)
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

            for(int i = 0; i<dimension_x; i++)
            {
                gameField.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(cellWidth) });
            }

            for(int i = 0; i< dimension_y; i++)
            {
                gameField.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellHeight) });
            }

            for(int y = 0; y<dimension_y; y++)
            {
                for(int x = 0; x<dimension_x; x++)
                {
                    square[x + dimension_y * y] = new Button();
                    square[x + dimension_y * y].Click += Square_Click;
                    square[x + dimension_y * y].MouseRightButtonDown += Put_Flag;
                    Grid.SetColumn(square[x + dimension_y * y], x);
                    Grid.SetRow(square[x + dimension_y * y], y);
                    gameField.Children.Add(square[x + dimension_y * y]);
                }
            }

            Grid.SetColumn(gameField, 0);
            Grid.SetRow(gameField, 1);
            GameWindow.Children.Add(gameField);
        }

        private void Put_Flag(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            int x = Grid.GetColumn(button);
            int y = Grid.GetRow(button);

            int flags = 0;

            if(flagsCount>0)
            {
                if((string)button.Content != "F")
                {
                    button.FontSize = 18;
                    button.FontWeight = FontWeights.Bold;
                    button.Content = "F";
                    flagsCount--;
                    cells--;
                }
                else
                {
                    button.Content = "";
                    flagsCount++;
                    cells++;
                }
                Flags.Content = flagsCount;
            }
            else if(flagsCount == 0 && (string)button.Content == "F")
            {
                button.Content = "";
                flagsCount++;
                cells++;
                Flags.Content = flagsCount;
            }

            if (cells == 0)
            {
                MessageBox.Show("You win!");
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
                cells--;

                if(cells==0)
                {
                    MessageBox.Show("You win!");
                }
            }
            else if(string.IsNullOrEmpty(field[x + 1, y + 1]))
            {
                button.IsEnabled = false;
                cells--;

                if (cells == 0)
                {
                    MessageBox.Show("You win!");
                }

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
                button.Background = Brushes.Red;
                int imagesCount = 0;
                //show all mines on the game field and game over
                for (int _y = 0; _y<dimension_y; _y++)
                {
                    for(int _x = 0; _x<dimension_x; _x++)
                    {
                        square[_x + dimension_y * _y].Click -= Square_Click;

                        if (field[_x + 1, _y + 1] == "M")
                        {
                            square[_x + dimension_y * _y].Content = mine[imagesCount];
                            imagesCount++;
                        }
                    }
                }

                MessageBox.Show("Game over!");
            }
        }
    }
}
