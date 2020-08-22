using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensusManager.model
{
        [Serializable]
    public class Village
    {
        public Village(string guid, string name)
        {
            this.guid = guid;
            this.name = name;
        }
        public string guid;
        public string name;


        public override string ToString()
        {
            return  name;
        }
        
    }
}
