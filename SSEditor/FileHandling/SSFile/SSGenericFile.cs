using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public class SSGenericFile : ISSGenericFile
    {
        public SSLinkRelativeUrl LinkRelativeUrl { get; protected set; }
        public string FileName { get; protected set; }
        public SSMod SourceMod { get; protected set; }

        public SSGenericFile(SSMod mod, SSFullUrl url)
        {
            SourceMod = mod;
            LinkRelativeUrl = url.LinkRelative;
        }

        public void CopyTo(SSBaseLinkUrl newPath)
        {
            SSBaseUrl InstallationUrl = new SSBaseUrl(newPath.Base);
            SSFullUrl SourceUrl = InstallationUrl + this.LinkRelativeUrl;

            SSFullUrl TargetUrl = newPath + this.LinkRelativeUrl.GetRelative();

            FileInfo sourceInfo = new FileInfo(SourceUrl.ToString());
            FileInfo targetInfo = new FileInfo(TargetUrl.ToString());
            DirectoryInfo targetDir = targetInfo.Directory;
            if (!targetDir.Exists)
            {
                targetDir.Create();
            }
            sourceInfo.CopyTo(targetInfo.FullName);
        }
    }
    public interface ISSGenericFile
    {
        SSLinkRelativeUrl LinkRelativeUrl { get; }
        string FileName { get; }
        SSMod SourceMod { get; }
        void CopyTo(SSBaseLinkUrl NewPath);
    }
}
