using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public class SSBinaryGroup : SSGroup<SSBinary>
    {
        public string FinalSourcePath { get; private set; }

        public void RecalculateFinal()
        {
            var OrderedPath = from f in base.CommonFiles
                               orderby f.SourceMod.ModName
                               select  f.SourceMod.ModUrl + f.RelativeUrl;

            SSFullUrl FinalPath = OrderedPath.FirstOrDefault();
            FinalSourcePath = FinalPath.ToString().Replace('/', '\\');
        }
        public SSBinaryGroup():base()
        { }
        public SSBinaryGroup(SSBinary firstfile):base()
        {
            this.Add(firstfile);
        }

        public override void WriteTo(SSBaseLinkUrl newPath)
        {
            throw new NotImplementedException();
        }
    }
}
