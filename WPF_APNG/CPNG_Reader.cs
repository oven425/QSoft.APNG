using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_APNG;

namespace APNG
{
    public class CPNG_Reader
    {
        
        public List<Chunk> Chunks { set; get; } = new List<Chunk>();
        byte[] m_PNGHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        public bool Open(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            bool result = true;
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
                            IHDR ihdr = new IHDR();
                            ihdr.Pos = stream.Position;
                            ihdr.Size = len;
                            ParseIHDR(br, ihdr);
                            this.Chunks.Add(ihdr);
                            System.Diagnostics.Trace.WriteLine($"IHDR width:{ihdr.Width} height:{ihdr.Height}");
                        }
                        break;
                    case "acTL":
                        {
                            acTL actl = new acTL();
                            actl.Pos = stream.Position;
                            actl.Size = len;
                            actl.Num_Frames = br.ReadInt32LN();
                            actl.Num_Plays = br.ReadInt32LN();
                            actl.CRC = br.ReadBytes(4);
                            this.Chunks.Add(actl);
                            System.Diagnostics.Trace.WriteLine($"acTL num_frames:{actl.Num_Frames} num_plays:{actl.Num_Plays}");
                        }
                        break;
                    case "fcTL":
                        {
                            fcTL fcTL = new fcTL();
                            fcTL.Pos = stream.Position;
                            fcTL.Size = len;
                            fcTL.SequenceNumber = br.ReadInt32LN();
                            fcTL.Width = br.ReadInt32LN();
                            fcTL.Height = br.ReadInt32LN();
                            fcTL.X_Offset = br.ReadInt32LN();
                            fcTL.Y_Offset = br.ReadInt32LN();
                            fcTL.Delay_Num = br.ReadInt16LN();
                            fcTL.Delay_Den = br.ReadInt16LN();
                            fcTL.Dispose_op = br.ReadByte();
                            fcTL.Blend_op = br.ReadByte();
                            fcTL.CRC = br.ReadBytes(4);
                            this.Chunks.Add(fcTL);
                            System.Diagnostics.Trace.WriteLine($"fcTL sequence_number:{fcTL.SequenceNumber} width:{fcTL.Width} height:{fcTL.Height} delay_num:{fcTL.Delay_Num} delay_den:{fcTL.Delay_Den}");
                        }
                        break;
                    case "fdAT":
                        {
                            fdAT fdat = new fdAT();
                            fdat.Pos = stream.Position;
                            fdat.Size = len;
                            fdat.SequenceNumber = br.ReadInt32LN();
                            fdat.Data = br.ReadBytes(len-4);
                            fdat.CRC = br.ReadBytes(4);
                            this.Chunks.Add(fdat);
                            System.Diagnostics.Trace.WriteLine($"fdAT len:{len} sequence_number:{fdat.SequenceNumber}");
                        }
                        break;
                    case "IDAT":
                        {
                            IDAT idat = new IDAT();
                            idat.Pos = stream.Position;
                            idat.Size = len;
                            //br.BaseStream.Seek(len, SeekOrigin.Current);
                            idat.Data = br.ReadBytes(len);
                            idat.CRC = br.ReadBytes(4);
                            this.Chunks.Add(idat);
                            System.Diagnostics.Trace.WriteLine($"IDAT len:{len}");
                        }
                        break;
                    case "IEND":
                        {
                            Chunk iend = new Chunk() { ChunkType = ChunkTypes.IEND };
                            iend.CRC = br.ReadBytes(4);
                            this.Chunks.Add(iend);
                            System.Diagnostics.Trace.WriteLine($"IEND len:{len} crc:{BitConverter.ToString(iend.CRC)}");
                            return true;
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
            return result;
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
            data.CRC = br.ReadBytes(4);
            System.Diagnostics.Trace.WriteLine($"IHDR crc:{BitConverter.ToString(data.CRC)}");
        }
    }

    
}
