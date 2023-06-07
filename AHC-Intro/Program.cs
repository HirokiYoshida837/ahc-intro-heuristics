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

            var outputList = Solve(input);

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
        public static int[] Solve(Input input)
        {

            var ansList = new List<int>();
            
            for (var i = 0; i < input.s.Length; i++)
            {
                var valueTuple = input.s[i].Select((item,index) => (item,index))
                    .OrderByDescending(x=>x.item)
                    .First();

                ansList.Add(valueTuple.index + 1);
            }
            
            
            // var ansList = Enumerable.Range(0, input.d)
            //     .Select(x => (x % 26) + 1)
            //     .ToArray();
            //

            // 返却前に一応値チェックしておく。
            if (ansList.Any(x => (x < 0 || 26 < x)))
            {
                throw new Exception("contest type error.");
            }

            return ansList.ToArray();
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
            // var contestHistory = Enumerable.Range(0, 26)
            //     .Select(x => new List<int>() {0})
            //     .ToArray();
            //
            // for (var i = 0; i < answerList.Length; i++)
            // {
            //     var todayContest = answerList[i];
            //     contestHistory[todayContest].Add(i + 1);
            // }

            // 0-indexな日付に変更
            var contestHistoryAns = answerList.Select(x => x - 1).ToArray();

            // 1日目からd日目まで、計算
            var scoreHistory = Enumerable.Range(0, input.d)
                .Select(_ => new long[26])
                .ToArray();

            // 最後に開催された日付け。随時更新。
            var contestLastHeld = Enumerable.Range(0, 26).Select(_ => -1).ToArray();

            var currentScore = 0L;
            for (int i = 0; i < input.d; i++)
            {
                var contestType = contestHistoryAns[i];

                var array = contestLastHeld
                    .Select((item, index) => (item, index))
                    .Select(x => (i - x.item, x.index))
                    .Select(x => -1 * x.Item1 * input.c[x.index])
                    .ToArray();

                array[contestType] = input.s[i][contestType];

                // なにかに使うかもしれないので、点数推移を持っておく。
                for (var i1 = 0; i1 < array.Length; i1++)
                {
                    scoreHistory[i][i1] = array[i1];
                }

                currentScore += array.Sum();

                // 出力
                Console.WriteLine(currentScore);

                // 最後に開催された日付を更新
                contestLastHeld[contestType] = i;
            }

            // 最終スコアを返す。
            var lastSum = scoreHistory.Select(x => x.Sum())
                .Sum();


            Debug.WriteLine($"[DEBUG] ### Last Score : {lastSum} ### ");

            // 返却するのは、0かlastSumか
            return Math.Max(0, lastSum);
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