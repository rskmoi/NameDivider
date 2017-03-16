using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace confirmpg
{
    public class NameWithCertainlyFactor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double CertainlyFactor { get; set; }
        public NameWithCertainlyFactor(int id,string name,double certainlyFactor)
        {
            this.Id = id;
            this.Name = name;
            this.CertainlyFactor = certainlyFactor;
        }
        public override string ToString()
        {
            return $"{Id}\t{Name}";
        }
    }
}
