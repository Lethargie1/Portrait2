using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    interface ISSUrl : INotifyPropertyChanged
    {
        string ToString();    
    }
    interface ISSLink : ISSUrl
    {
        string Link { get; set; }
    }
    interface ISSBase : ISSUrl
    {
        string Base { get; set; }
    }
    interface ISSRelative :ISSUrl
    {
        string Relative { get; set; }
    }
}
