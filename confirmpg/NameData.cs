using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace confirmpg
{
    public class NameData
    {
        public double OrderProbability { get; set; }
        public double LengthProbability { get; set; }
        public string FullName { get; set; }
        public NameData(string fullName,double op,double lp)
        {
            this.FullName = fullName;
            this.OrderProbability = op;
            this.LengthProbability = lp;
        }
        public override string ToString()
        {
            return $"{this.FullName},{this.OrderProbability},{this.LengthProbability}";
        }
    }
}
