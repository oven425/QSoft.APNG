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
using System.IO;
using System.Windows.Resources;
using APNG;
using System.Windows.Threading;
using System.Windows.Media.Composition;
using System.Windows.Interop;

namespace WPF_APNG
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
        Dictionary<fcTL, MemoryStream> m_Apng = new Dictionary<fcTL, MemoryStream>();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StreamResourceInfo sri = Application.GetResourceStream(new Uri("pack://application:,,,/apng_spinfox.png", UriKind.Absolute));
            CPng_Reader pngr = new CPng_Reader();
            this.m_Apng = pngr.Open(sri.Stream).SpltAPng();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        int index = 0;
        Dictionary<int, BitmapImage> m_Bmps = new Dictionary<int, BitmapImage>();
        private void Timer_Tick(object sender, EventArgs e)
        {
            Stream stream = this.m_Apng.ElementAt(index).Value;
            if(this.m_Bmps.ContainsKey(index) == false)
            {
                stream.Position = 0;
                BitmapImage bmp = new BitmapImage();
                
                bmp.BeginInit();
                bmp.StreamSource = stream;
                bmp.EndInit();
                this.m_Bmps.Add(index, bmp);
                //bmp.Freeze();
            }
            
            this.img.Source = this.m_Bmps[index];
            index = index + 1;
            if (index >= this.m_Apng.Count)
            {
                index = 0;
            }
        }
    }

    public class AA : BitmapSource
    {
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
