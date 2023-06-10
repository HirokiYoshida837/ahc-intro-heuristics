using System;
using System.Collections.Generic;
using System.Linq;
using AHC_Intro.Solver;
using AHC_Intro.Solver.Implementation;
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
        public void テストケース10個生成して評価_greedy()
        {
            var scores = new List<Result>();

            for (int seed = 0; seed < 10; seed++)
            {
                var generatedCase = TestCaseGenerator.Generate(seed);

                var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var solveProblem = AHC_Intro.Program.SolveProblem(generatedCase, new EditorialGreedySolver(10));
                var end = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var result = new Result(seed, start, end, solveProblem.lastScore);
                scores.Add(result);

                Console.WriteLine(result);
            }


            Console.WriteLine("### Test Finished ###");
            Console.WriteLine($"score Sum : {scores.Select(x => x.score).Sum()} \t totalTime : {scores.Select(x => x.time).Sum()}ms");
        }
        
        /// <summary>
        /// テストケース10個で2秒くらい
        /// </summary>
        [Test]
        public void テストケース10個生成して評価_climbing()
        {
            var scores = new List<Result>();

            for (int seed = 0; seed < 10; seed++)
            {
                var generatedCase = TestCaseGenerator.Generate(seed);

                var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var solveProblem = AHC_Intro.Program.SolveProblem(generatedCase, new EditorialClimbingSolver());
                var end = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var result = new Result(seed, start, end, solveProblem.lastScore);
                scores.Add(result);

                Console.WriteLine(result);
            }


            Console.WriteLine("### Test Finished ###");
            Console.WriteLine($"score Sum : {scores.Select(x => x.score).Sum()} \t totalTime : {scores.Select(x => x.time).Sum()}ms");
        }
        
        
        /// <summary>
        /// テストケース10個で2秒くらい
        /// </summary>
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        public void テストケース10個生成して評価_kを調整(int solverK)
        {
            var scores = new List<Result>();

            for (int seed = 0; seed < 10; seed++)
            {
                var generatedCase = TestCaseGenerator.Generate(seed);

                var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var solveProblem = AHC_Intro.Program.SolveProblem(generatedCase, new EditorialGreedySolver(solverK));
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