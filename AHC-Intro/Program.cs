using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Math;

namespace AHC_Intro
{
    public static class Program
    {
        // スコアを自分で計算する場合は true に設定
        public static readonly bool IS_DEBUG_ENABLED = false;

        public static void Main(string[] args)
        {
            // Read Input
            var d = ReadValue<int>();
            var cList = ReadList<long>().ToArray();
            var sList = Enumerable.Range(0, d)
                .Select(_ => ReadList<long>().ToArray())
                .ToArray();

            var input = new Input(d, cList, sList);

            // var outputList = new SimpleGreedySolver().Solve(input);
            var outputList = new EditorialGreedySolver().Solve(input);

            // var outputList = Enumerable.Range(0, d)
            //     .Select(_ => ReadValue<int>())
            //     .ToArray();

            foreach (var item in outputList)
            {
                Console.WriteLine(item);
            }

            if (IS_DEBUG_ENABLED)
            {
                var sum = CalculateScoreSum(input, outputList);
            }
        }

        /// <summary>
        /// 答えとして出力する、コンテスト開催タイプ(1-indexed)のリストを返却します。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public interface ISolver
        {
            int[] Solve(Input input);
        }

        public static void ValidateAnsArray(Input input, int[] ansList)
        {
            // 返却前に一応値チェックしておく。
            if (ansList.Any(x => (x < 1 || 26 < x)))
            {
                throw new Exception("contest type error.");
            }

            if (ansList.Length != input.d)
            {
                throw new Exception("anser length error.");
            }
        }

        public class EditorialGreedySolver : ISolver
        {
            public int[] Solve(Input input)
            {
                var ansList = new List<int>();

                for (int day = 0; day < input.d; day++)
                {
                    var currentMaxScore = long.MinValue;
                    var bestContestType = 0;

                    for (int type = 0; type < 26; type++)
                    {
                        ansList.Add(type);

                        var score = CalculateScoreSum(input, ansList.Select(x => x + 1).ToArray());

                        if (currentMaxScore < score)
                        {
                            currentMaxScore = score;
                            bestContestType = type;
                        }

                        ansList.RemoveAt(ansList.Count - 1);
                    }

                    ansList.Add(bestContestType);
                }


                var ret = ansList.Select(x => x + 1).ToArray();

                ValidateAnsArray(input, ret);
                return ret;
            }
        }


        public class SimpleGreedySolver : ISolver
        {
            public int[] Solve(Input input)
            {
                var ansList = new List<int>();

                // 1日目からd日目まで、計算
                var scoreHistory = Enumerable.Range(0, input.d)
                    .Select(_ => new long[26])
                    .ToArray();

                var lastHeldDate = Enumerable.Range(0, 26)
                    .Select(_ => -1)
                    .ToArray();

                for (var i = 0; i < input.s.Length; i++)
                {
                    // 今日のスコアの元

                    var todayScoreBase = lastHeldDate
                        .Select((item, index) => (item, index))
                        .Select(x => (i - x.item, x.index))
                        .Select(x => -1 * x.Item1 * input.c[x.index])
                        .ToArray();

                    var todaySum = todayScoreBase.Sum();
                    var todayS = input.s[i];

                    var maxScore = long.MinValue;
                    var selectedType = 0;

                    // Greedyにやる。26種類それぞれためして、一番スコアが高くなる種別を選ぶ。
                    for (int j = 0; j < 26; j++)
                    {
                        var v = todaySum + (-1 * todayScoreBase[j]);
                        v += todayS[j];

                        if (maxScore <= v)
                        {
                            selectedType = j;
                            maxScore = v;
                        }
                    }

                    // その日選んだものを保存。
                    ansList.Add(selectedType);

                    // 最後の開催日データを更新
                    lastHeldDate[selectedType] = i;
                }

                // ansListを1-indexedな日付に変更

                ansList = ansList.Select(x => x + 1).ToList();


                // 返却前に一応値チェックしておく。
                if (ansList.Any(x => (x < 1 || 26 < x)))
                {
                    throw new Exception("contest type error.");
                }

                if (ansList.Count != input.d)
                {
                    throw new Exception("anser length error.");
                }

                return ansList.ToArray();
            }
        }

        public struct Input
        {
            // 開催される日数
            public int d { get; }

            // コンテストが開かれなかったときのscoreの下がりやすさ
            public long[] c { get; }

            // i日目にコンテストが開かれたときに得られるscore
            public long[][] s { get; }

            public Input(int d, long[] c, long[][] s)
            {
                this.d = d;
                this.c = c;
                this.s = s;
            }
        }

        public static long CalculateScoreSum(Input input, int[] answerList)
        {
            // 扱いやすいように0-indexな日付に変更
            var contestHistoryAns = answerList.Select(x => x - 1).ToArray();

            // 最後に開催された日付け。随時更新。
            var contestLastHeld = Enumerable.Range(0, 26).Select(_ => -1).ToArray();

            var currentScore = 0L;
            
            for (int days = 0; days < answerList.Length; days++)
            {
                var contestType = contestHistoryAns[days];
                contestLastHeld[contestType] = days;

                for (int i = 0; i < 26; i++)
                {
                    currentScore -= input.c[i] * (days - contestLastHeld[i]);
                }

                currentScore += input.s[days][contestType];

            }

            Debug.WriteLine($"[DEBUG] ### Last Score : {currentScore} ### ");
            return currentScore;
        }


        public static T ReadValue<T>()
        {
            var input = Console.ReadLine();
            return (T) Convert.ChangeType(input, typeof(T));
        }

        public static (T1, T2) ReadValue<T1, T2>()
        {
            var input = Console.ReadLine().Split();
            return (
                (T1) Convert.ChangeType(input[0], typeof(T1)),
                (T2) Convert.ChangeType(input[1], typeof(T2))
            );
        }

        public static (T1, T2, T3) ReadValue<T1, T2, T3>()
        {
            var input = Console.ReadLine().Split();
            return (
                (T1) Convert.ChangeType(input[0], typeof(T1)),
                (T2) Convert.ChangeType(input[1], typeof(T2)),
                (T3) Convert.ChangeType(input[2], typeof(T3))
            );
        }

        /// <summary>
        /// 指定した型として、一行読み込む。
        /// </summary>
        /// <param name="separator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
#nullable enable
        public static IEnumerable<T> ReadList<T>(params char[]? separator)
        {
            return Console.ReadLine()
                .Split(separator)
                .Select(x => (T) Convert.ChangeType(x, typeof(T)));
        }
#nullable disable
    }
}