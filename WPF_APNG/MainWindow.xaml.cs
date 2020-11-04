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

        void ParseIHDR(BinaryReader br, IHDR data)
        {
            data.Width = br.ReadInt32LN();
            data.Height = br.ReadInt32LN();
            data.BitDepth = br.ReadByte();
            data.ColorType = br.ReadByte();
            data.Compression = br.ReadByte();
            data.Filter = br.ReadByte();
            data.Iterlace = br.ReadByte();
            byte[] crc = br.ReadBytes(4);
            System.Diagnostics.Trace.WriteLine($"IHDR crc:{BitConverter.ToString(crc)}");
        }


        byte[] m_PNGHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IHDR ihdr = new IHDR();
            StreamResourceInfo sri = Application.GetResourceStream(new Uri("pack://application:,,,/apng_spinfox.png", UriKind.Absolute));
            //BinaryReader br = new BinaryReader(sri.Stream);
            //this.ParsePNG(br);
            CPNG_Reader pngr = new CPNG_Reader();
            pngr.Open(sri.Stream);
            


            FileStream fs = File.Create("test.png");
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(this.m_PNGHeader);
            this.WriteIHDR(bw, pngr.Chunks.FirstOrDefault(x=>x.ChunkType== ChunkTypes.IHDR) as IHDR);
            this.WriteIDAT(bw, (pngr.Chunks.FirstOrDefault(x => x.ChunkType == ChunkTypes.IDAT) as IDAT).Data);
            this.WriteIEND(bw);
            fs.Close();
            fs.Dispose();
            FileStream fs1 = File.OpenRead(@"test.png");
            CPNG_Reader pngr1 = new CPNG_Reader();
            pngr1.Open(fs1);
            //br = new BinaryReader(fs1);
            //idat = null;
            //this.ParsePNG(br);


        }

        IHDR m_IHDR = new IHDR();
        void ParsePNG( BinaryReader br)
        {
            byte[] sss = br.ReadBytes(8);
            System.Diagnostics.Trace.WriteLine(BitConverter.ToString(sss));
            while (true)
            {
                int len = br.ReadInt32LN();
                sss = br.ReadBytes(4);
                string id = Encoding.UTF8.GetString(sss);

                switch (id)
                {
                    case "IHDR":
                        {
                            ParseIHDR(br, this.m_IHDR);
                            System.Diagnostics.Trace.WriteLine($"IHDR width:{this.m_IHDR.Width} height:{this.m_IHDR.Height}");
                        }
                        break;
                    case "acTL":
                        {
                            int num_frames = br.ReadInt32LN();
                            int num_plays = br.ReadInt32LN();
                            br.ReadBytes(4);
                            System.Diagnostics.Trace.WriteLine($"acTL num_frames:{num_frames} num_plays:{num_plays}");
                        }
                        break;
                    case "fcTL":
                        {
                            int sequence_number = br.ReadInt32LN();
                            int width = br.ReadInt32LN();
                            int height = br.ReadInt32LN();
                            int x_offset = br.ReadInt32LN();
                            int y_offset = br.ReadInt32LN();
                            int delay_num = br.ReadInt16LN();
                            int delay_den = br.ReadInt16LN();
                            byte dispose_op = br.ReadByte();
                            byte blend_op = br.ReadByte();
                            br.ReadBytes(4);
                            //System.Diagnostics.Trace.WriteLine($"fcTL sequence_number:{sequence_number} width:{width} height:{height} delay_num:{delay_num} delay_den:{delay_den}");
                        }
                        break;
                    case "fdAT":
                        {
                            int sequence_number = br.ReadInt32LN();
                            br.ReadBytes(len - 4);
                            br.ReadBytes(4);
                            //System.Diagnostics.Trace.WriteLine($"fdAT len:{len} sequence_number:{sequence_number}");
                        }
                        break;
                    case "IDAT":
                        {
                            if(idat == null)
                            {
                                idat = br.ReadBytes(len);

                                byte[] crc = br.ReadBytes(4);
                                System.Diagnostics.Trace.WriteLine($"IDAT len:{len} crc:{BitConverter.ToString(crc)}");
                            }
                            else
                            {
                                br.ReadBytes(len + 4);
                            }
                           
                            //CRC32Cls crc32 = new CRC32Cls();
                            //List<byte> vs = new List<byte>();
                            //vs.AddRange(Encoding.UTF8.GetBytes("IDAT"));
                            //vs.AddRange(idat);
                            //ulong cr1c32 = crc32.GetCRC32Str(vs.ToArray());
                        }
                        break;
                    case "IEND":
                        {
                            CRC32Cls crc32 = new CRC32Cls();
                            ulong cr1c32 = crc32.GetCRC32Str("IEND");
                            byte[] bb = br.ReadBytes(4);
                            System.Diagnostics.Trace.WriteLine($"IEND len:{len} crc:{BitConverter.ToString(bb)}");
                            return;
                        }
                        break;
                    default:
                        {
                            System.Diagnostics.Trace.WriteLine(id);
                            br.ReadBytes(len + 4);
                        }
                        break;
                }
            }
            }
        

        byte[] idat;

        void WriteIHDR(BinaryWriter bw, IHDR data)
        {
            MemoryStream mm = new MemoryStream();
            BinaryWriter w = new BinaryWriter(mm);
            byte[] sss = Encoding.UTF8.GetBytes("IHDR");
            w.Write(sss);
            w.WriteLN(data.Width);
            w.WriteLN(data.Height);
            w.Write(data.BitDepth);
            w.Write(data.ColorType);
            w.Write(data.Compression);
            w.Write(data.Filter);
            w.Write(data.Iterlace);

            byte[] bb = mm.ToArray();

            bw.WriteLN(bb.Length - 4);
            bw.Write(bb);
            CRC32Cls crc32 = new CRC32Cls();
            ulong crc_ulong = crc32.GetCRC32Str(bb);
            sss = BitConverter.GetBytes(crc_ulong);
            Array.Reverse(sss);
            bw.Write(sss, 4, 4);
        }

        void WriteIDAT(BinaryWriter bw, byte[] data)
        {
            MemoryStream mm = new MemoryStream();
            BinaryWriter w = new BinaryWriter(mm);
            byte[] sss = Encoding.UTF8.GetBytes("IDAT");
            w.Write(sss);
            w.Write(data);
            byte[] bb = mm.ToArray();

            bw.WriteLN(bb.Length - 4);
            bw.Write(bb);
            CRC32Cls crc32 = new CRC32Cls();
            ulong crc_ulong = crc32.GetCRC32Str(bb);
            sss = BitConverter.GetBytes(crc_ulong);
            Array.Reverse(sss);
            bw.Write(sss, 4, 4);
        }

        void WriteIEND(BinaryWriter bw)
        {
            MemoryStream mm = new MemoryStream();
            BinaryWriter w = new BinaryWriter(mm);
            byte[] sss = Encoding.UTF8.GetBytes("IEND");
            w.Write(sss);
            byte[] bb = mm.ToArray();


            bw.WriteLN(bb.Length - 4);
            bw.Write(bb);
            CRC32Cls crc32 = new CRC32Cls();
            ulong crc_ulong = crc32.GetCRC32Str(bb);
            sss = BitConverter.GetBytes(crc_ulong);
            Array.Reverse(sss);
            bw.Write(sss, 4, 4);
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
