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

        public ISSGroup CreateGroupFromFile(ISSGenericFile file)
        {
            switch (file)
            {
                case SSFaction f:
                    SSFactionGroup newFGroup = new SSFactionGroup();
                    newFGroup.Add(f);
                    return newFGroup;
                case SSJson gf:
                    SSJsonGroup newgfGroup = new SSJsonGroup();
                    newgfGroup.Add(gf);
                    return newgfGroup;
                case SSCsv fc:
                    return new SSCsvGroup(fc);
                case SSBinary b:
                    return new SSBinaryGroup(b);
            }
            return null;
        }
    }
}
