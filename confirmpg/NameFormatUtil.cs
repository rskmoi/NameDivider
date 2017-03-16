using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace confirmpg
{
    public class NameFormatUtil
    {
        public static bool isInvalidName(string fullName)
        {
            if (fullName.Length == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int isMixingKana(string fullName)
        {
            string firstKanaType = getKanaType(fullName[0]);
            for (int i = 1; i < fullName.Length - 1; i++)
            {
                if (getKanaType(fullName[i]) != firstKanaType && getKanaType(fullName[i + 1]) != firstKanaType)
                {
                    return i;
                }
            }
            return 0;
        }

        private static string getKanaType(char c)
        {
            if (c.ToString() == "ヶ")
            {
                return "Kanji";
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(c.ToString(), @"^\p{IsHiragana}+$"))
            {
                return "Hiragana";
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(c.ToString(), @"^[\p{IsKatakana}\u31F0-\u31FF\u3099-\u309C\uFF65-\uFF9F]+$"))
            {
                return "Katakana";
            }
            else
            {
                return "Kanji";
            }
        }
    }
}
