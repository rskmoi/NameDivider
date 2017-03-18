using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace confirmpg
{
    public class NameDivider
    {
        public List<Kanji> kanjiList { get; set; }

        public NameDivider(List<Kanji> mainKanjiList)
        {
            kanjiList = mainKanjiList;
        }
        //区切れる個所すべてで区切った処理済みの名前を返すメソッド
        public List<Candidate> getNameCandidate(string fullName)
        {
            //不正な名前だったらnullを返す
            if (NameFormatUtil.isInvalidName(fullName))
            {
                return null;
            }
            //漢字かな（カナ）混合だったら確信度1で即決
            int mixingPoint = NameFormatUtil.isMixingKana(fullName);
            if (mixingPoint != 0)
            {
                string myouji = fullName.Substring(0, mixingPoint);
                string namae = fullName.Substring(mixingPoint);
                return new List<Candidate>() { new Candidate(1, $"{myouji} {namae}") };
            }
            //以下、漢字のみで構成された名前の処理
            List<NameData> avarageProbabilities = getNameDataList(fullName);
            List<Candidate> candidate = getTotalProbability(avarageProbabilities);
            //一番確率の高いものをリストの先頭に持ってくる
            candidate.Sort((a, b) => (int)(b.TotalProbability * 100000) - (int)(a.TotalProbability * 100000));
            return candidate;
        }

        /// <summary>
        /// NameData（特徴量込みの名前）のリストを返すメソッド
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        private List<NameData> getNameDataList(string fullName)
        {
            List<NameData> probabilitiesList = new List<NameData>();
            int fullNameLength = fullName.Length;
            for (int i = 1; i < fullNameLength; i++)
            {
                string myouji = fullName.Substring(0, i);
                string namae = fullName.Substring(i);
                double[] myoujiProbability = calcurateMyoujiProbability(myouji, fullNameLength);
                double[] namaeProbability = calcurateNamaeProbability(namae, fullNameLength);
                double sumOrderProbability = myoujiProbability[0] + namaeProbability[0];
                double sumLengthProbability = myoujiProbability[1] + namaeProbability[1];
                double averageOrderProbability = ((fullNameLength - 2) != 0) ? sumOrderProbability / (fullNameLength - 2) : 0;
                double averageLengthProbability = sumLengthProbability / fullNameLength;
                NameData addClass = new NameData($"{myouji} {namae}", averageOrderProbability, averageLengthProbability);
                probabilitiesList.Add(addClass);
            }
            return probabilitiesList;
        }

        /// <summary>
        /// 特徴量から確率を計算して候補リストを返すメソッド
        /// </summary>
        /// <param name="avarageProbabilitiesList"></param>
        /// <returns></returns>
        private static List<Candidate> getTotalProbability(List<NameData> avarageProbabilitiesList)
        {
            List<Candidate> candidateList = new List<Candidate>();
            foreach (NameData avarageProbabilities in avarageProbabilitiesList)
            {
                double total = calcurateTotalProbability(avarageProbabilities.OrderProbability, avarageProbabilities.LengthProbability);
                Candidate candidate = new Candidate(total, avarageProbabilities.FullName);
                candidateList.Add(candidate);
            }
            return candidateList;
        }

        private double[] calcurateMyoujiProbability(string myouji, int fullNameLength)
        {
            double sumOrderProbability = 0;
            double sumLengthProbability = 0;
            for (int i = 0; i < myouji.Length; i++)
            {
                int order;
                if (i == 0)
                {
                    order = 0;
                }
                else if (i == myouji.Length - 1)
                {
                    order = 2;
                }
                else
                {
                    order = 1;
                }
                Kanji searchingMyoujiKanji = searchKanji(myouji[i].ToString());
                if (searchingMyoujiKanji != null)
                {
                    int myoujiLengthType = (myouji.Length <= 3) ? myouji.Length - 1 : 3;
                    double debugOdouble = calculateOrderProbability(searchingMyoujiKanji, i, fullNameLength, order);
                    sumOrderProbability += debugOdouble;
                    double debugLdouble = calculateLengthProbability(searchingMyoujiKanji,i, fullNameLength, myoujiLengthType);
                    sumLengthProbability += debugLdouble;
                }
            }
            double[] sumProbability = { sumOrderProbability, sumLengthProbability };
            return sumProbability;
        }

        private double[] calcurateNamaeProbability(string namae, int fullNameLength)
        {
            double sumOrderProbability = 0;
            double sumLengthProbability = 0;
            for (int i = 0; i < namae.Length; i++)
            {
                int order;
                if (i == 0)
                {
                    order = 3;
                }
                else if (i == namae.Length - 1)
                {
                    order = 5;
                }
                else
                {
                    order = 4;
                }
                Kanji searchingNamaeKanji = searchKanji(namae[i].ToString());
                if (searchingNamaeKanji != null)
                {
                    int namaeLengthType = (namae.Length <= 3) ? namae.Length + 3 : 7;
                    double debugOdouble = calculateOrderProbability(searchingNamaeKanji, fullNameLength - namae.Length + i, fullNameLength, order);
                    sumOrderProbability += debugOdouble;
                    double debugLdouble = calculateLengthProbability(searchingNamaeKanji, fullNameLength - namae.Length + i, fullNameLength, namaeLengthType);
                    sumLengthProbability += debugLdouble;
                }
            }
            double[] sumProbability = { sumOrderProbability, sumLengthProbability };
            return sumProbability;
        }
        public static double calculateOrderProbability(Kanji kanji, int order, int length, int real)
        {
            if (order == 0)
            {
                return 0;
            }
            else if (order == length - 1)
            {
                return 0;
            }
            else if (order == 1)
            {
                if (length == 3)
                {
                    int denomitor = (kanji.OrderCount[2] + kanji.OrderCount[3]);
                    return (denomitor != 0) ? ((double)(kanji.OrderCount[real]) / denomitor) : 0;
                }
                else
                {
                    int denomitor = kanji.OrderCount[1] + kanji.OrderCount[2] + kanji.OrderCount[3];
                    return (denomitor != 0) ? ((double)(kanji.OrderCount[real]) / denomitor) : 0;

                }
            }
            else if (order == length - 2)
            {
                int denomitor = kanji.OrderCount[2] + kanji.OrderCount[3] + kanji.OrderCount[4];
                return (denomitor != 0) ? ((double)(kanji.OrderCount[real]) / denomitor) : 0;
            }
            else
            {
                int denomitor = kanji.OrderCount[1] + kanji.OrderCount[2] + kanji.OrderCount[3] + kanji.OrderCount[4];
                return (denomitor != 0) ? ((double)(kanji.OrderCount[real]) / denomitor) : 0;
            }
        }

        public static double calculateLengthProbability(Kanji kanji, int order, int length, int real)
        {
            int possibleNum = Math.Min(length - 1, 4);
            int normalOrder = Math.Min(order, 3);
            int denominator = 0;
            if (order != length - 1)
            {
                for (int i = normalOrder; i < possibleNum; i++)
                {
                    denominator += kanji.LengthCount[i];
                }
            }
            if (order != 0)
            {
                int reverseOrder = Math.Min(length - order - 1, 3);
                for (int i = reverseOrder + 4; i < possibleNum + 4; i++)
                {
                    denominator += kanji.LengthCount[i];
                }
            }
            double retval = (denominator != 0) ? (double)(kanji.LengthCount[real]) / denominator : 0;
            return retval;
        }

        public static double calcurateTotalProbability(double op, double lp)
        {
            //double x = Math.Exp(op * 10.251) + Math.Exp(lp * 5.311) + Math.Exp(-7.477);
            //double x = Math.Exp(8.489 * (op + lp)) + Math.Exp(3.006 * Math.Pow(op - lp, 2)) + Math.Exp(- 7.498);
            double x = Math.Exp(8.567 * (op + lp)) + Math.Exp(2.656 * Math.Pow(op - lp, 2)) + Math.Exp(- 7.701);
            return x / (x + 1);
        }
        public Kanji searchKanji(string moji)
        {
            return kanjiList.FirstOrDefault(kanji => kanji.CharKanji == moji);
        }
    }
}
