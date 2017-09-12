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

namespace MazeGenerator
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Maze maze;
        private void MazeField_Loaded(object sender, RoutedEventArgs e)
        {
            maze = new Maze(10, 10, (sender as Grid));
            maze.MakeMaze();
        }
        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            if (isSizeChanged)
            {
                isSizeChanged = false;
                maze.Reset();
            }
                
            maze.MakeMaze();
        }
        private void ButtonGene_Click(object sender, RoutedEventArgs e)
        {
            isSizeChanged = false;
            maze.Reset();
            maze.MakeMaze();
        }
        private void ButtinGeneRun_Click(object sender, RoutedEventArgs e)
        {
            isSizeChanged = false;
            maze.Reset();
            maze.Process();
            maze.MakeMaze();
        }
        private void ButtonShowAns_Click(object sender, RoutedEventArgs e)
        {
            if (!isSizeChanged)
            {
                maze.MakeMaze();
            }
        }
        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if (!isSizeChanged)
            {
                maze.StepProcess();
                maze.MakeMaze();
            }

        }
        private void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            if (!isSizeChanged)
            {
                maze.Process();
                maze.MakeMaze();
            }

        }
        bool isSizeChanged = false;
        private void SliderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (maze != null)
            {
                maze.SetSize((int)(sender as Slider).Value);
                isSizeChanged = true;
                LabelSize.Content = "MazeSize: " + (int)(sender as Slider).Value;
            }
            
        }
        private void MenuItemSaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (isSizeChanged)
            {
                isSizeChanged = false;
                maze.Reset();
            }
            maze.MakeMaze();
            var filePicker = new System.Windows.Forms.SaveFileDialog();
            filePicker.Filter = "PNG files|*.png|All Files|*.*";
            filePicker.Title = "Save diagram as PNG";
            if (filePicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveUsingEncoder(filePicker.FileName, MazeField, new PngBitmapEncoder());
                MazeField.UpdateLayout();
            }
            maze.MakeMaze();

        }
        private static void SaveUsingEncoder(string fileName, FrameworkElement UIElement, BitmapEncoder encoder)
        {
            int height = Math.Min((int)UIElement.ActualHeight, (int)UIElement.ActualWidth);
            int width = Math.Min((int)UIElement.ActualHeight, (int)UIElement.ActualWidth);

            // These two line of code make sure that you get completed visual bitmap.
            // In case your Framework Element is inside the scroll viewer then some part which is not
            // visible gets clip.  
            UIElement.Measure(new System.Windows.Size(width, height));
            UIElement.Arrange(new Rect(new System.Windows.Point(), new Point(width, height)));

            RenderTargetBitmap bitmap = new RenderTargetBitmap(width, height,
                                                                    96, // These decides the dpi factors 
                                                                    96,// The can be changed when we'll have preview options.
                                                                    PixelFormats.Pbgra32);
            bitmap.Render(UIElement);

            SaveUsingBitmapTargetRenderer(fileName, bitmap, encoder);
        }
        private static void SaveUsingBitmapTargetRenderer(string fileName, RenderTargetBitmap renderTargetBitmap, BitmapEncoder bitmapEncoder)
        {
            BitmapFrame frame = BitmapFrame.Create(renderTargetBitmap);
            bitmapEncoder.Frames.Add(frame);
            // Save file .
            using (var stream = File.Create(fileName))
            {
                bitmapEncoder.Save(stream);
            }
        }
    }
}
