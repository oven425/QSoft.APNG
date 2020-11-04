using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_APNG;

namespace APNG
{
    public class CPNG_Writer
    {
        BinaryWriter m_Bw;
        //MemoryStream m_Temp = new MemoryStream();
        byte[] m_PNGHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        public bool Open(Stream stream)
        {
            this.m_Bw = new BinaryWriter(stream);
            this.m_Bw.Write(this.m_PNGHeader);
            return true;
        }

        public bool WriteIHDR(IHDR data)
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

            this.m_Bw.WriteLN(bb.Length - 4);
            this.m_Bw.Write(bb);
            CRC32Cls crc32 = new CRC32Cls();
            ulong crc_ulong = crc32.GetCRC32Str(bb);
            sss = BitConverter.GetBytes(crc_ulong);
            Array.Reverse(sss);
            this.m_Bw.Write(sss, 4, 4);
            return true;
        }

        public void WriteIDAT(byte[] data)
        {
            MemoryStream mm = new MemoryStream();
            BinaryWriter w = new BinaryWriter(mm);
            byte[] sss = Encoding.UTF8.GetBytes("IDAT");
            w.Write(sss);
            w.Write(data);
            byte[] bb = mm.ToArray();

            this.m_Bw.WriteLN(bb.Length - 4);
            this.m_Bw.Write(bb);
            CRC32Cls crc32 = new CRC32Cls();
            ulong crc_ulong = crc32.GetCRC32Str(bb);
            sss = BitConverter.GetBytes(crc_ulong);
            Array.Reverse(sss);
            this.m_Bw.Write(sss, 4, 4);
        }

        public void WriteIEND()
        {
            MemoryStream mm = new MemoryStream();
            BinaryWriter w = new BinaryWriter(mm);
            byte[] sss = Encoding.UTF8.GetBytes("IEND");
            w.Write(sss);
            byte[] bb = mm.ToArray();


            this.m_Bw.WriteLN(bb.Length - 4);
            this.m_Bw.Write(bb);
            CRC32Cls crc32 = new CRC32Cls();
            ulong crc_ulong = crc32.GetCRC32Str(bb);
            sss = BitConverter.GetBytes(crc_ulong);
            Array.Reverse(sss);
            this.m_Bw.Write(sss, 4, 4);
        }
    }
}
