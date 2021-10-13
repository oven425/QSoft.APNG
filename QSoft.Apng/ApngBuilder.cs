using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QSoft.Apng.Build
{
    public class ApngBuilder
    {
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
        public Stream Build(IEnumerable<string> images, TimeSpan totaltime)
        {
            MemoryStream temp = new MemoryStream();
            MemoryStream stream = new MemoryStream();
            TimeSpan time = TimeSpan.FromSeconds(totaltime.TotalSeconds/images.Count());
            IHDR ihdr = null;
            acTL actl = new acTL();
            actl.Num_Frames = images.Count();
            PNG_Writer pngw = new PNG_Writer();
            pngw.Open(stream);
            int index =0;
            foreach (var oo in images)
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = new MemoryStream(File.ReadAllBytes(oo));
                img.EndInit();
                var color = GetPixelColor(img, 10, 10);

                temp.SetLength(0);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                var frame = BitmapFrame.Create(img, null, null, null);
                encoder.Frames.Add(frame);
                encoder.Save(temp);
                temp.Position = 0;
                Png_Reader pngr = new Png_Reader();
                //pngr.Open(File.OpenRead(oo));
                pngr.Open(temp);
                var idat = pngr.iDAT();

                

                if (ihdr == null)
                {
                    ihdr = new IHDR();
                    ihdr.Width = (int)148;
                    ihdr.Height = (int)148;
                    ihdr.BitDepth = 8;
                    ihdr.ColorType = 6;
                    ihdr.Filter = 0;
                    ihdr.Compression = 0;
                    ihdr.Iterlace = 0;
                    pngw.WriteIHDR(ihdr);
                    pngw.WriteACTL(actl);

                    fcTL fcTL = new fcTL();
                    fcTL.SequenceNumber = index++;
                    fcTL.Width = (int)148;
                    fcTL.Height = (int)148;
                    fcTL.X_Offset = 0;
                    fcTL.Y_Offset = 0;
                    fcTL.Delay_Num = 50;
                    fcTL.Delay_Den = 1000;
                    fcTL.Dispose_op = fcTL.Diposes.Background;
                    fcTL.Blend_op = fcTL.Blends.Source;
                    pngw.WriteFCTL(fcTL);

                    MemoryStream mm = new MemoryStream(idat.Data);
                    while(true)
                    {
                        byte[] buf = new byte[8192];
                        int readlen = mm.Read(buf, 0, buf.Length);
                        pngw.WriteIDAT(buf, readlen);
                        if(readlen < buf.Length)
                        {
                            break;
                        }
                    }
                    mm.Close();
                    mm.Dispose();
                    //pngw.WriteIDAT(idat.Data);
                }
                else
                {
                    fcTL fcTL = new fcTL();
                    fcTL.SequenceNumber = index++;
                    fcTL.Width = (int)148;
                    fcTL.Height = (int)148;
                    fcTL.X_Offset = 0;
                    fcTL.Y_Offset = 0;
                    fcTL.Delay_Num = 50;
                    fcTL.Delay_Den = 1000;
                    fcTL.Dispose_op = fcTL.Diposes.Background;
                    fcTL.Blend_op = fcTL.Blends.Source;
                    pngw.WriteFCTL(fcTL);

                    MemoryStream mm = new MemoryStream(idat.Data);
                    while (true)
                    {
                        byte[] buf = new byte[8192];
                        int readlen = mm.Read(buf, 0, buf.Length);
                        pngw.WritefdAT(buf, readlen, index++);
                        if (readlen < buf.Length)
                        {
                            break;
                        }
                    }
                    mm.Close();
                    mm.Dispose();

                    //pngw.WritefdAT(idat.Data, index++);
                }


                

            }
            pngw.WriteIEND();
            return stream;
        }
    }
}
