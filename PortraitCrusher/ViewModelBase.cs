using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PortraitCrusher
{
    public abstract class  ViewModelBase : INotifyPropertyChanged //, IDisposable
    {
        #region Constructor

        protected ViewModelBase()
        {
        }

        #endregion // Constructor


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            //this.VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion

        //#region IDisposable Members

        ///// <summary>
        ///// Invoked when this object is being removed from the application
        ///// and will be subject to garbage collection.
        ///// </summary>
        //public void Dispose()
        //{
        //    this.OnDispose();
        //}

        ///// <summary>
        ///// Child classes can override this method to perform 
        ///// clean-up logic, such as removing event handlers.
        ///// </summary>
        //protected virtual void OnDispose()
        //{
        //}

        //#endregion 
    }
}
