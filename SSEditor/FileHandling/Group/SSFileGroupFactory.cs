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
        
        public ISSFileGroup CreateGroupFromFile(ISSGenericFile file)
        {
            if(file is SSFactionFile f)
            {
                SSFactionGroup newFGroup = new SSFactionGroup();
                newFGroup.Add(f);
                return newFGroup;
            }
            if(file is SSFile gf)
            {
                SSFileGroup<SSFile> newgfGroup = new SSFileGroup<SSFile>();
                newgfGroup.Add(gf);
                return newgfGroup;
            }
            if(file is SSFileCsv fc)
            {
                SSFileCsvGroup newCsvGroup = new SSFileCsvGroup();
                newCsvGroup.Add(fc);
                return newCsvGroup;
            }
            SSFileGroup<ISSGenericFile> newGroup = new SSFileGroup<ISSGenericFile>();
            newGroup.Add(file);
            return newGroup;
        }
    }
}
