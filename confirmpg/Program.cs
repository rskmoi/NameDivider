using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace confirmpg
{
    public class Program
    {
        public static List<Kanji> kanjiList { get; set; }
        public static List<NameWithCertainlyFactor> NWCFList { get; set; }
        public static StringBuilder StudySb = new StringBuilder();
        public static double Coeffcient { get; set; }

        public static void Main(string[] args)
        {
            kanjiList = readKanjiList();
            Console.WriteLine("+++++姓名分離くん START+++++");
            Help();
            bool IsActive = true;
            while (IsActive)
            {
                Console.WriteLine("メインメニューです。");
                Console.Write(">");
                string firstCommand = Console.ReadLine();
                if (firstCommand == "test")
                {
                    DoTest();
                }
                else if (firstCommand == "divide")
                {
                    DivideNameFromText();
                }
                else if (firstCommand == "divide -c")
                {
                    DivideNameFromTextOrderByCertaintyFactor();
                }
                else if (firstCommand == "mklist")
                {
                    KanjiListMaker.GenerateKanjiList();
                    readKanjiList();
                }
                else if(firstCommand == "accuracy")
                {
                    CheckAccuracy();
                }
                else if (firstCommand == "help")
                {
                    Help();
                }
                else if (firstCommand == "quit")
                {
                    IsActive = false;
                }
                else
                {
                    Console.WriteLine("not command");
                }
            }
        }

        private static void Help()
        {
            Console.WriteLine("test:試しにコンソール上で姓名分離");
            Console.WriteLine("divide:テキストファイルにある名前を姓名分離");
            Console.WriteLine("divide -c:テキストファイルにある名前を姓名分離して確信度が低い順にソート");
            Console.WriteLine("help:ヘルプ");
            Console.WriteLine("quit:アプリケーションの終了");
        }

        static bool activateTestState = true;
        public static void DoTest()
        {
            NameDivider nameDivider = new NameDivider(kanjiList);
            activateTestState = true;
            while (activateTestState)
            {
                Console.WriteLine("お試しモードです。名前をコンソールに入力してください。");
                Console.Write(">");
                string name = Console.ReadLine();
                if (name == "quit")
                {
                    activateTestState = false;
                }
                else if (name == string.Empty)
                {
                }
                else
                {
                    if (nameDivider.getNameCandidate(name) != null)
                    {
                        Console.WriteLine(nameDivider.getNameCandidate(name)[0].Sentence);
                    }
                    else
                    {
                        Console.WriteLine("名前が正しくありません");
                    }
                }
            }
        }

        public static void CheckAccuracy()
        {
            NameDivider nameDivider = new NameDivider(kanjiList);
            Console.WriteLine("ファイルのパス名を入力してください。");
            Console.Write(">");
            string path = Console.ReadLine();
            if (path == "quit")
            {
                return;
            }
            else if (File.Exists(path) == false)
            {
                Console.WriteLine("ファイルを見つけられません。パス名が正しいか確認してください。");
                return;
            }

            string line = string.Empty;
            StringBuilder sb = new StringBuilder();
            int nameCount = 0;
            int mistakeCount = 0;
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    string noSpaceName = line.Replace(" ", "");
                    if (nameDivider.getNameCandidate(noSpaceName) == null)
                    {
                        Console.WriteLine($"{nameCount + 1}件目の名前が正しくありません。エラーが発生したため処理が完了されませんでした。");
                        return;
                    }

                    string dividedName = nameDivider.getNameCandidate(noSpaceName)[0].Sentence;
                    if (line == dividedName )
                    {
                        sb.AppendLine($"true,{dividedName}");
                    }
                    else
                    {
                        sb.AppendLine($"false,{dividedName}");
                        mistakeCount++;
                    }
                    nameCount++;
                }
                using (StreamWriter sw = new StreamWriter($".\\NameDivideResult{System.DateTime.Now.ToString("MMddhhmmss")}.txt"))
                {
                    sw.WriteLine(sb.ToString());
                }
                Console.WriteLine($"{nameCount}件の処理が正常に終了しました。間違えた数:{mistakeCount}正解率:{Math.Round((double)(nameCount - mistakeCount)/nameCount,3)}");
            }



        }
        public static void DivideNameFromText()
        {
            NameDivider nameDivider = new NameDivider(kanjiList);
            Console.WriteLine("ファイルのパス名を入力してください。");
            Console.Write(">");
            string path = Console.ReadLine();
            if (path == "quit")
            {
                return;
            }
            else if (File.Exists(path) == false)
            {
                Console.WriteLine("ファイルを見つけられません。パス名が正しいか確認してください。");
                return;
            }

            string line = string.Empty;
            StringBuilder sb = new StringBuilder();
            int nameCount = 0;

            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (nameDivider.getNameCandidate(line) != null)
                    {
                        sb.AppendLine(nameDivider.getNameCandidate(line)[0].Sentence);
                    }
                    else
                    {
                        Console.WriteLine($"{nameCount + 1}件目の名前が正しくありません。エラーが発生したため処理が完了されませんでした。");
                        return;
                    }
                    nameCount++;
                }
                using (StreamWriter sw = new StreamWriter($".\\NameDivideResult{System.DateTime.Now.ToString("MMddhhmmss")}.txt"))
                {
                    sw.WriteLine(sb.ToString());
                }
            }
            Console.WriteLine($"{nameCount}件の処理が正常に終了しました。");
        }

        public static void DivideNameFromTextOrderByCertaintyFactor()
        {
            NameDivider nameDivider = new NameDivider(kanjiList);
            NWCFList = new List<NameWithCertainlyFactor>();
            Console.WriteLine("ファイルのパス名を入力してください。");
            Console.Write(">");
            string path = Console.ReadLine();
            if (path == "quit")
            {
                return;
            }
            else if (File.Exists(path) == false)
            {
                Console.WriteLine("ファイルを見つけられません。パス名が正しいか確認してください。");
                return;
            }

            string line = string.Empty;
            StringBuilder sb = new StringBuilder();
            int nameCount = 0;

            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (nameDivider.getNameCandidate(line) != null)
                    {
                        setCertainlyFactor(nameDivider.getNameCandidate(line), nameCount + 1);
                    }
                    else
                    {
                        Console.WriteLine($"{nameCount + 1}件目の名前が正しくありません。エラーが発生したため処理が完了されませんでした。");
                        return;
                    }
                    nameCount++;
                }

                NWCFList.Sort((a, b) => (int)(a.CertainlyFactor * 100000) - (int)(b.CertainlyFactor * 100000));
                using (StreamWriter sw = new StreamWriter($".\\NameDivideResult{System.DateTime.Now.ToString("MMddhhmmss")}.txt"))
                {
                    foreach (NameWithCertainlyFactor nwcf in NWCFList)
                    {
                        sw.WriteLine(nwcf.ToString());
                    }
                }
            }
            Console.WriteLine($"{nameCount}件の処理が正常に終了しました。");
        }

        private static void setCertainlyFactor(List<Candidate> forsortList, int id)
        {
            NameWithCertainlyFactor nameWithCertainlyFactor;
            if (forsortList.Count() == 1)
            {
                nameWithCertainlyFactor = new NameWithCertainlyFactor(id, forsortList[0].Sentence, 1.0);
            }
            else
            {
                double certainFactor = forsortList[0].TotalProbability - forsortList[1].TotalProbability;
                nameWithCertainlyFactor = new NameWithCertainlyFactor(id, forsortList[0].Sentence, certainFactor);
            }
            NWCFList.Add(nameWithCertainlyFactor);
        }

        private static List<Kanji> readKanjiList()
        {
            using (StreamReader sr = new StreamReader(@".\\KanjiList.txt", Encoding.UTF8))
            {
                List<Kanji> kanjiList = new List<Kanji>();
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    string[] splitLine = line.Split(',');
                    Kanji kanji = new Kanji(splitLine[0]);
                    kanji.OrderCount[0] = Int32.Parse(splitLine[1]);
                    kanji.OrderCount[1] = Int32.Parse(splitLine[2]);
                    kanji.OrderCount[2] = Int32.Parse(splitLine[3]);
                    kanji.OrderCount[3] = Int32.Parse(splitLine[4]);
                    kanji.OrderCount[4] = Int32.Parse(splitLine[5]);
                    kanji.OrderCount[5] = Int32.Parse(splitLine[6]);
                    kanji.LengthCount[0] = Int32.Parse(splitLine[7]);
                    kanji.LengthCount[1] = Int32.Parse(splitLine[8]);
                    kanji.LengthCount[2] = Int32.Parse(splitLine[9]);
                    kanji.LengthCount[3] = Int32.Parse(splitLine[10]);
                    kanji.LengthCount[4] = Int32.Parse(splitLine[11]);
                    kanji.LengthCount[5] = Int32.Parse(splitLine[12]);
                    kanji.LengthCount[6] = Int32.Parse(splitLine[13]);
                    kanji.LengthCount[7] = Int32.Parse(splitLine[14]);
                    kanjiList.Add(kanji);
                }
                Console.WriteLine("漢字ロード完了");
                return kanjiList;
            }
        }
    }
}
