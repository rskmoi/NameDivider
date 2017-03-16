using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace confirmpg
{
    public class KanjiForCount : Kanji
    {
        public int Total { get; set; }
        public KanjiForCount(string kanji) : base(kanji)
        {
        }

        public void setCounts(int orderType, int lengthType)
        {
            this.OrderCount[orderType] += 1;
            this.LengthCount[lengthType] += 1;
            Total += 1;
        }

        public override string ToString()
        {
            return $"{this.CharKanji},{this.OrderCount[0]},{this.OrderCount[1]},{this.OrderCount[2]},{this.OrderCount[3]},{this.OrderCount[4]},{this.OrderCount[5]},{this.LengthCount[0]},{this.LengthCount[1]},{this.LengthCount[2]},{this.LengthCount[3]},{this.LengthCount[4]},{this.LengthCount[5]},{this.LengthCount[6]},{this.LengthCount[7]}";
        }
    }
}
