//#define TestD3DImage
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Resources;
using APNG;
using System.Windows.Threading;
using System.Windows.Interop;
using SharpDX.Direct3D9;
using System.Runtime.InteropServices;
using SharpDX;
using System.Linq;
using APNG.Tool;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Threading.Tasks;

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
        Storyboard _checkStoryboard;
        async private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            
            return;
            StreamResourceInfo sri = Application.GetResourceStream(new Uri("pack://application:,,,/apng_spinfox.png", UriKind.Absolute));
            CPng_Reader pngr = new CPng_Reader();
            //this.m_Apng = pngr.Open(sri.Stream).SpltAPng();
            this.m_Apng = pngr.Open(File.OpenRead("D:\\sample.png")).SpltAPng();
#if TestD3DImage
                        IHDR ihdr = pngr.Chunks.FirstOrDefault(x => x.ChunkType == ChunkTypes.IHDR) as IHDR;
                        this.m_D3DImage.Open(ihdr.Width, ihdr.Height);
                        this.img.Source = this.m_D3DImage;
#endif


            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();





        }

        

        

        int index = 0;
#if TestD3DImage
        Dictionary<int, Tuple<WriteableBitmap, int>> m_Bmps = new Dictionary<int, Tuple<WriteableBitmap, int>>();
#else
        Dictionary<int, BitmapImage> m_Bmps = new Dictionary<int, BitmapImage>();
#endif
        private void Timer_Tick(object sender, EventArgs e)
        {
            return;
#if NET5
#endif
#if TestD3DImage

            if (this.m_Bmps.ContainsKey(index) == false)
            {
                MemoryStream stream = this.m_Apng.ElementAt(index).Value;
                stream.Position = 0;
                IntPtr intptr = Marshal.AllocHGlobal((int)stream.Length);
                Marshal.Copy(stream.ToArray(), 0, intptr, (int)stream.Length);
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = stream;
                bmp.EndInit();
                //WriteableBitmap writeableBitmap = new WriteableBitmap(bmp);
                //this.img.Source = writeableBitmap;
                this.m_Bmps.Add(index, Tuple.Create<WriteableBitmap, int>(new WriteableBitmap(bmp), 148*148*4));
            }
            this.m_D3DImage.Refresh(this.m_Bmps[index].Item1.BackBuffer, this.m_Bmps[index].Item2);
            //this.img.Source = this.m_Bmps[index];
            index = index + 1;
            if (index >= this.m_Apng.Count)
            {
                index = 0;
            }
#else
            Stream stream = this.m_Apng.ElementAt(index).Value;
            if(this.m_Bmps.ContainsKey(index) == false)
            {
                stream.Position = 0;
                BitmapImage bmp = new BitmapImage();
                
                bmp.BeginInit();
                bmp.StreamSource = stream;
                bmp.EndInit();
                this.m_Bmps.Add(index, bmp);
            }
            
            this.img.Source = this.m_Bmps[index];
            index = index + 1;
            if (index >= this.m_Apng.Count)
            {
                index = 0;
            }
#endif
        }

        async private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            TranslateTransform _heartTransform = (sender as Image).RenderTransform as TranslateTransform;
            _checkStoryboard = new Storyboard();

            var keyFrames = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(keyFrames, sender as Image);
            Storyboard.SetTargetProperty(keyFrames, new PropertyPath("RenderTransform.(TranslateTransform.X)"));
            TimeSpan start = TimeSpan.Zero;
            for (var i = 0; i < 28; i++)
            {
                var keyFrame = new DiscreteDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromSeconds((i + 1d) / 28d),
                    //KeyTime = TimeSpan.FromSeconds(1),
                    Value = -(i + 1) * 100
                };
                keyFrames.KeyFrames.Add(keyFrame);
            }

            _checkStoryboard.Children.Add(keyFrames);

            _checkStoryboard.FillBehavior = FillBehavior.HoldEnd;

            await Task.Delay(1000);
            //_checkStoryboard.Begin();
        }

        private void Button_test_Click(object sender, RoutedEventArgs e)
        {
            _checkStoryboard.Begin();
        }
    }

    public class CD3DImage:D3DImage
    {
        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
        public void Open(int width, int height)
        {
            Direct3DEx _direct3D = new Direct3DEx();

            PresentParameters presentparams = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                PresentationInterval = PresentInterval.Default,
                PresentFlags = PresentFlags.Video,
                // The device back buffer is not used.
                BackBufferFormat = Format.Unknown,
                BackBufferWidth = width,
                BackBufferHeight = height,

                // Use dummy window handle.
                DeviceWindowHandle = GetDesktopWindow()
            };


            _device = new DeviceEx(_direct3D, 0, DeviceType.Hardware, IntPtr.Zero,
                                   CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve,
                                   presentparams);
            IntPtr handle = IntPtr.Zero;
            //SharpDX.Direct3D9.Texture texture = new Texture(_device, width, height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default, ref handle);
            //SharpDX.Direct3D9.Surface surface = texture.GetSurfaceLevel(0);
            surface = SharpDX.Direct3D9.Surface.CreateOffscreenPlain(_device, width, height, Format.X8R8G8B8, Pool.Default);

            _swapChain = new SwapChain(_device, presentparams);


            this.Lock();
            this.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _swapChain.GetBackBuffer(0).NativePointer);
            this.Unlock();
        }

        int m_Width;
        int m_Height;
        SwapChain _swapChain;
        DeviceEx _device;
        Surface surface;
        public void Refresh(IntPtr ptr, int len)
        {
            DataRectangle rect = surface.LockRectangle(LockFlags.Discard);
            CopyMemory(rect.DataPointer, ptr, (uint)len);

            
            surface.UnlockRectangle();

            using (Surface bb = _swapChain.GetBackBuffer(0))
            {
                try
                {
                    _swapChain.Device.StretchRectangle(surface, bb, TextureFilter.None);

                }
                catch (Exception ee)
                {
                    System.Diagnostics.Trace.WriteLine(ee.Message);
                }
                _swapChain.Device.Present();
            }
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                Lock();
                this.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _swapChain.GetBackBuffer(0).NativePointer);
                AddDirtyRect(new Int32Rect(0, 0, PixelWidth, PixelHeight));

                Unlock();
            }));
        }
    }
}
