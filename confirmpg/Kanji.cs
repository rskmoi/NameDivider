using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace confirmpg
{
    public class Kanji
    {
        public string CharKanji { get; set; }

        public int[] OrderCount { get; set; }

        public int[] LengthCount { get; set; }

        public Kanji(string kanji)
        {
            this.CharKanji = kanji;
            this.OrderCount = new int[6];
            this.LengthCount = new int[8];
        }

        public override string ToString()
        {
            return CharKanji;
        }
    }
}
