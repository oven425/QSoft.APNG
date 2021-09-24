//#define TestD3DImage
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
#if TestD3DImage
        CD3DImage m_D3DImage = new CD3DImage();
#endif
        MainUI m_MainUI = null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.m_MainUI == null)
            {
                DirectoryInfo dir = new DirectoryInfo("../../testapng");
                var files =  dir.GetFiles();
                this.m_MainUI = new MainUI();
                foreach(var oo in files)
                {
                    this.m_MainUI.Files.Add(oo);
                }
                this.DataContext = this.m_MainUI;
            }
            Uri uri = new Uri("pack://application:,,,/WPF_APNG;component/elephant.png");
           
            //File.OpenRead("pack://application:,,,/WPF_APNG;component/elephant.png");
            //var apngs = Directory.GetFiles("../../testapng");
            //var file = File.OpenRead("../../testapng/elephant.png");
            var file = File.OpenRead("../../testapng/SDve91m.png");
            //var file = File.OpenRead("../../testapng/pyani.png");
            Png_Reader pngr = new Png_Reader();
            pngr.Open(file).ToWPF(this.image_png).Begin();
            //this.m_Apng = pngr.Open(file).SpltAPng();

            
            //var storyboard = new Storyboard();
            //var keyFrames = new ObjectAnimationUsingKeyFrames();
            //Storyboard.SetTarget(keyFrames, this.image_png);
            //Storyboard.SetTargetProperty(keyFrames, new PropertyPath("Source"));
            //TimeSpan start = TimeSpan.Zero;
            //IHDR ihdr = pngr.IHDR;
            //fcTL fctl_prev = null;
            //BitmapSource lastblendsource = null;
            //for (int i = 0; i < this.m_Apng.Count; i++)
            //{
            //    fcTL fctl = this.m_Apng.ElementAt(i).Key;
            //    var drawingVisual = new DrawingVisual();
            //    using (DrawingContext dc = drawingVisual.RenderOpen())
            //    {
                    
            //        BitmapImage img = new BitmapImage();
            //        img.BeginInit();
            //        img.StreamSource = this.m_Apng.ElementAt(i).Value;
            //        img.EndInit();
            //        img.Freeze();
            //        if(fctl.Blend_op == fcTL.Blends.Over && lastblendsource != null)
            //        {
            //            dc.DrawImage(lastblendsource, new Rect(0, 0, ihdr.Width, ihdr.Height));
            //        }
            //        dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, ihdr.Width, ihdr.Height));
            //        dc.DrawImage(img, new Rect(fctl.X_Offset, fctl.Y_Offset, img.Width, img.Height));
            //    }
            //    RenderTargetBitmap rtb = new RenderTargetBitmap((int)drawingVisual.ContentBounds.Width, (int)drawingVisual.ContentBounds.Height, 96, 96, PixelFormats.Pbgra32);
            //    rtb.Render(drawingVisual);
            //    if (fctl_prev != null)
            //    {
            //        var dddd = TimeSpan.FromMilliseconds((double)(fctl_prev.Delay_Num) / fctl_prev.Delay_Den);
            //        start = start + TimeSpan.FromSeconds((double)(fctl_prev.Delay_Num) / fctl_prev.Delay_Den);
            //    }
            //    else
            //    {
            //        fctl_prev = fctl;
            //    }
            //    rtb.Freeze();
            //    //if(fctl.Blend_op == fcTL.Blends.Source)
            //    {
            //        lastblendsource = rtb;
            //    }
            //    var keyFrame = new DiscreteObjectKeyFrame
            //    {
            //        //KeyTime = TimeSpan.FromSeconds(i * 0.04),
            //        KeyTime = start,
            //        Value = rtb
            //    };
            //    keyFrame.Freeze();
            //    keyFrames.KeyFrames.Add(keyFrame);

            //    //// Encoding the RenderBitmapTarget as a PNG file.
            //    //PngBitmapEncoder png = new PngBitmapEncoder();
            //    //png.Frames.Add(BitmapFrame.Create(rtb));
            //    //using (Stream stm = File.Create($"{ this.m_Apng.ElementAt(i).Key.SequenceNumber}.png"))
            //    //{
            //    //    png.Save(stm);
            //    //}
            //    //File.WriteAllBytes($"{this.m_Apng.ElementAt(i).Key.SequenceNumber}.png", this.m_Apng.ElementAt(i).Value.ToArray());
            //}
            //storyboard.RepeatBehavior = RepeatBehavior.Forever;
            //keyFrames.Freeze();
            //storyboard.Children.Add(keyFrames);
            //storyboard.Freeze();
            //storyboard.Begin();

            






        }





    }

}
