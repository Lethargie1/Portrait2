﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    abstract class SSUrl
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public override abstract string ToString();
    }

    class SSBaseUrl : SSUrl, ISSBase, IEquatable<SSBaseUrl>
    {
        public string Base { get; set; }

        public SSBaseUrl(string common) => Base = common;

        public bool Equals(SSBaseUrl other)
        {
            string otherBase = other?.Base;
            return otherBase != null && this.Base != null ? otherBase == this.Base : false;

        }
        public override string ToString()
        {
            if (this.Base == null)
                return null;
            string result = this.Base;
            return result;
        }

        public static SSBaseLinkUrl operator +(SSBaseUrl common,SSLinkUrl link)
        {
            SSBaseLinkUrl baseLink = new SSBaseLinkUrl();
            baseLink.Base = common.Base;
            baseLink.Link = link.Link;
            return baseLink;
        }
        public static SSFullUrl operator +(SSBaseUrl common, SSLinkRelativeUrl linkRela)
        {
            SSFullUrl full = new SSFullUrl();
            full.Base = common.Base;
            full.Link = linkRela.Link;
            full.Relative = linkRela.Relative;
            return full;
        }
    }

    class SSLinkUrl : SSUrl, IEquatable<SSLinkUrl>
    {
        public string Link { get; set; }

        public SSLinkUrl(string link) => Link = link;

        public bool Equals(SSLinkUrl other)
        {
            string otherLink = other?.Link;
            return otherLink != null && this.Link != null ? otherLink == this.Link : false;
        }
        public override string ToString()
        {
            if (this.Link == null)
                return null;
            string result = this.Link;
            return result;
        }

        public static SSLinkRelativeUrl operator +(SSLinkUrl link, SSRelativeUrl relative)
        {
            SSLinkRelativeUrl LinkRela = new SSLinkRelativeUrl();
            LinkRela.Relative = relative.Relative;
            LinkRela.Link = link.Link;
            return LinkRela;
        }
    }
    class SSRelativeUrl : SSUrl, IEquatable<SSRelativeUrl>
    {
        public string Relative { get; set; }

        public SSRelativeUrl(string relative) => Relative = relative;

        public bool Equals(SSRelativeUrl other)
        {
            string otherRelative = other?.Relative;
            return otherRelative != null && this.Relative != null ? otherRelative == this.Relative : false;
        }
        public override string ToString()
        {
            if (this.Relative == null)
                return null;
            string result = this.Relative;
            return result;
        }
    }

    class SSLinkRelativeUrl : SSUrl
    {
        public string Link { get; set; }
        public string Relative { get; set; }

        public override string ToString()
        {
            if (this.Link == null)
                return null;
            string result = this.Link;
            if (this.Relative != null)
                result = Path.Combine(result, this.Relative);
            return result;
        }
    }

    class SSBaseLinkUrl : SSUrl
        {
        public string Base { get; set; }
        public string Link { get; set; }

        public override string ToString()
        {
            if (this.Base == null)
                return null;
            string result = this.Base;
            if (this.Link != null)
                result = Path.Combine(result, this.Link);
            return result;
        }

        public static SSFullUrl operator +(SSBaseLinkUrl baseLink, SSRelativeUrl rela)
        {
            SSFullUrl full = new SSFullUrl();
            full.Base = baseLink.Base;
            full.Link = baseLink.Link;
            full.Relative = rela.Relative;
            return full;
        }
    }

    class SSFullUrl : SSUrl
    {
        public string Base { get; set; }
        public string Link { get; set; }
        public string Relative { get; set; }

        public override string ToString()
        {
            if (this.Base == null)
                return null;
            string result = this.Base;
            if (this.Link != null)
                result = Path.Combine(result, this.Link);
            if (this.Relative != null)
                result = Path.Combine(result, this.Relative);
            return result;
        }
    }

}
