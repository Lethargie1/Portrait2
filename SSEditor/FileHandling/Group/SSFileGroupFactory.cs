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
        
        public SSFileGroup CreateGroupFromFile(ISSGenericFile file)
        {
            if(file is SSFactionFile f)
            {
                SSFactionGroup newFGroup = new SSFactionGroup();
                newFGroup.Add(f);
                return newFGroup;
            }
            SSFileGroup newGroup = new SSFileGroup();
            newGroup.Add(file);
            return newGroup;
        }
    }
}
