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
using System.Windows.Controls.Primitives;
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
            SettingsPopup.IsOpen = false;
            string Scene, Quality = "", Width = "";

            Scene = ((ComboBoxItem)SceneSelect.Items[SceneSelect.SelectedIndex]).Content.ToString();

            foreach (var child in QualityRadio.Children)
            {
                if (child is RadioButton radioButton && radioButton.IsChecked == true)
                {
                    Quality = radioButton.Content.ToString();
                    break;
                }
            }
            foreach (var child in ImageWidth.Children)
            {
                if (child is RadioButton radioButton && radioButton.IsChecked == true)
                {
                    Width = radioButton.Content.ToString();
                    break;
                }
            }
            render.Start(Scene, Quality, Width);
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

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopup.IsOpen = !SettingsPopup.IsOpen;
        }
    }
}
