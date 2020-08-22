using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CensusManager.model
{
    [Serializable]
    public class Person
    {
        public string relation;
        public string name;
        public string id;
        public string race;
        public string address;

        public Person(string relation, string name, string id, string race, string address)
        {
            this.relation = relation;
            this.name = name;
            this.id = id;
            this.race = race;
            this.address = address;
        }

        public override string ToString()
        {
            return String.Concat("关系：",relation,"姓名：",name,"身份证：",id,"名族：",race,"住址：",address);
        }
    }
}
