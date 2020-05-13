using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EditorInterface.ViewModel
{
    public class ISSWritableViewModel
    {
        public ISSWritable Writable { get; set; }
        public ISSWritableViewModel(ISSWritable writable)
        {
            Writable = writable;
        }

        public string Filepath { get => Writable?.RelativeUrl.ToString(); }

        public bool MustOverwrite { get => Writable?.MustOverwrite ?? false; }


    }

    [ValueConversion(typeof(SSFactionGroup), typeof(FactionGroupTokenViewModel))]
    public class ISSWritableToISSWritableVMConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case ISSJsonGroup j:
                    return new JsonGroupViewModel(j);
                case ISSWritable w:
                    return new ISSWritableViewModel(w);
                case null:
                    return null;
                default:
                    throw new ArgumentException("Converting a not writable");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
