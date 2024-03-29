﻿using System;
using System.Collections.Generic;

namespace QSoft.Apng
{
    public class pHYs : Chunk
    {
        public pHYs()
        {
            this.ChunkType = ChunkTypes.pHYs;
        }
        public int X { set; get; }
        public int Y { set; get; }
        public byte Unit { set; get; }
    }
    public class IDAT: Chunk
    {
        public IDAT()
        {
            this.ChunkType = ChunkTypes.IDAT;
        }
        public byte[] Data { set; get; }
    }

    public class fdAT : IDAT
    {
        public fdAT()
        {
            this.ChunkType = ChunkTypes.fdAT;
        }
        public int SequenceNumber { set; get; }
    }

    public class IHDR: Chunk
    {
        public IHDR()
        {
            this.ChunkType = ChunkTypes.IHDR;
        }

        public IHDR(IHDR data)
            : base(data)
        {
            this.Width = data.Width;
            this.Height = data.Height;
            this.BitDepth = data.BitDepth;
            this.ColorType = data.ColorType;
            this.Compression = data.Compression;
            this.Filter = data.Filter;
            this.Iterlace = data.Iterlace;
        }

        public int Width { set; get; }
        public int Height { set; get; }
        public byte BitDepth { set; get; }
        public byte ColorType { set; get; }
        public byte Compression { set; get; }
        public byte Filter { set; get; }
        public byte Iterlace { set; get; }
    }

    public class fcTL:Chunk
    {
        public fcTL()
        {
            this.ChunkType = ChunkTypes.fcTL;
        }
        public int SequenceNumber { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }
        public int X_Offset { set; get; }
        public int Y_Offset { set; get; }
        public short Delay_Num { set; get; }
        public short Delay_Den { set; get; }
        public Diposes Dispose_op { set; get; }
        public Blends Blend_op { set; get; }

        public enum Diposes
        {
            None,
            Background,
            Previous
        }

        public enum Blends
        {
            Source,
            Over
        }
    }

    public class acTL:Chunk
    {
        public acTL()
        {
            this.ChunkType = ChunkTypes.acTL;
        }
        public int Num_Frames { set; get; }
        public int Num_Plays { set; get; }
    }

    public class PLTE : Chunk
    {
        public PLTE()
        {
            this.ChunkType = ChunkTypes.pLTE;

        }


//#if NET472 || NET461

//        //public List<(byte R, byte G, byte B)> RGBs { set; get; } = new List<(byte R, byte G, byte B)>();
//#else
//        string ss = Environment.GetEnvironmentVariable("TEMP");
//#endif

        public class RGB
        {
            public byte R { set; get; }
            public byte G { set; get; }
            public byte B { set; get; }
        }
        public List<RGB> RGBs { set; get; } = new List<RGB>();
    }

    public class tRNS:Chunk
    {
        public tRNS()
        {
            this.ChunkType = ChunkTypes.tRNS;
        }

        public byte[] Data { set; get; }
    }

    public enum ChunkTypes
    {
        IDAT,
        pHYs,
        IHDR,
        acTL,
        fcTL,
        fdAT,
        pLTE,
        tRNS,
        IEND,
    }

    public class sRGB : Chunk
    {
        //0: Perceptual
        //1: Relative colorimetric
        //2: Saturation
        //3: Absolute colorimetric
        public byte Data { set; get; }
    }

    public class gAMA : Chunk
    {
        public uint Data { set; get; }
    }

    public class Chunk
    {
        public Chunk()
        {

        }

        public Chunk(Chunk data)
        {
            this.ChunkType = data.ChunkType;
            this.Pos = data.Pos;
            this.Size = data.Size;
            this.CRC = new byte[data.CRC.Length];
            Array.Copy(data.CRC, this.CRC, data.CRC.Length);
        }
        public ChunkTypes ChunkType { set; get; }
        public long Pos { set; get; }
        public int Size { set; get; }
        public byte[] CRC { set; get; }

    }

    
}
