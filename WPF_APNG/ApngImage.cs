using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPF_APNG
{
    public class ApngImage : Control
    {
        public static readonly DependencyProperty SourceProperty;
        static ApngImage()
        {
            SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(ApngImage));
        }

        public Uri Source
        {
            set
            {
                SetValue(SourceProperty, value);
            }
            get
            {
                return (Uri)GetValue(SourceProperty);
            }
        }
    }
}
