using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RayTracer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly static Render render = new Render();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = render;
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            render.Render_image = RenderImage;
            //render.Start(); // temporary
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            render.Start(6);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (render.Result != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PNG file (*.png)|*.png",
                    FileName = "render"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    render.Result.Save(saveFileDialog.FileName, ImageFormat.Png);
                }
            }
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
