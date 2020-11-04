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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StreamResourceInfo sri = Application.GetResourceStream(new Uri("pack://application:,,,/apng_spinfox.png", UriKind.Absolute));
            //BinaryReader br = new BinaryReader(sri.Stream);
            //this.ParsePNG(br);
            CPNG_Reader pngr = new CPNG_Reader();
            pngr.Open(sri.Stream);

            IHDR ihdr = pngr.Chunks.FirstOrDefault(x => x.ChunkType == ChunkTypes.IHDR) as IHDR;
            for(int i=0; i<pngr.Chunks.Count; i++)
            {
                switch(pngr.Chunks[i].ChunkType)
                {
                    case ChunkTypes.fcTL:
                        {

                        }
                        break;
                    case ChunkTypes.fdAT:
                        {
                            fdAT fdat = pngr.Chunks[i] as fdAT;
                            FileStream fs_ = File.Create($"{fdat.SequenceNumber}test.png");
                            CPNG_Writer pngw_ = new CPNG_Writer();
                            pngw_.Open(fs_);
                            pngw_.WriteIHDR(ihdr);

                            pngw_.WriteIDAT(fdat.Data);
                            pngw_.WriteIEND();
                            fs_.Close();
                            fs_.Dispose();
                        }
                        break;
                }
            }


            FileStream fs = File.Create("test.png");
            CPNG_Writer pngw = new CPNG_Writer();
            pngw.Open(fs);
            pngw.WriteIHDR(pngr.Chunks.FirstOrDefault(x => x.ChunkType == ChunkTypes.IHDR) as IHDR);
            
            pngw.WriteIDAT((pngr.Chunks.FirstOrDefault(x => x.ChunkType == ChunkTypes.IDAT) as IDAT).Data);
            pngw.WriteIEND();
            fs.Close();
            fs.Dispose();
            FileStream fs1 = File.OpenRead(@"test.png");
            CPNG_Reader pngr1 = new CPNG_Reader();
            pngr1.Open(fs1);
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
