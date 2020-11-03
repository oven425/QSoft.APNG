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
            byte[] sss = br.ReadBytes(4);
            Array.Reverse(sss);
            data.Width = BitConverter.ToInt32(sss, 0);
            sss = br.ReadBytes(4);
            Array.Reverse(sss);
            data.Height = BitConverter.ToInt32(sss, 0);
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
            BinaryReader br = new BinaryReader(sri.Stream);
            
            this.ParsePNG(br);


            FileStream fs = File.Create("test.png");
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(this.m_PNGHeader);
            this.WriteIHDR(bw, this.m_IHDR);
            this.WriteIDAT(bw, idat);
            this.WriteIEND(bw);
            fs.Close();
            fs.Dispose();
            FileStream fs1 = File.OpenRead(@"test.png");
            br = new BinaryReader(fs1);
            idat = null;
            this.ParsePNG(br);


        }

        IHDR m_IHDR = new IHDR();
        void ParsePNG( BinaryReader br)
        {
            byte[] sss = br.ReadBytes(8);
            System.Diagnostics.Trace.WriteLine(BitConverter.ToString(sss));
            while (true)
            {
                sss = br.ReadBytes(4);
                Array.Reverse(sss);
                int len = BitConverter.ToInt32(sss, 0);
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
                            sss = br.ReadBytes(4);
                            Array.Reverse(sss);
                            int num_frames = BitConverter.ToInt32(sss, 0);
                            sss = br.ReadBytes(4);
                            Array.Reverse(sss);
                            int num_plays = BitConverter.ToInt32(sss, 0);
                            br.ReadBytes(4);
                            System.Diagnostics.Trace.WriteLine($"acTL num_frames:{num_frames} num_plays:{num_plays}");
                        }
                        break;
                    case "fcTL":
                        {
                            sss = br.ReadBytes(4);
                            Array.Reverse(sss);
                            int sequence_number = BitConverter.ToInt32(sss, 0);
                            sss = br.ReadBytes(4);
                            Array.Reverse(sss);
                            int width = BitConverter.ToInt32(sss, 0);
                            sss = br.ReadBytes(4);
                            Array.Reverse(sss);
                            int height = BitConverter.ToInt32(sss, 0);
                            sss = br.ReadBytes(4);
                            Array.Reverse(sss);
                            int x_offset = BitConverter.ToInt32(sss, 0);
                            sss = br.ReadBytes(4);
                            Array.Reverse(sss);
                            int y_offset = BitConverter.ToInt32(sss, 0);
                            sss = br.ReadBytes(2);
                            Array.Reverse(sss);
                            int delay_num = BitConverter.ToInt16(sss, 0);
                            sss = br.ReadBytes(2);
                            Array.Reverse(sss);
                            int delay_den = BitConverter.ToInt16(sss, 0);
                            byte dispose_op = br.ReadByte();
                            byte blend_op = br.ReadByte();
                            br.ReadBytes(4);
                            //System.Diagnostics.Trace.WriteLine($"fcTL sequence_number:{sequence_number} width:{width} height:{height} delay_num:{delay_num} delay_den:{delay_den}");
                        }
                        break;
                    case "fdAT":
                        {

                            sss = br.ReadBytes(4);
                            Array.Reverse(sss);
                            int sequence_number = BitConverter.ToInt32(sss, 0);
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
            sss = BitConverter.GetBytes(data.Width);
            Array.Reverse(sss);
            w.Write(sss);
            sss = BitConverter.GetBytes(data.Height);
            Array.Reverse(sss);
            w.Write(sss);
            w.Write(data.BitDepth);
            w.Write(data.ColorType);
            w.Write(data.Compression);
            w.Write(data.Filter);
            w.Write(data.Iterlace);

            byte[] bb = mm.ToArray();

            sss = BitConverter.GetBytes(bb.Length-4);
            Array.Reverse(sss);
            bw.Write(sss);
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

            sss = BitConverter.GetBytes(bb.Length-4);
            Array.Reverse(sss);
            bw.Write(sss);
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
            

            sss = BitConverter.GetBytes(bb.Length-4);
            Array.Reverse(sss);
            bw.Write(sss);
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

        //获取字符串的CRC32校验值
        public ulong GetCRC32Str(string sInputString)
        {
            //生成码表
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

        //获取字符串的CRC32校验值
        public ulong GetCRC32Str(byte[] data)
        {
            //生成码表
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

    public class IHDR
    {
        //long pos = br.BaseStream.Position;
        //sss = br.ReadBytes(4);
        //                    Array.Reverse(sss);
        //                     = BitConverter.ToInt32(sss, 0);
        //sss = br.ReadBytes(4);
        //                    Array.Reverse(sss);
        //                    int height = BitConverter.ToInt32(sss, 0);
        //byte bitdepth = br.ReadByte();
        //byte colortype = br.ReadByte();
        //byte compression = br.ReadByte();
        //byte filter = br.ReadByte();
        //byte interlace = br.ReadByte();
        //pos = br.BaseStream.Position - pos;
        //                    br.ReadBytes(4);
        //                    System.Diagnostics.Trace.WriteLine($"IHDR width:{width} height:{height}");
        public int Width { set; get; }
        public int Height { set; get; }
        public byte BitDepth { set; get; }
        public byte ColorType { set; get; }
        public byte Compression { set; get; }
        public byte Filter { set; get; }
        public byte Iterlace { set; get; }
    }

    public class CPngReader
    {
        public bool IsAPNG { set; get; }
        public bool Open(string file)
        {
            bool result = true;

            return result;
        }


    }
}
