using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    class SSFileGroupFactory
    {
        public SSFileGroupFactory()
        {

        }
        
        public ISSGroup CreateGroupFromFile(ISSMergable file)
        {
            if(file is SSFaction f)
            {
                SSFactionGroup newFGroup = new SSFactionGroup();
                newFGroup.Add(f);
                return newFGroup as ISSGroup;
            }
            if(file is SSJson gf)
            {
                SSJsonGroup newgfGroup = new SSJsonGroup();
                newgfGroup.Add(gf);
                return newgfGroup as ISSGroup;
            }
            if(file is SSCsv fc)
            {
                SSCsvGroup newCsvGroup = new SSCsvGroup();
                newCsvGroup.Add(fc);
                return newCsvGroup as ISSGroup;
            }

            return null;
        }
    }
}
