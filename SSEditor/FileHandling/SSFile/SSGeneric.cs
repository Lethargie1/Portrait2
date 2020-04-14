using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public abstract class SSGeneric : ISSGenericFile
    {
        public SSLinkRelativeUrl LinkRelativeUrl { get; protected set; }
        public string FileName { get; protected set; }
        public SSMod SourceMod { get; protected set; }

        public SSGeneric(SSMod mod, SSFullUrl url)
        {
            SourceMod = mod;
            LinkRelativeUrl = url.LinkRelative;
            FileInfo info = new FileInfo(url.ToString());
            FileName = info.Name ?? throw new ArgumentNullException("The FileName cannot be null.");
        }

        public void CopyTo(SSBaseLinkUrl newPath)
        {
            SSBaseUrl InstallationUrl = new SSBaseUrl(newPath.Base);
            SSFullUrl SourceUrl = InstallationUrl + this.LinkRelativeUrl;

            SSFullUrl TargetUrl = newPath + this.LinkRelativeUrl.GetRelative();

            FileInfo sourceInfo = new FileInfo(SourceUrl.ToString());
            FileInfo targetInfo = new FileInfo(TargetUrl.ToString());
            if (targetInfo.Exists)
            {
              targetInfo = new FileInfo(TargetUrl.ToString()+SourceMod.ModName);
            }
            DirectoryInfo targetDir = targetInfo.Directory;
            if (!targetDir.Exists)
            {
                targetDir.Create();
            }
            sourceInfo.CopyTo(targetInfo.FullName);
        }

        public override string ToString()
        {
            return FileName + " from " + SourceMod.ModName;
        }
    }
    
}
