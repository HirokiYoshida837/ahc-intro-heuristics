using System;
using System.Collections.Generic;
using System.Linq;
using AHC_Intro.Solver;
using Microsoft.VisualBasic;
using NUnit.Framework;

namespace AHC_Intro_test.TestUtils
{
    public class TestCaseGenerator
    {

        public static Input Generate(int seed = 1)
        {
            var input = new Input();
            input.d = 365;

            // 一旦seed固定
            var randomNumGenerator = new Random(seed);

            var cList = new List<long>();
            for (int i = 0; i < 26; i++)
            {
                cList.Add(randomNumGenerator.Next(InputStrict.minC, InputStrict.maxC));
            }
            input.c = cList.ToArray();
            

            var sListList = new List<List<long>>();
            for (int i = 0; i < input.d; i++)
            {

                var sList = new List<long>();
                for (int j = 0; j < 26; j++)
                {
                    sList.Add(randomNumGenerator.Next(InputStrict.minS, InputStrict.maxS));
                }
                sListList.Add(sList);
            }
            input.s = sListList.Select(x => x.ToArray()).ToArray();


            return input;
        }
        
        
        [Test]
        public void GenerateTestCase()
        {
            var generated = Generate();

            checkInputFile(generated);
            writeInput(generated);
        }

        public struct InputStrict
        {
            public static readonly int d = 365;
            
            
            public static readonly int minC = 0;
            public static readonly int maxC = 100;

            public static readonly int minS = 0;
            public static readonly int maxS = 20000;
        }
        
        /// <summary>
        /// 生成したテストケースが正当かどうかチェックする。
        /// </summary>
        public static void checkInputFile(Input input)
        {
            // sの日数チェック
            Assert.That(input.d, Is.EqualTo(input.s.Length));

            // Cのフォーマットチェック
            Assert.That(input.c.Length.Equals(26));
            Assert.That(input.c.All(x => x >= 0 && x <= 100), Is.True);

            // sのフォーマットチェック
            Assert.That(input.s.Select(x => x.Length).All(x => x == 26), Is.True);
            Assert.That(input.s.Select(x => x.All(y => y >= 0 && y <= 20000)).All(x => x), Is.True);
        }

        public static void writeInput(Input input)
        {
                        
            Console.WriteLine(input.d);

            var @joinC = Strings.Join(input.c.Select(x=>x.ToString()).ToArray(), " ");
            Console.WriteLine(joinC);


            foreach (var longs in input.s)
            {
                var @join = Strings.Join(longs.Select(x=>x.ToString()).ToArray(), " ");
                Console.WriteLine(join);
            }
        }
        
        
    }
}