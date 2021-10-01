﻿//#define TestD3DImage
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
            //file.SplitApng();
            //var file = File.OpenRead("../../testapng/pyani.png");
            Png_Reader pngr = new Png_Reader();
            var storyboard = pngr.Open(file).ToWPF(this.image_png);
            storyboard.CurrentTimeInvalidated += Storyboard_CurrentTimeInvalidated;
            //var pngs = pngr.Open(file).SpltAPng();
            //for (int i = 0; i < pngs.Count; i++)
            //{
            //    File.WriteAllBytes($"{i}.png", pngs.ElementAt(i).Value.ToArray());
            //}
            file.Close();
            file.Dispose();
        }

        private void Storyboard_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }

}
