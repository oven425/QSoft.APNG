using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;

namespace WPF_APNG
{
    public class MainUI
    {
        public ObservableCollection<FileInfo> Files { set; get; } = new ObservableCollection<FileInfo>();
    }
}
