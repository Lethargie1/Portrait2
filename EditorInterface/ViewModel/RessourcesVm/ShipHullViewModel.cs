using SSEditor.Ressources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EditorInterface.ViewModel
{
    public class ShipHullViewModel
    {
        public IShipHull ShipHull { get; set; }
        public ShipHullViewModel(IShipHull shipHull)
        {
            ShipHull = shipHull;
        }

        public string DisplayName { get => ShipHull.Id; }
    }

    [ValueConversion(typeof(IShipHull), typeof(ShipHullViewModel))]
    public class ShipHullToShipHullVMConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new ShipHullViewModel((IShipHull)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
