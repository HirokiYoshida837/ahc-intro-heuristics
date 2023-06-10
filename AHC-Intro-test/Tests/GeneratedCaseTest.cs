using System;
using System.Collections.Generic;
using System.Linq;
using AHC_Intro.Solver;
using AHC_Intro_test.TestUtils;
using Microsoft.VisualBasic;
using NUnit.Framework;

namespace AHC_Intro_test
{
    public class GeneratedCaseTest
    {
        [Test]
        public void test_defaultSeed()
        {
            var generatedCase = TestCaseGenerator.Generate();
            var solveProblem = AHC_Intro.Program.SolveProblem(generatedCase);
            WriteResponse(solveProblem);
        }

        [Test]
        public void test_seed2()
        {
            var generatedCase = TestCaseGenerator.Generate(2);

            var solveProblem = AHC_Intro.Program.SolveProblem(generatedCase);
            WriteResponse(solveProblem);
        }

        /// <summary>
        /// テストケース10個で2秒くらい
        /// </summary>
        [Test]
        public void テストケース50個生成して評価()
        {
            var scores = new List<Result>();

            for (int seed = 0; seed < 50; seed++)
            {
                var generatedCase = TestCaseGenerator.Generate(seed);

                var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var solveProblem = AHC_Intro.Program.SolveProblem(generatedCase);
                var end = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var result = new Result(seed, start, end, solveProblem.lastScore);
                scores.Add(result);

                Console.WriteLine(result);
            }


            Console.WriteLine("### Test Finished ###");
            Console.WriteLine($"score Sum : {scores.Select(x => x.score).Sum()} \t totalTime : {scores.Select(x => x.time).Sum()}ms");
        }


        public class Result
        {
            private int seed;
            private long start;
            private long end;

            public long time { get; }
            public long score { get; }

            public Result(int seed, long start, long end, long score)
            {
                this.seed = seed;
                this.start = start;
                this.end = end;

                this.time = end - start;
                this.score = score;
            }


            public override string ToString()
            {
                return ($"seed : {seed} \t score: {score} \t\t times: {time}ms");
            }
        }


        public static void WriteResponse(Response res)
        {
            Console.WriteLine($"### last score : {res.lastScore} ###");
            res.AnswerWrite();
        }
    }
}