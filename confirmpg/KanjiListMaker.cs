using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace confirmpg
{
    public static class KanjiListMaker
    {
        public static void GenerateKanjiList()
        {
            using (StreamReader sr = new StreamReader(@"C:\Users\rei\Desktop\workspace\0111marge.txt", Encoding.Default))
            {
                string line = "";
                int orderType = 0;
                int lengthType = 0;
                var kanjiList = new List<KanjiForCount>();
                while ((line = sr.ReadLine()) != null)
                {
                    string myouji = line.Split(' ')[0];
                    string namae = line.Split(' ')[1];
                    for (int i = 0; i < myouji.Length; i++)
                    {
                        if (i == 0)
                        {
                            orderType = 0;
                        }
                        else if (i == myouji.Length - 1)
                        {
                            orderType = 2;
                        }
                        else
                        {
                            orderType = 1;
                        }

                        lengthType = Math.Min(myouji.Length - 1, 3);
                        var settingKanji = myouji[i].ToString();
                        if (kanjiList.FirstOrDefault(kanji => kanji.CharKanji == settingKanji) == null)
                        {
                            kanjiList.Add(new KanjiForCount(settingKanji));
                        }

                        kanjiList.FirstOrDefault(kanji => kanji.CharKanji == settingKanji).setCounts(orderType, lengthType);
                    }
                    for (int i = 0; i < namae.Length; i++)
                    {
                        if (i == namae.Length - 1)
                        {
                            orderType = 5;
                        }
                        else if (i == 0)
                        {
                            orderType = 3;
                        }
                        else
                        {
                            orderType = 4;
                        }
                        lengthType = Math.Min(namae.Length + 3, 7);
                        var settingKanji = namae[i].ToString();
                        if (kanjiList.FirstOrDefault(kanji => kanji.CharKanji == settingKanji) == null)
                        {
                            kanjiList.Add(new KanjiForCount(settingKanji));
                        }
                        kanjiList.FirstOrDefault(kanji => kanji.CharKanji == settingKanji).setCounts(orderType, lengthType);
                    }
                }
                kanjiList.Sort((a, b) => b.Total - a.Total);
                using (StreamWriter sw = new StreamWriter(@"C: \Users\rei\Desktop\workspace\KanjiListTest.txt"))
                {
                    foreach (KanjiForCount kanji in kanjiList)
                    {
                        sw.WriteLine(kanji.ToString());
                    }
                }
            }

        }
    }
}
