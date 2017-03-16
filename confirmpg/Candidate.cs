using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace confirmpg
{
    public class Candidate
    {
        public double TotalProbability { get; set; }
        public string Sentence { get; set; }
        public Candidate(double tb,string st)
        {
            this.TotalProbability = tb;
            this.Sentence = st;
        }
    }
}
