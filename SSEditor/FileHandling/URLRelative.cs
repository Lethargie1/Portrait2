using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public class URLRelative : INotifyPropertyChanged, IEquatable<URLRelative>
    {
        #region Properties
        string _CommonUrl = null;
        public string CommonUrl { get { return _CommonUrl; } set { _CommonUrl = value; NotifyPropertyChanged(); NotifyUrlChanged(); } }
        string _LinkingUrl = null;
        public string LinkingUrl { get { return _LinkingUrl; } set { _LinkingUrl = value; NotifyPropertyChanged(); NotifyUrlChanged(); } }
        string _RelativeUrl = null;
        public string RelativeUrl { get { return _RelativeUrl; } set { _RelativeUrl = value; NotifyPropertyChanged(); NotifyUrlChanged(); } }
        public void NotifyUrlChanged()
        {
            NotifyPropertyChanged("FullUrl");
            NotifyPropertyChanged("ShortUrl");
            NotifyPropertyChanged("IsComplete");
        }
        #endregion //Properties

        #region read  only property
        public string FullUrl
        {
            get
            {
                if (this.CommonUrl == null)
                    return null;
                string result = this.CommonUrl;
                if (this.LinkingUrl != null)
                    result = Path.Combine(result, this.LinkingUrl);
                if (this.RelativeUrl != null)
                    result = Path.Combine(result, this.RelativeUrl);
                return result;
            }
        }
        public string ShortUrl
        {
            get
            {
                if (this.RelativeUrl != null)
                    return this.RelativeUrl;
                if (this.LinkingUrl != null)
                    return this.LinkingUrl;
                if (this.CommonUrl != null)
                    return this.CommonUrl;

                return null;
            }
        }
        public bool IsComplete
        {
            get
            {
                if (CommonUrl == null || LinkingUrl == null || RelativeUrl == null)
                    return false;
                return true;
            }
        }
        #endregion




        #region Contructors
        public URLRelative() { }

        public URLRelative(string commonUrl, string linkingUrl, string relativeUrl)
        {
            this.CommonUrl = commonUrl;
            this.LinkingUrl = linkingUrl;
            this.RelativeUrl = relativeUrl;
        }
        public URLRelative(URLRelative other)
        {
            this.CommonUrl = other.CommonUrl;
            this.LinkingUrl = other.LinkingUrl;
            this.RelativeUrl = other.RelativeUrl;
        }
        #endregion

        #region method
        public bool Exist()
        {
            DirectoryInfo CoreFactionDirectory = new DirectoryInfo(this.FullUrl);
            if (!CoreFactionDirectory.Exists)
                return false;
            return true;
        }

        public override string ToString()
        {
            return FullUrl;
        }

        public URLRelative CreateFromCommon(string RelativeUrl)
        {
            URLRelative NewOne = new URLRelative(this);
            NewOne.RelativeUrl = RelativeUrl;
            return NewOne;
        }
        #endregion

        #region static method
        public static List<string> CheckFileLinkingExist(string commonUrl, List<string> availableLink, string relativeUrl)
        {
            List<string> result = new List<string>();
            foreach (string link in availableLink)
            {
                FileInfo possibleFile = new FileInfo(Path.Combine(commonUrl, link, relativeUrl));
                if (possibleFile.Exists)
                    result.Add(link);

            }
            return result;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion

        #region Iequatable members
        public override bool Equals(object other)
        {
            if (other == null) return false;
            URLRelative objAsURL = other as URLRelative;
            if (objAsURL == null) return false;
            else return Equals(objAsURL);
        }
        public bool Equals(URLRelative other)
        {
            if (other == null) return false;
            return (this.CommonUrl == other.CommonUrl && this.LinkingUrl == other.LinkingUrl && this.RelativeUrl == other.RelativeUrl);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
