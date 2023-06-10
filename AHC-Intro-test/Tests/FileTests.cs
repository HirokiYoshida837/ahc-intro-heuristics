using System;
using System.IO;
using System.Linq;
using AHC_Intro.Solver;
using NUnit.Framework;

namespace AHC_Intro_test
{
    // testcaseをファイルから読み込んで実施。
    public class FileTests
    {
        /// <summary>
        /// サンプルにある d = 5 の簡単なケース
        /// </summary>
        [Test]
        public void test0()
        {
            var reading = File.ReadAllLines($@"Resources\Cases\FileTests\case0\input.txt");
            checkInputFile(reading);

            var input = readFileToInput(reading);
            var solveProblem = AHC_Intro.Program.SolveProblem(input);

            WriteResponse(solveProblem);
        }

        [Test]
        public void test1()
        {
            var reading = File.ReadAllLines($@"Resources\Cases\FileTests\case1\input.txt");
            checkInputFile(reading);
            
            var input = readFileToInput(reading);
            var solveProblem = AHC_Intro.Program.SolveProblem(input);
            
            WriteResponse(solveProblem);
        }

        public static void WriteResponse(Response res)
        {
            Console.WriteLine($"### last score : {res.lastScore} ###");
            res.AnswerWrite();
        }


        public static Input readFileToInput(string[] reading)
        {
            var input = new Input
            {
                d = int.Parse(reading[0]),
                c = reading[1].Split().Select(long.Parse).ToArray(),
                s = reading.Skip(2).Select(x => x.Split().Select(long.Parse).ToArray()).ToArray()
            };

            return input;
        }

        /// <summary>
        /// 生成したテストケースが正当かどうかチェックする。
        /// </summary>
        public static void checkInputFile(string[] reading)
        {
            var input = readFileToInput(reading);


            // sの日数チェック
            Assert.That(input.d, Is.EqualTo(input.s.Length));

            // Cのフォーマットチェック
            Assert.That(input.c.Length.Equals(26));
            Assert.That(input.c.All(x => x >= 0 && x <= 100), Is.True);

            // sのフォーマットチェック
            Assert.That(input.s.Select(x => x.Length).All(x => x == 26), Is.True);
            Assert.That(input.s.Select(x => x.All(y => y >= 0 && y <= 20000)).All(x => x), Is.True);
        }
    }
}