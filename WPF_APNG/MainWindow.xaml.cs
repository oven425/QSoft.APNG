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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StreamResourceInfo sri = Application.GetResourceStream(new Uri("pack://application:,,,/apng_spinfox.png", UriKind.Absolute));
            //FileStream fs = File.OpenRead(@"D:\apng_spinfox.png");
            BinaryReader br = new BinaryReader(sri.Stream);
            byte[] sss = br.ReadBytes(8);
            System.Diagnostics.Trace.WriteLine(BitConverter.ToString(sss));
            while(true)
            {
                sss = br.ReadBytes(4);
                Array.Reverse(sss);
                int len = BitConverter.ToInt32(sss, 0);
                sss = br.ReadBytes(4);
                string id = Encoding.UTF8.GetString(sss);
                
                switch(id)
                {
                    case "IHDR":
                        {
                            long pos = br.BaseStream.Position;
                            sss = br.ReadBytes(4);
                            Array.Reverse(sss);
                            int width = BitConverter.ToInt32(sss, 0);
                            sss = br.ReadBytes(4);
                            Array.Reverse(sss);
                            int height = BitConverter.ToInt32(sss, 0);
                            byte bitdepth = br.ReadByte();
                            byte colortype = br.ReadByte();
                            byte compression = br.ReadByte();
                            byte filter = br.ReadByte();
                            byte interlace = br.ReadByte();
                            pos = br.BaseStream.Position - pos;
                            br.ReadBytes(4);
                            System.Diagnostics.Trace.WriteLine($"IHDR width:{width} height:{height}");
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
                            System.Diagnostics.Trace.WriteLine($"fcTL sequence_number:{sequence_number} width:{width} height:{height} delay_num:{delay_num} delay_den:{delay_den}");
                        }
                        break;
                    case "fdAT":
                        {

                            sss = br.ReadBytes(4);
                            Array.Reverse(sss);
                            int sequence_number = BitConverter.ToInt32(sss, 0);
                            br.ReadBytes(len - 4);
                            br.ReadBytes(4);
                            System.Diagnostics.Trace.WriteLine($"fdAT len:{len} sequence_number:{sequence_number}");
                        }
                        break;
                    case "IDAT":
                        {
                            System.Diagnostics.Trace.WriteLine($"IDAT len:{len}");
                            br.ReadBytes(len + 4);
                        }
                        break;
                    case "IEND":
                        {
                            br.ReadBytes(4);
                            return;
                        }
                        break;
                    default:
                        {
                            System.Diagnostics.Trace.WriteLine(id);
                            br.ReadBytes(len+4);
                        }
                        break;
                }
                
            }
            

        }
        
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
