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
using AHC_Intro.Solver;
using AHC_Intro.Solver.Implementation;
using static System.Math;

namespace AHC_Intro
{
    public static class Program
    {
        // スコアを自分で計算する場合は true に設定
        public static readonly bool IS_DEBUG_ENABLED = false;

        public static void Main(string[] args)
        {
            if (IS_DEBUG_ENABLED)
            {
                Console.WriteLine("### [WARNING] DEBUG MODE IS ENABLE!! ###");
            }

            // Read Input
            var d = ReadValue<int>();
            var cList = ReadList<long>().ToArray();
            var sList = Enumerable.Range(0, d)
                .Select(_ => ReadList<long>().ToArray())
                .ToArray();

            var input = new Input {d = d, c = cList, s = sList};

            var solvedResult = SolveProblem(input);

            // 出力
            solvedResult.AnswerWrite();

            if (IS_DEBUG_ENABLED)
            {
                Console.WriteLine($"[DEBUG] ### Last Score : {solvedResult.lastScore} ### ");
            }
        }

        // 外部からInjectしてテストできるようにする。
        public static Response SolveProblem(Input input)
        {
            return SolveProblem(input, GetDefaultSolver());
        }

        // 外部からInjectしてテストできるようにする。
        public static Response SolveProblem(Input input, ISolver solver)
        {
            return solver.Solve(input);
        }

        // ここを変える。
        public static ISolver GetDefaultSolver()
        {
            // return new EditorialGreedySolver(10);
            return new EditorialClimbingSolver();
        }


        // public class SimpleGreedySolver : ISolver
        // {
        //     public Response Solve(Input input)
        //     {
        //         var ansList = new List<int>();
        //
        //         // 1日目からd日目まで、計算
        //         var scoreHistory = Enumerable.Range(0, input.d)
        //             .Select(_ => new long[26])
        //             .ToArray();
        //
        //         var lastHeldDate = Enumerable.Range(0, 26)
        //             .Select(_ => -1)
        //             .ToArray();
        //
        //         for (var i = 0; i < input.s.Length; i++)
        //         {
        //             // 今日のスコアの元
        //
        //             var todayScoreBase = lastHeldDate
        //                 .Select((item, index) => (item, index))
        //                 .Select(x => (i - x.item, x.index))
        //                 .Select(x => -1 * x.Item1 * input.c[x.index])
        //                 .ToArray();
        //
        //             var todaySum = todayScoreBase.Sum();
        //             var todayS = input.s[i];
        //
        //             var maxScore = long.MinValue;
        //             var selectedType = 0;
        //
        //             // Greedyにやる。26種類それぞれためして、一番スコアが高くなる種別を選ぶ。
        //             for (int j = 0; j < 26; j++)
        //             {
        //                 var v = todaySum + (-1 * todayScoreBase[j]);
        //                 v += todayS[j];
        //
        //                 if (maxScore <= v)
        //                 {
        //                     selectedType = j;
        //                     maxScore = v;
        //                 }
        //             }
        //
        //             // その日選んだものを保存。
        //             ansList.Add(selectedType);
        //
        //             // 最後の開催日データを更新
        //             lastHeldDate[selectedType] = i;
        //         }
        //
        //         // ansListを1-indexedな日付に変更
        //
        //         ansList = ansList.Select(x => x + 1).ToList();
        //
        //
        //         // 返却前に一応値チェックしておく。
        //         if (ansList.Any(x => (x < 1 || 26 < x)))
        //         {
        //             throw new Exception("contest type error.");
        //         }
        //
        //         if (ansList.Count != input.d)
        //         {
        //             throw new Exception("anser length error.");
        //         }
        //
        //
        //         return new Response
        //         {
        //             answerList = ansList.ToArray(),
        //             lastScore = CalculateScoreSum(input, ansList.ToArray())
        //         };
        //     }
        // }


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

    namespace Solver
    {
        public struct Input
        {
            // 開催される日数
            public int d { get; set; }

            // コンテストが開かれなかったときのscoreの下がりやすさ
            public long[] c { get; set; }

            // i日目にコンテストが開かれたときに得られるscore
            public long[][] s { get; set; }
        }

        /// <summary>
        /// Solverとやり取りするための型
        /// </summary>
        public struct Response
        {
            public int[] answerList;
            public long lastScore;

            public void AnswerWrite()
            {
                foreach (var i in answerList)
                {
                    Console.WriteLine(i);
                }
            }
        }

        /// <summary>
        /// 答えとして出力する、コンテスト開催タイプ(1-indexed)のリストを返却します。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public interface ISolver
        {
            Response Solve(Input input);
        }


        namespace Implementation
        {
            public static class Utils
            {
                public static void ValidateAnsArray(Input input, int[] ansList)
                {
                    // 返却前に一応値チェックしておく。
                    if (ansList.Any(x => (x < 1 || 26 < x)))
                    {
                        throw new Exception("contest type error.");
                    }

                    if (ansList.Length != input.d)
                    {
                        throw new Exception("answer length error.");
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

                    // Debug.WriteLine($"[DEBUG] ### Last Score : {currentScore} ### ");
                    // Console.WriteLine($"[DEBUG] ### Last Score : {currentScore} ### ");

                    return Math.Max(1000000 + currentScore, 0);
                }
            }


