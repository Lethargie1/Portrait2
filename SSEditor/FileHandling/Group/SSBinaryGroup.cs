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
        public SSFullUrl FinalPath { get; private set; }
        public SSBinary FinalFile { get; private set; }
        public void RecalculateFinal()
        {
            var OrderedFile = from f in base.CommonFiles
                              orderby f.SourceMod.ModName
                              select f;
            FinalFile = OrderedFile.FirstOrDefault();
            FinalPath = FinalFile.SourceMod.ModUrl + FinalFile.RelativeUrl; ;
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
