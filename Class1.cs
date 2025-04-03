using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
                Background = new SolidColorBrush(Colors.Transparent),
            };

            grid.Children.Add(txtOutput);
            Grid.SetColumn(txtOutput, 0);
            Grid.SetRow(txtOutput, 1);
            Grid.SetRowSpan(txtOutput, 5);

        }

        private void buttonFly_Click(object e, EventArgs a)
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
            txtOutput.Text = outputContent;
        }
    }
}
