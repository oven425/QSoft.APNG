using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace QSoft.Apng.WPF
{
    public static class Apng
    {
        public static Dictionary<fcTL, MemoryStream> SplitApng(this Stream src)
        {
            Png_Reader pngr = new Png_Reader();
            var pngs = pngr.Open(src).SpltAPng();
            return pngs;
        }

        public static Storyboard ToWPF(this Png_Reader src, Image image_png)
        {
            var m_Apng = src.SpltAPng();
            var storyboard = new Storyboard();
            var keyFrames = new ObjectAnimationUsingKeyFrames();
            
            TimeSpan start = TimeSpan.Zero;
            IHDR ihdr = src.IHDR;
            fcTL fctl_prev = null;
            BitmapSource lastblendsource = null;
            for (int i = 0; i < m_Apng.Count; i++)
            {
                fcTL fctl = m_Apng.ElementAt(i).Key;
                var drawingVisual = new DrawingVisual();
                using (DrawingContext dc = drawingVisual.RenderOpen())
                {

                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = m_Apng.ElementAt(i).Value;
                    img.EndInit();
                    img.Freeze();
                    if (fctl.Blend_op == fcTL.Blends.Over && lastblendsource != null)
                    {
                        dc.DrawImage(lastblendsource, new Rect(0, 0, ihdr.Width, ihdr.Height));
                    }
                    dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, ihdr.Width, ihdr.Height));
                    dc.DrawImage(img, new Rect(fctl.X_Offset, fctl.Y_Offset, img.Width, img.Height));
                }
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)drawingVisual.ContentBounds.Width, (int)drawingVisual.ContentBounds.Height, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(drawingVisual);
                if (fctl_prev != null)
                {
                    var dddd = TimeSpan.FromMilliseconds((double)(fctl_prev.Delay_Num) / fctl_prev.Delay_Den);
                    start = start + TimeSpan.FromSeconds((double)(fctl_prev.Delay_Num) / fctl_prev.Delay_Den);
                }
                else
                {
                    fctl_prev = fctl;
                }
                rtb.Freeze();
                //if(fctl.Blend_op == fcTL.Blends.Source)
                {
                    lastblendsource = rtb;
                }
                var keyFrame = new DiscreteObjectKeyFrame
                {
                    KeyTime = start,
                    Value = rtb
                };
                keyFrame.Freeze();
                keyFrames.KeyFrames.Add(keyFrame);

            }

            Storyboard.SetTarget(keyFrames, image_png);
            Storyboard.SetTargetProperty(keyFrames, new PropertyPath("Source"));

            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            keyFrames.Freeze();
            storyboard.Children.Add(keyFrames);
            storyboard.Freeze();

            return storyboard;
        }

        
    }

    //public class ApngSource : BitmapSource
    //{
    //    protected override Freezable CreateInstanceCore()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
