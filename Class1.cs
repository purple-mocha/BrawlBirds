using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace birds
{
    public class SpanTheCells : Window
    {
        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            app.Run(new SpanTheCells());
        }

        private List<TextBox> textBoxes = new List<TextBox>();
        private string[] astrLabel = { "_Скорость:",  "_Угол наклона:",
                "_Масса:",
                "_Коэфициент сопротивления воздуха:" };
        private TextBox txtOutput;
        private Canvas canv;
        public SpanTheCells()
        {
            Title = "Bird game";
            Grid grid = new Grid();
            Content = grid;
            grid.Margin = new Thickness(5);
            for (int i = 0; i <= 3; i++)
            {
                ColumnDefinition coldef = new ColumnDefinition();
                if (i == 1)
                {
                    coldef.Width = new GridLength(100, GridUnitType.Star);
                }
                else
                {
                    coldef.Width = new GridLength(100, GridUnitType.Auto);
                }
                grid.ColumnDefinitions.Add(coldef);
            }


            for (int i = 0; i <= 6; i++)
            {
                RowDefinition rowdef = new RowDefinition();
                rowdef.Height = GridLength.Auto;
                grid.RowDefinitions.Add(rowdef);
                if (i == 5 || i == 0)
                {
                    rowdef.Height = new GridLength(100, GridUnitType.Star);
                }
            }

            for (int i = 0; i < astrLabel.Length; i++)
            {

                Label lbl = new Label
                {
                    Content = astrLabel[i],
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.White),
                    Background = new SolidColorBrush(Color.FromScRgb(0.5f, 0, 0, 0)), 
                };

                grid.Children.Add(lbl);
                Grid.SetRow(lbl, i + 1);
                Grid.SetColumn(lbl, 1);

                TextBox txtbox = new TextBox 
                {
                    BorderThickness = new Thickness(2),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(5),
                    VerticalContentAlignment = VerticalAlignment.Center 
                };
                grid.Children.Add(txtbox);
                Grid.SetRow(txtbox, i + 1);
                Grid.SetColumn(txtbox, 2);
                Grid.SetColumnSpan(txtbox, 3);
                textBoxes.Add(txtbox);

            }

            ImageBrush image = new ImageBrush();
            string stringi = "https://avatars.mds.yandex.net/i?id=2c2fcf51440f35283ad5ebca369b8fde0ec5cb1e-5889206-images-thumbs&n=13";
            Uri uri = new Uri(stringi);
            BitmapImage bitmap = new BitmapImage(uri);
            image.ImageSource = bitmap;

            Image bs = new Image();
            uri = new Uri("https://i.imgur.com/YJbL26E.png");
            bitmap = new BitmapImage(uri);
            bs.Source = bitmap;
            Button btn = new Button
            {
                Content = bs,
                Margin = new Thickness(5),
                IsDefault = true

            };
            btn.Click += buttonFly_Click;

            grid.Children.Add(btn);
            Grid.SetRow(btn, 6);
            Grid.SetColumn(btn, 3);

            grid.Children[1].Focus();
            grid.Background = image;


            txtOutput = new TextBox
            {
                Margin = new Thickness(5),
                IsReadOnly = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                TextWrapping = TextWrapping.Wrap,
                Background = new SolidColorBrush(Colors.Transparent)
            };
            
            grid.Children.Add(txtOutput);
            Grid.SetColumn(txtOutput, 0);
            Grid.SetRow(txtOutput, 1);
            Grid.SetRowSpan(txtOutput, 5);

            Button readFile = new Button
            {
                Content = "Файл",
                Margin = new Thickness(5),
                IsDefault = true,
                Height = 50,
                VerticalAlignment = VerticalAlignment.Bottom,
                Background = new SolidColorBrush(Color.FromScRgb(0.5f, 0, 0, 0)),
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("Arial"),
                FontWeight = FontWeights.Bold,
                FontSize = 20
            };
            readFile.Click += readFile_Click;
            grid.Children.Add(readFile);
            Grid.SetRow(readFile, 5);
            Grid.SetColumn(readFile, 3);


        }

        private void fillBoxes(string inputFile)
        {
            string[] lines = File.ReadAllLines(inputFile);
            for (int i = 0; i < lines.Length; i++)
            {
                textBoxes[i].Text = lines[i];
            }
        }

        private void readFile_Click(object e, EventArgs a)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if ((bool)dialog.ShowDialog(this))
            {
                string inputFile = dialog.FileName;
                fillBoxes(inputFile);
                buttonFly_Click(e, a);
            }
        }

        async private void buttonFly_Click(object e, EventArgs a)
        {

            ProjectileMotion projectile = new ProjectileMotion();
            string inputfile = "dannie.txt";
            string outputfile = "otvet.txt";
            
            using (StreamWriter writer = new StreamWriter(inputfile))
            {
                foreach (var txtBox in textBoxes)
                {
                    writer.WriteLine(txtBox.Text);
                }
            }
            projectile.ReadInputData(inputfile);
            projectile.CalculateTrajectory(outputfile);
            string outputContent = File.ReadAllText(outputfile);
            Window window = new Window();
            window.Owner = this;
            ImageBrush image = new ImageBrush();
            string stringi = "https://i.pinimg.com/originals/cb/20/0c/cb200cb1486977f8efec0172d8f035db.jpg";
            Uri uri = new Uri(stringi);
            BitmapImage bitmap = new BitmapImage(uri);
            image.ImageSource = bitmap;
            canv = new Canvas
            {
                Margin = new Thickness(5),
                Background = image
            };
           
            window.Content = canv;
            txtOutput.Text = outputContent;
            window.Show();
            DrawLines(canv, projectile.getXes(), projectile.getYes(), window);
            await Task.Delay(1000);

        }
        async void DrawLines(Canvas canv, List<double> X, List<double> Y, Window window)
        {
            canv.Children.Clear();

            double maxX = X.Max();
            double maxY = Y.Max();

            double k_x = (canv.ActualWidth) / (maxX);
            double k_y = (canv.ActualHeight) / (maxY);
            double k = Math.Min(k_x, k_y);
            int radius = 8;
            Point ptCenter = new Point(0, 0);
            Ellipse elips = new Ellipse
            {
                Stroke = Brushes.Red,
                StrokeThickness = 3,
                Width = radius*2,
                Height = radius*2
            };
            canv.Children.Add(elips);
            Canvas.SetLeft(elips, 0);
            Canvas.SetTop(elips, 0);
            for (int i = 0; i < X.Count - 1 & Y[i + 1] >= 0; i++)
            {
                Line line = new Line
                {
                    X1 = X[i] * k,
                    Y1 = canv.ActualHeight - Y[i] * k,
                    X2 = X[i + 1] * k,
                    Y2 = canv.ActualHeight - Y[i + 1] * k,
                    Stroke = Brushes.Black,
                    StrokeThickness = 4
                };
                Canvas.SetLeft(elips, X[i + 1] * k - radius);
                Canvas.SetTop(elips, canv.ActualHeight - Y[i + 1] * k - radius);
                canv.Children.Add(line);
                await Task.Delay(1);
            }
            await Task.Delay(1000);
            window.Close();
        }
    }
}
