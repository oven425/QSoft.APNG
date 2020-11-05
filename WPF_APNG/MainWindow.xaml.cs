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
            CPNG_Reader pngr = new CPNG_Reader();
            pngr.Open(sri.Stream);

            IHDR ihdr = pngr.Chunks.FirstOrDefault(x => x.ChunkType == ChunkTypes.IHDR) as IHDR;

            MemoryStream mm = new MemoryStream();
            fcTL lastfctl = null;
            for (int i = 0; i < pngr.Chunks.Count; i++)
            {
                switch (pngr.Chunks[i].ChunkType)
                {
                    case ChunkTypes.IDAT:
                        {
                            IDAT idat = pngr.Chunks[i] as IDAT;
                            mm.Write(idat.Data, 0, idat.Data.Length);
                        }
                        break;
                    case ChunkTypes.fcTL:
                        {
                            fcTL fctl = pngr.Chunks[i] as fcTL;
                            if (mm.Length > 0)
                            {
                                //Stream fs_ = File.Create($"{i}.png");
                                //CPNG_Writer pngw_ = new CPNG_Writer();
                                //pngw_.Open(fs_);
                                //pngw_.WriteIHDR(ihdr);

                                //pngw_.WriteIDAT(mm.ToArray());
                                //pngw_.WriteIEND();
                                //fs_.Close();
                                //fs_.Dispose();


                                MemoryStream ms = new MemoryStream();
                                CPNG_Writer pngw_ = new CPNG_Writer();
                                pngw_.Open(ms);
                                pngw_.WriteIHDR(ihdr);
                                pngw_.WriteIDAT(mm.ToArray());
                                pngw_.WriteIEND();
                                ms.Position = 0;
                                this.m_Apng.Add(lastfctl, ms);
                            }
                            lastfctl = fctl;
                            mm.SetLength(0);
                        }
                        break;
                    case ChunkTypes.fdAT:
                        {
                            fdAT fdat = pngr.Chunks[i] as fdAT;
                            mm.Write(fdat.Data, 0, fdat.Data.Length);
                        }
                        break;
                    case ChunkTypes.IEND:
                        {
                            //FileStream fs_ = File.Create($"{i}.png");
                            //CPNG_Writer pngw_ = new CPNG_Writer();
                            //pngw_.Open(fs_);
                            //pngw_.WriteIHDR(ihdr);

                            //pngw_.WriteIDAT(mm.ToArray());
                            //pngw_.WriteIEND();
                            //fs_.Close();
                            //fs_.Dispose();


                            MemoryStream ms = new MemoryStream();
                            CPNG_Writer pngw_ = new CPNG_Writer();
                            pngw_.Open(ms);
                            pngw_.WriteIHDR(ihdr);
                            pngw_.WriteIDAT(mm.ToArray());
                            pngw_.WriteIEND();
                            ms.Position = 0;
                            this.m_Apng.Add(lastfctl, ms);
                            mm.SetLength(0);

                            
                        }
                        break;
                }
            }

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += Timer_Tick;
            timer.Start();
            //DrawingGroup imageDrawings = new DrawingGroup();
            //// Create a 100 by 100 image with an upper-left point of (75,75).
            //ImageDrawing bigKiwi = new ImageDrawing();
            //bigKiwi.Rect = new Rect(0, 0, 843, 486);
            //bigKiwi.ImageSource = new BitmapImage(
            //    new Uri(@"C:\Users\ben_hsu\Pictures\1.png"));

            //imageDrawings.Children.Add(bigKiwi);

            //// Create a 25 by 25 image with an upper-left point of (0,150).
            //ImageDrawing smallKiwi1 = new ImageDrawing();
            //smallKiwi1.Rect = new Rect(0, 0, 843, 486);
            //smallKiwi1.ImageSource = new BitmapImage(new Uri(@"C:\Users\ben_hsu\Pictures\2.png"));
            //imageDrawings.Children.Add(smallKiwi1);

            //// Create a 25 by 25 image with an upper-left point of (150,0).
            //ImageDrawing smallKiwi2 = new ImageDrawing();
            //smallKiwi2.Rect = new Rect(0, 0, 843, 486);
            //smallKiwi2.ImageSource = new BitmapImage(new Uri(@"C:\Users\ben_hsu\Pictures\3.png"));
            //imageDrawings.Children.Add(smallKiwi2);

            //// Create a 75 by 75 image with an upper-left point of (0,0).
            //ImageDrawing wholeKiwi = new ImageDrawing();
            //wholeKiwi.Rect = new Rect(0, 0, 843, 486);
            //wholeKiwi.ImageSource = new BitmapImage(new Uri(@"C:\Users\ben_hsu\Pictures\4.png"));
            //imageDrawings.Children.Add(wholeKiwi);

            ////
            //// Use a DrawingImage and an Image control to
            //// display the drawings.
            ////
            //DrawingImage drawingImageSource = new DrawingImage(imageDrawings);
            //this.img.Source = drawingImageSource;


            //FileStream fs = File.Create("test.png");
            //CPNG_Writer pngw = new CPNG_Writer();
            //pngw.Open(fs);
            //pngw.WriteIHDR(pngr.Chunks.FirstOrDefault(x => x.ChunkType == ChunkTypes.IHDR) as IHDR);

            //pngw.WriteIDAT((pngr.Chunks.FirstOrDefault(x => x.ChunkType == ChunkTypes.IDAT) as IDAT).Data);
            //pngw.WriteIEND();
            //fs.Close();
            //fs.Dispose();
            //FileStream fs1 = File.OpenRead(@"test.png");
            //CPNG_Reader pngr1 = new CPNG_Reader();
            //pngr1.Open(fs1);
        }

        int index = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            Stream stream = this.m_Apng.ElementAt(index).Value;
            stream.Position = 0;
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = stream;
            bmp.EndInit();
            this.img.Source = bmp;
            index = index + 1;
            if (index >= this.m_Apng.Count)
            {
                index = 0;
            }
        }
    }

    class CRC32Cls
    {
        protected ulong[] Crc32Table;
        //生成CRC32码表
        public void GetCRC32Table()
        {
            ulong Crc;
            Crc32Table = new ulong[256];
            int i, j;
            for (i = 0; i < 256; i++)
            {
                Crc = (ulong)i;
                for (j = 8; j > 0; j--)
                {
                    if ((Crc & 1) == 1)
                        Crc = (Crc >> 1) ^ 0xEDB88320;
                    else
                        Crc >>= 1;
                }
                Crc32Table[i] = Crc;
            }
        }

        public ulong GetCRC32Str(string sInputString)
        {
            GetCRC32Table();
            byte[] buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(sInputString);
            ulong value = 0xffffffff;
            int len = buffer.Length;
            for (int i = 0; i < len; i++)
            {
                value = (value >> 8) ^ Crc32Table[(value & 0xFF) ^ buffer[i]];
            }
            return value ^ 0xffffffff;
        }

        public ulong GetCRC32Str(byte[] data)
        {
            GetCRC32Table();
            byte[] buffer = data;
            ulong value = 0xffffffff;
            int len = buffer.Length;
            for (int i = 0; i < len; i++)
            {
                value = (value >> 8) ^ Crc32Table[(value & 0xFF) ^ buffer[i]];
            }
            return value ^ 0xffffffff;
        }

    }

}
