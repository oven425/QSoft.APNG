#define PngBuild
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Resources;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Compression;
using QSoft.Apng;
using QSoft.Apng.WPF;
using System.Windows.Media.Composition;
using QSoft.Apng.Build;

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
        Dictionary<fcTL, byte[]> m_Raws = new Dictionary<fcTL, byte[]>();

        public WriteableBitmap Clone(BitmapSource bitmap)
        {
            WriteableBitmap wb = new WriteableBitmap(bitmap.PixelWidth,bitmap.PixelHeight, bitmap.DpiX, bitmap.DpiY, bitmap.Format, bitmap.Palette);
            var rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            var bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            var bytes = new byte[bytesPerPixel* bitmap.PixelWidth* bitmap.PixelHeight];
            bitmap.CopyPixels(rect, bytes, bytesPerPixel* bitmap.PixelWidth, 0);
            wb.WritePixels(rect, bytes, bytesPerPixel * bitmap.PixelWidth, 0);
            return wb;
        }

        public Color GetPixelColor(BitmapSource bitmap, int x, int y)
        {
            Color color;
            var bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            var bytes = new byte[bytesPerPixel];
            var rect = new Int32Rect(x, y, 1, 1);

            bitmap.CopyPixels(rect, bytes, bytesPerPixel, 0);

            if (bitmap.Format == PixelFormats.Bgra32)
            {
                color = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
            else if (bitmap.Format == PixelFormats.Bgr32)
            {
                color = Color.FromRgb(bytes[2], bytes[1], bytes[0]);
            }
            // handle other required formats
            else
            {
                color = Colors.Black;
            }

            return color;
        }

        MainUI m_MainUI = null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.m_MainUI == null)
            {
                BitmapSource bmp = image.Source as BitmapSource;
                
                if(bmp.Format != PixelFormats.Bgr32)
                {
                    FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
                    newFormatedBitmapSource.BeginInit();
                    newFormatedBitmapSource.Source = bmp;
                    newFormatedBitmapSource.DestinationFormat = PixelFormats.Bgra32;
                    newFormatedBitmapSource.EndInit();
                    bmp = newFormatedBitmapSource;
                }
                
                GetPixelColor(bmp, 251, 44);
                var wb = this.Clone(bmp);
                this.image1.Source = wb;
                DirectoryInfo dir = new DirectoryInfo("../../testapng");
                var files =  dir.GetFiles();
                this.m_MainUI = new MainUI();
                foreach(var oo in files)
                {
                    this.m_MainUI.Files.Add(oo);
                }
                this.DataContext = this.m_MainUI;
            }
            //Uri uri = new Uri("pack://application:,,,/WPF_APNG;component/elephant.png");

            ////File.OpenRead("pack://application:,,,/WPF_APNG;component/elephant.png");
            ////var apngs = Directory.GetFiles("../../testapng");
            ////var file = File.OpenRead("../../testapng/elephant.png");
            //var file = File.OpenRead("../../testapng/SDve91m.png");
            ////file.SplitApng();
            ////var file = File.OpenRead("../../testapng/pyani.png");
            //Png_Reader pngr = new Png_Reader();
            //var storyboard = pngr.Open(file).ToWPF(this.image_png);
            //storyboard.CurrentTimeInvalidated += Storyboard_CurrentTimeInvalidated;
            ////var pngs = pngr.Open(file).SpltAPng();
            ////for (int i = 0; i < pngs.Count; i++)
            ////{
            ////    File.WriteAllBytes($"{i}.png", pngs.ElementAt(i).Value.ToArray());
            ////}
            //file.Close();
            //file.Dispose();

            //var file = File.OpenRead("test.png");
            //var file = File.OpenRead("2.png");
            var file = File.OpenRead("../../testapng/DiteNUU.png");
            var pngs = file.SplitApng();
            //for (int i = 0; i < pngs.Count; i++)
            //{
            //    File.WriteAllBytes($"{i}.png", pngs.ElementAt(i).Value.ToArray());
            //}
            file.Close();
            file.Dispose();

            //var file = File.OpenRead("0.png");
            //Png_Reader pngr = new Png_Reader();
            //pngr.Open(file);
            //var buf = pngr.iDAT();
            //var pngs = file.SplitApng();
            //for (int i = 0; i < pngs.Count; i++)
            //{
            //    File.WriteAllBytes($"{i}.png", pngs.ElementAt(i).Value.ToArray());
            //}

            //var pngs_1 = Directory.GetFiles(".", "*.png").OrderBy(x => int.Parse(x.Replace(".\\", "").Replace(".png", "")));
            
            //ApngBuilder apngbuild = new ApngBuilder();
            
            //var apngstream = apngbuild.Build(pngs_1, TimeSpan.FromSeconds(10));
            //apngstream.Position = 0;
            //apngstream.CopyTo(File.Create("test.png"));
        }

        private void Storyboard_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }

}
