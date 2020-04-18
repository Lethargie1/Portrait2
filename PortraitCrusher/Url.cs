using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ookii.Dialogs.Wpf;

using SSEditor.FileHandling;

namespace PortraitCrusher
{
    public enum URLstate { Acceptable, Rejected, Unset }
    public class URLViewModel : ViewModelBase
    {
        #region Field
        string _PointedUrl;
        #endregion

        #region Properties
        public string PointedUrl
        {
            get
            {
                return _PointedUrl;
            }
            set
            {
                _PointedUrl = value;
                base.NotifyPropertyChanged();
                base.NotifyPropertyChanged("DisplayUrl");
                base.NotifyPropertyChanged("UrlState");
            }
        }


        public string DisplayName { get; set; }
        public Predicate<SSFullUrl> ValidityChecker { get; set; } = null;
        public URLstate UrlState
        {
            get
            {
                if (_PointedUrl == null)
                    return URLstate.Unset;
                if (ValidityChecker == null)
                {
                    if (_PointedUrl.Exist() == true)
                        return URLstate.Acceptable;
                    else
                        return URLstate.Rejected;
                }
                else
                {
                    if (ValidityChecker(this.PointedUrl) == true)
                        return URLstate.Acceptable;
                    else
                        return URLstate.Rejected;
                }

            }
        }
        #endregion

        #region Constructors
        public URLViewModel()
        {
            PointedUrl = "";
            DisplayName = null;
        }
        public URLViewModel(string displayName, string pointed = "")
        {
            PointedUrl = pointed;
            DisplayName = displayName;
        }


    }

    public class EditableURLViewModel : URLViewModel
    {
        #region Command properties
        RelayCommand<object> _SelectNewUrlCommand;
        public ICommand SelectNewUrlCommand
        {
            get
            {
                if (_SelectNewUrlCommand == null)
                {
                    _SelectNewUrlCommand = new RelayCommand<object>(param => this.SelectNewUrl_Execute());
                }
                return _SelectNewUrlCommand;
            }
        }
        #endregion

        #region Properties
        public string ButtonName { get; set; } = "Edit Url";
        #endregion

        #region Constructors
        public EditableURLViewModel()
            : base()
        { }
        public EditableURLViewModel(string displayName, string buttonName)
            : base(displayName)
        {
            ButtonName = buttonName;
        }
        #endregion

        #region helper method
        private void SelectNewUrl_Execute()
        {
            VistaFolderBrowserDialog OpenRootFolder = new VistaFolderBrowserDialog();
            if (OpenRootFolder.ShowDialog() == true)
            {
                base.LinkingUrl = null;
                base.RelativeUrl = null;
                base.CommonUrl = OpenRootFolder.SelectedPath;

            }
            return;
        }
        #endregion

    }
}
