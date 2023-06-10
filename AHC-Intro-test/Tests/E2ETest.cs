using System;
using System.IO;
using NUnit.Framework;

namespace AHC_Intro_test
{
    public class E2ETest : TestBase
    {
        [Test]
        public void MainClassTest()
        {
            // テストがエラー出ずに動くかどうかのテストだけ
            var input = File.ReadAllText($@"Resources\Cases\MainClassTest\input.txt");
            var test = Test(input, null, () => { AHC_Intro.Program.Main(new String[] { }); });
        }
    }
}