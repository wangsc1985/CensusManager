using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensusManager.model
{
    [Serializable]
    public class Build
    {
        public Build(string guid,string mid, string number, string villageGuid)
        {
            this.guid = guid;
            this.number = number;
            this.mid = mid;
            this.villageGuid = villageGuid;
        }
        public string guid;
        public string mid;
        public string number;
        public string villageGuid;

        public override string ToString()
        {
            return number;
        }
    }
}