            /// <summary>
            /// 山登り法
            /// </summary>
            public class EditorialClimbingSolver : ISolver
            {
                public Response Solve(Input input)
                {
                    // タイマー 1.8 秒
                    var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var timeLimit = 1800L;

                    var randomGenerator = new Random(Seed: 10);

                    // 各日に開催するコンテストをGreedyで先にきめてしまう。
                    var editorialGreedySolver = new EditorialGreedySolver(10);
                    var response = editorialGreedySolver.Solve(input);
                    var array = response.answerList.Select(x => x - 1).ToArray();
                    // var array = Enumerable.Range(0, input.d).Select(x => randomGenerator.Next(0, 26)).ToArray();

                    var currentScore = Utils.CalculateScoreSum(input, array.Select(x => x + 1).ToArray());

                    // startから1.8秒までの間、探索し続ける。
                    while (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() < start + 1800)
                    {
                        var type = randomGenerator.NextDouble();
                        if (type > 0.8)
                        {
                            // 変更する日と、どの種別に変更するかをランダムにやる。
                            var d1 = randomGenerator.Next(0, input.d);

                            var q1 = randomGenerator.Next(0, 26);
                            var oldQ1 = array[d1];

                            // d1を入れ替えてみる
                            array[d1] = q1;

                            var newScore = Utils.CalculateScoreSum(input, array.Select(x => x + 1).ToArray());

                            if (newScore > currentScore)
                            {
                                currentScore = newScore;
                            }
                            else
                            {
                                // 上がらなかったら戻す。
                                array[d1] = oldQ1;
                            }
                        }
                        else if (type > 0.5)
                        {
                            // 変更する日と、どの種別に変更するかをランダムにやる。
                            var d1 = randomGenerator.Next(0, input.d - 32);
                            var oldQ1 = array[d1];

                            // 2日選んで、その間でコンテストタイプを入れ替えてみる
                            var d2 = randomGenerator.Next(d1 + 1, Math.Min((d1 + 16), input.d));
                            var oldQ2 = array[d2];

                            var d3 = randomGenerator.Next(d2 + 1, Math.Min((d2 + 16), input.d));
                            var oldQ3 = array[d3];

                            // d1とd2を入れ替えてみる
                            array[d1] = oldQ3;
                            array[d2] = oldQ1;
                            array[d3] = oldQ2;

                            var newScore = Utils.CalculateScoreSum(input, array.Select(x => x + 1).ToArray());

                            if (newScore > currentScore)
                            {
                                currentScore = newScore;
                            }
                            else
                            {
                                // 上がらなかったら戻す。
                                array[d1] = oldQ1;
                                array[d2] = oldQ2;
                                array[d3] = oldQ3;
                            }
                        }
                        else
                        {
                            // 変更する日と、どの種別に変更するかをランダムにやる。
                            var d1 = randomGenerator.Next(0, input.d - 1);
                            var oldQ1 = array[d1];

                            // 2日選んで、その間でコンテストタイプを入れ替えてみる
                            var d2 = randomGenerator.Next(d1 + 1, Math.Min((d1 + 16), input.d));
                            var oldQ2 = array[d2];

                            // d1とd2を入れ替えてみる
                            array[d1] = oldQ2;
                            array[d2] = oldQ1;

                            var newScore = Utils.CalculateScoreSum(input, array.Select(x => x + 1).ToArray());

                            if (newScore > currentScore)
                            {
                                currentScore = newScore;
                            }
                            else
                            {
                                // 上がらなかったら戻す。
                                array[d1] = oldQ1;
                                array[d2] = oldQ2;
                            }
                        }
                    }


                    var ret = array.Select(x => x + 1).ToArray();
                    Utils.ValidateAnsArray(input, ret);

                    return new Response
                    {
                        answerList = ret,
                        lastScore = Utils.CalculateScoreSum(input, ret)
                    };
                }
            }


            public class EditorialGreedySolver : ISolver
            {
                private readonly int duration;

                public EditorialGreedySolver(int k)
                {
                    this.duration = k;
                }

                public Response Solve(Input input)
                {
                    var ansList = new List<int>();

                    for (int day = 0; day < input.d; day++)
                    {
                        var currentMaxScore = long.MinValue;
                        var bestContestType = 0;

                        for (int type = 0; type < 26; type++)
                        {
                            ansList.Add(type);

                            var score = evaluate(input, ansList.Select(x => x + 1).ToArray(), duration);

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
                    Utils.ValidateAnsArray(input, ret);

                    return new Response
                    {
                        answerList = ret,
                        lastScore = Utils.CalculateScoreSum(input, ret)
                    };
                }

                // 評価関数
                private long evaluate(Input input, int[] answerList, int k)
                {
                    var score = 0L;

                    var ans = answerList.Select(x => x - 1).ToArray();

                    // 最後に開催された日付け。随時更新。
                    var contestLastHeld = Enumerable.Range(0, 26).Select(_ => -1).ToArray();

                    for (var d = 0; d < ans.Length; d++)
                    {
                        // Console.WriteLine(d);
                        contestLastHeld[ans[d]] = d;

                        for (int i = 0; i < 26; i++)
                        {
                            score -= input.c[i] * (d - contestLastHeld[i]);
                        }

                        score += input.s[d][ans[d]];
                    }

                    // k回後まで他になにもコンテストが開催されないと仮定してスコア計算してみる。
                    for (int d = ans.Length; d < Math.Min(ans.Length + k, input.d); d++)
                    {
                        for (int i = 0; i < 26; i++)
                        {
                            score -= input.c[i] * (d - contestLastHeld[i]);
                        }
                    }

                    return score;
                }
            }

            public class RandomSolver : ISolver
            {
                public Response Solve(Input input)
                {
                    var list = new List<int>();

                    for (int i = 0; i < input.d; i++)
                    {
                        list.Add((i % 26) + 1);
                    }

                    return new Response
                    {
                        answerList = list.ToArray(),
                        lastScore = Utils.CalculateScoreSum(input, list.ToArray())
                    };
                }
            }
        }
    }
}