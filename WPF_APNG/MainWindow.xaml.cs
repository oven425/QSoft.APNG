#define TestBuild
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
            //var bytes = new byte[bytesPerPixel* bitmap.PixelWidth* bitmap.PixelHeight];
            //bitmap.CopyPixels(rect, bytes, bytesPerPixel* bitmap.PixelWidth, 0);
            var bytes = Copy(bitmap);
            wb.WritePixels(rect, bytes, bytesPerPixel * bitmap.PixelWidth, 0);
            return wb;
        }

        public WriteableBitmap Clone(BitmapSource bitmap, byte[] data)
        {
            WriteableBitmap wb = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX, bitmap.DpiY, bitmap.Format, bitmap.Palette);
            var rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            var bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            wb.WritePixels(rect, data, bytesPerPixel * bitmap.PixelWidth, 0);
            return wb;
        }

        public byte[] Copy(BitmapSource bitmap)
        {
            var rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            var bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            var bytes = new byte[bytesPerPixel * bitmap.PixelWidth * bitmap.PixelHeight];
            bitmap.CopyPixels(bytes, bytesPerPixel * bitmap.PixelWidth, 0);
            return bytes;
        }

        public List<Tuple<Point, Color>> CopyToColor(BitmapSource bitmap)
        {
            List<Tuple<Point, Color>> colors = new List<Tuple<Point, Color>>();
            var buf = this.Copy(bitmap);
            for(int i=0; i< buf.Length; i=i+4)
            {
                int x = i % bitmap.PixelWidth;
                int y = i / bitmap.PixelWidth/4;
                Point pt = new Point(x, y);
                Color color = Color.FromArgb(buf[i+3], buf[i+2], buf[i+1], buf[i+0]);
                colors.Add(Tuple.Create<Point,Color>(pt,color));
            }
            return colors;
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
            else
            {
                color = Colors.Black;
            }

            return color;
        }

        public void Clip1(BitmapSource bitmap, Int32Rect clip)
        {
            byte[] src = this.Copy(bitmap);
            int w = bitmap.PixelWidth;
            int h = bitmap.PixelHeight;
            int begin = (int)(clip.X * 4 * clip.Y);
            int end = (int)((clip.X+clip.Width) * 4 * (clip.Y+clip.Height));
            byte[] dst = new byte[end - begin];
            byte[] dst1 = new byte[end - begin];
            Array.Copy(src, begin, dst1, 0, dst.Length);
            
            int stride = bitmap.Format.BitsPerPixel * (int)clip.Width / 8;
            bitmap.CopyPixels(clip, dst, stride, 0);
            for (int i = 0; i < dst.Length; i++)
            {
                if (src[i] != dst[i])
                {
                    System.Diagnostics.Trace.WriteLine($"dst:{dst[i]} dst1:{dst1[i]} {i}");
                    //break;
                }
            }
            //Array.Clear(dst, 0, dst.Length);
            //for(int i=0; i< dst.Length/2; i=i+4)
            //{
            //    dst[i] = 255;
            //    dst[i + 1] = 255;
            //    dst[i + 2] = 255;
            //    dst[i + 3] = 255;
            //}

            BitmapSource bmp = BitmapSource.Create((int)clip.Width, (int)clip.Height, 96, 96, bitmap.Format, null, dst, stride);
            var frame = BitmapFrame.Create(bmp, null, null, null);


            //BitmapEncoder bmpe = new BmpBitmapEncoder();
            //bmpe.Frames.Add(frame);
            //bmpe.Save(File.Create("AA.bmp"));

            PngBitmapEncoder pnge = new PngBitmapEncoder();
            pnge.Frames.Add(frame);
            pnge.Save(File.Create("AA.png"));

        }

        MainUI m_MainUI = null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.m_MainUI == null)
            {
                BitmapSource bmp_1 = image_1.Source as BitmapSource;
                BitmapSource bmp_2 = image_2.Source as BitmapSource;

                this.Clip1(bmp_1, new Int32Rect(0, 0, 74, 74));

                //if (bmp_1.Format != PixelFormats.Bgr32)
                //{
                //    FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
                //    newFormatedBitmapSource.BeginInit();
                //    newFormatedBitmapSource.Source = bmp_1;
                //    newFormatedBitmapSource.DestinationFormat = PixelFormats.Bgra32;
                //    newFormatedBitmapSource.EndInit();
                //    bmp_1 = newFormatedBitmapSource;
                //}
                var b1 = this.CopyToColor(bmp_1);
                var b2 = this.CopyToColor(bmp_2);
                List<byte> b3 = new List<byte>();
                //var b3 = new byte[this.Copy(bmp_1).Length];
                Point begin = new Point(double.MaxValue, double.MaxValue);
                Point end = new Point(double.MinValue, double.MinValue);
                
                for (int i=0; i<b1.Count; i++)
                {
                    if (b2[i].Item2 != b1[i].Item2)
                    {
                        if(begin.X > b2[i].Item1.X)
                        {
                            begin.X = b2[i].Item1.X;
                        }
                        if (begin.Y > b2[i].Item1.Y)
                        {
                            begin.Y = b2[i].Item1.Y;
                        }
                        if (end.X < b2[i].Item1.X)
                        {
                            end.X = b2[i].Item1.X;
                        }
                        if (end.Y < b2[i].Item1.Y)
                        {
                            end.Y = b2[i].Item1.Y;
                        }
                        b3.Add(b2[i].Item2.B);
                        b3.Add(b2[i].Item2.G);
                        b3.Add(b2[i].Item2.R);
                        b3.Add(b2[i].Item2.A);
                        //b3[i * 4 + 0] = b2[i].Item2.B;
                        //b3[i * 4 + 1] = b2[i].Item2.G;
                        //b3[i * 4 + 2] = b2[i].Item2.R;
                        //b3[i * 4 + 3] = b2[i].Item2.A;
                    }
                }
                var bytesPerPixel = (bmp_1.Format.BitsPerPixel + 7) / 8;
                
                int w = (int)(end.X - begin.X);
                int h = (int)(end.Y - begin.Y);
                int pos_begin = (int)(begin.X + begin.Y * bmp_1.PixelWidth*4);
                int pos_end = (int)(end.X + end.Y * bmp_1.PixelWidth*4);
                
                var stride = bytesPerPixel * w;
                var b4 = new byte[pos_end - pos_begin];
                Array.Copy(this.Copy(bmp_2), pos_begin, b4, 0, stride*h);
                BitmapSource bmp = BitmapSource.Create(w, h, 96, 96, bmp_1.Format, null, b4, stride);
                PngBitmapEncoder pnge = new PngBitmapEncoder();
                var frame = BitmapFrame.Create(bmp, null, null, null);
                pnge.Frames.Add(frame);
                pnge.Save(File.Create("AA.png"));
                //BitmapSource bmp = BitmapSource.Create(bmp_1.PixelWidth, bmp_1.PixelHeight, 96, 96, bmp_1.Format, null, b3.ToArray(), stride);
                //var wb = this.Clone(bmp_1, b3.ToArray());


                //var wb = this.Clone(bmp_2);
                //this.image1.Source = wb;
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
#if TestBuild
            //var pngs_1 = Directory.GetFiles(".", "*.png");
            ////var pngs_1 = Directory.GetFiles(".", "*.png").OrderBy(x => int.Parse(x.Replace(".\\", "").Replace(".png", "")));

            //ApngBuilder apngbuild = new ApngBuilder();

            //var apngstream = apngbuild.Build(pngs_1, TimeSpan.FromSeconds(10));
            //apngstream.Position = 0;
            //apngstream.CopyTo(File.Create("test.png"));
#endif
        }

        private void Storyboard_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }

}
