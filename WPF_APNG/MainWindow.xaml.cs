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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //            StreamResourceInfo sri = Application.GetResourceStream(new Uri("pack://application:,,,/apng_spinfox.png", UriKind.Absolute));
            //            CPng_Reader pngr = new CPng_Reader();
            //            this.m_Apng = pngr.Open(sri.Stream).SpltAPng();
            //#if TestD3DImage
            //            IHDR ihdr = pngr.Chunks.FirstOrDefault(x => x.ChunkType == ChunkTypes.IHDR) as IHDR;
            //            this.m_D3DImage.Open(ihdr.Width, ihdr.Height);
            //            this.img.Source = this.m_D3DImage;
            //#endif


            //            DispatcherTimer timer = new DispatcherTimer();
            //            timer.Interval = TimeSpan.FromMilliseconds(5);
            //            timer.Tick += Timer_Tick;
            //            timer.Start();


            //StreamResourceInfo sri = Application.GetResourceStream(new Uri("pack://application:,,,/photo.jpg", UriKind.Absolute));
            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            //BitmapImage bmp = new BitmapImage();
            //bmp.BeginInit();
            //bmp.StreamSource = sri.Stream;
            //bmp.EndInit();
            //encoder.Rotation = Rotation.Rotate90;
            //encoder.Frames.Add(BitmapFrame.Create(bmp));
            //using (Stream fs = File.Create("rotate90.jpg"))
            //{
            //    encoder.Save(fs);
            //}
            //BinaryWriter bw = new BinaryWriter(File.Open("rotate90.jpg", FileMode.Open));
            //bw.BaseStream.Position = 48;
            //bw.WriteLN(1);
            BinaryReader br = new BinaryReader(File.Open("rotate90.jpg", FileMode.Open));
            byte[] header = br.ReadBytes(2);
            while(true)
            {
                header = br.ReadBytes(2);
                string header_str = BitConverter.ToString(header);
                System.Diagnostics.Trace.WriteLine(header_str);
                short len = br.ReadInt16LN();
                switch (header_str)
                {
                    case "FF-E0":
                        {
                            string Identifier = Encoding.UTF8.GetString(br.ReadBytes(5));
                            byte[] version = br.ReadBytes(2);
                            byte Density_units = br.ReadByte();
                            short Xdensity = br.ReadInt16LN();
                            short Ydensity = br.ReadInt16LN();
                            byte XThumbnail = br.ReadByte();
                            byte YThumbnail = br.ReadByte();
                        }
                        break;
                    case "FF-C0":
                        {
                            br.ReadByte();
                            byte[] widths = br.ReadBytes(2);
                            byte[] heights = br.ReadBytes(2);
                            int width = widths[0] * 256 + widths[1];
                            int height = heights[0] * 256 + heights[1];
                            br.ReadByte();
                            br.ReadBytes(3);
                            br.ReadBytes(3);
                            br.ReadBytes(3);
                        }
                        break;
                    case "FF-E1":
                        {
                            
                            string exif = Encoding.UTF8.GetString(br.ReadBytes(4));
                            byte[] bb = br.ReadBytes(2);
                            long exifbegin = br.BaseStream.Position;
                            string mmll = Encoding.UTF8.GetString(br.ReadBytes(2));
                            string version = BitConverter.ToString(br.ReadBytesLN(2));
                            int offset = br.ReadInt32LN();
                            short ifd_count = br.ReadInt16LN();
                            for(int i=0; i<ifd_count; i++)
                            {
                                ushort tag = br.ReadUInt16LN();
                                short type = br.ReadInt16LN();
                                int count = br.ReadInt32LN();
                                
                                int offset1 = br.ReadInt32LN();
                                if(tag == 0x8769)
                                {
                                    long oldpos = br.BaseStream.Position;
                                    br.BaseStream.Position = exifbegin + offset1;
                                    int subcount = br.ReadUInt16LN();
                                    tag = br.ReadUInt16LN();
                                    type = br.ReadInt16LN();
                                    count = br.ReadInt32LN();
                                    offset1 = br.ReadInt32LN();

                                    br.BaseStream.Position = oldpos;
                                }
                                else
                                {
                                    long oldpos = br.BaseStream.Position;
                                    br.BaseStream.Position = exifbegin + offset1;
                                    byte[]rr = br.ReadBytes(3);

                                    br.BaseStream.Position = oldpos;
                                }
                            }
                        }
                        break;
                    case "FF-DB"://Define Quantization Table
                    case "FF-C4"://Define Huffman Table
                    default:
                        {
                            header = br.ReadBytes(len - 2);
                        }
                        break;
                }
                
                
                
            }
        }

        int index = 0;
#if TestD3DImage
        Dictionary<int, Tuple<WriteableBitmap, int>> m_Bmps = new Dictionary<int, Tuple<WriteableBitmap, int>>();
#else
        Dictionary<int, BitmapImage> m_Bmps = new Dictionary<int, BitmapImage>();
#endif
        private void Timer_Tick(object sender, EventArgs e)
        {
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
