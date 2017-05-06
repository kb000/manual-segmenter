using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Segmenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string sourcePath;
        private string filename;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (.jpeg)|*.jpeg|PNG Files (.png)|*.png|JPG Files (.jpg)|*.jpg|GIF Files (.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                filename = dlg.FileName;
                textBox1.Text = filename;
                mainImage.Source = new BitmapImage(new Uri(filename));
                //imageScrollViewer.MaxHeight = mainImage.Source.Height;
                //imageScrollViewer.MaxWidth = mainImage.Source.Width;
                imageScrollViewer.UpdateLayout();
            }
        }

        private Point? p, q;

        private void mainImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            maybeWritePoint();

            if (p == null)
            {
                p = e.GetPosition(mainImage);
                p = new Point(Math.Round(p?.X ?? 0), Math.Round(p?.Y ?? 0));
                textBox1.Text = $"({p?.X}, {p?.Y})";
            }
            else
            {
                q = e.GetPosition(mainImage);
                q = new Point(Math.Round(q?.X ?? 0), Math.Round(q?.Y ?? 0));
                textBox2.Text = $"({q?.X}, {q?.Y})";
                updateCanvas(p, q);
            }
        }

        private Rectangle rect;

        private void cancelRect()
        {
            p = null;
            textBox1.Text = "-";
            q = null;
            textBox2.Text = "-";
            if (rect != null)
            {
                mainCanvas.Children.Remove(rect);
                rect = null;
            }
        }

        private void updateCanvas(Point? p1, Point? p2)
        {
            if (p1 != null && p2 != null)
            {
                var pt1 = p1.Value;
                var pt2 = p2.Value;
                rect = new System.Windows.Shapes.Rectangle();
                rect.Stroke = new SolidColorBrush(Colors.RoyalBlue);
                rect.Fill = null;
                rect.Width = Math.Abs(pt2.X - pt1.X);
                rect.Height = Math.Abs(pt2.Y - pt1.Y);
                Canvas.SetLeft(rect, min(pt1.X, pt2.X));
                Canvas.SetTop(rect, min(pt1.Y, pt2.Y));
                mainCanvas.Children.Add(rect);
            }
        }

        private static double min(double x1, double x2)
        {
            return x1 < x2 ? x1 : x2;
        }

        private void maybeWritePoint()
        {
            if (p != null && q != null)
            {
                rect.Stroke = new SolidColorBrush(Colors.Black);
                writeQuadToCsv(p.Value, q.Value);
                p = null;
                q = null;
                rect = null;
                textBox1.Text = "";
                textBox2.Text = "";
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                cancelRect();
            }
        }

        private void writeQuadToCsv(Point p, Point q)
        {
            using (var writer = File.AppendText($"{filename}.csv"))
            {
                writer.WriteLine($"{p.X}, {p.Y}, {q.X}, {q.Y}");
            }
        }
    }
}
