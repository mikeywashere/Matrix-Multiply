using Free.Matrix;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Matrix.UnitTests
{
    [TestClass]
    public class MatrixTester
    {
        [TestMethod]
        public void TestThatColumnOptimizedIsCorrect()
        {
            var i = 0;
            var matrix = new ColumnOptimizedMatrix(10, 10);
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    matrix[x, y] = i;
                    i++;
                }
            }

            for (int j = 0; j < 100; j++)
            {
                var v = matrix[j];
                if (Convert.ToInt32(v) != j)
                {
                    Assert.Fail("OOPS");
                }
            }
        }

        [TestMethod]
        public void TestThatRowOptimizedIsCorrect()
        {
            var i = 0;
            var matrix = new RowOptimizedMatrix(10, 10);
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    matrix[x, y] = i;
                    i++;
                }
            }

            for (int j = 0; j < 100; j++)
            {
                var v = matrix[j];
                if (Convert.ToInt32(v) != j)
                {
                    Assert.Fail("OOPS");
                }
            }
        }

        private static MatrixBase TestValues(MatrixType type)
        {
            var matrix = MatrixFactory.Create(10, 10, type);
            for (int x = 0; x < 10; x++)
            {
                matrix[x, 0] = new int[] { 285, 330, 375, 420, 465, 510, 555, 600, 645, 690 }[x];
                matrix[x, 1] = new int[] { 330, 385, 440, 495, 550, 605, 660, 715, 770, 825 }[x];
                matrix[x, 2] = new int[] { 375, 440, 505, 570, 635, 700, 765, 830, 895, 960 }[x];
                matrix[x, 3] = new int[] { 420, 495, 570, 645, 720, 795, 870, 945, 1020, 1095 }[x];
                matrix[x, 4] = new int[] { 465, 550, 635, 720, 805, 890, 975, 1060, 1145, 1230 }[x];
                matrix[x, 5] = new int[] { 510, 605, 700, 795, 890, 985, 1080, 1175, 1270, 1365 }[x];
                matrix[x, 6] = new int[] { 555, 660, 765, 870, 975, 1080, 1185, 1290, 1395, 1500 }[x];
                matrix[x, 7] = new int[] { 600, 715, 830, 945, 1060, 1175, 1290, 1405, 1520, 1635 }[x];
                matrix[x, 8] = new int[] { 645, 770, 895, 1020, 1145, 1270, 1395, 1520, 1645, 1770 }[x];
                matrix[x, 9] = new int[] { 690, 825, 960, 1095, 1230, 1365, 1500, 1635, 1770, 1905 }[x];
            }
            return matrix;
        }

        private static string expected = @"0: 285.0000,330.0000,375.0000,420.0000,465.0000,510.0000,555.0000,600.0000,645.0000,690.0000
1: 330.0000,385.0000,440.0000,495.0000,550.0000,605.0000,660.0000,715.0000,770.0000,825.0000
2: 375.0000,440.0000,505.0000,570.0000,635.0000,700.0000,765.0000,830.0000,895.0000,960.0000
3: 420.0000,495.0000,570.0000,645.0000,720.0000,795.0000,870.0000,945.0000,1,020.0000,1,095.0000
4: 465.0000,550.0000,635.0000,720.0000,805.0000,890.0000,975.0000,1,060.0000,1,145.0000,1,230.0000
5: 510.0000,605.0000,700.0000,795.0000,890.0000,985.0000,1,080.0000,1,175.0000,1,270.0000,1,365.0000
6: 555.0000,660.0000,765.0000,870.0000,975.0000,1,080.0000,1,185.0000,1,290.0000,1,395.0000,1,500.0000
7: 600.0000,715.0000,830.0000,945.0000,1,060.0000,1,175.0000,1,290.0000,1,405.0000,1,520.0000,1,635.0000
8: 645.0000,770.0000,895.0000,1,020.0000,1,145.0000,1,270.0000,1,395.0000,1,520.0000,1,645.0000,1,770.0000
9: 690.0000,825.0000,960.0000,1,095.0000,1,230.0000,1,365.0000,1,500.0000,1,635.0000,1,770.0000,1,905.0000";
        /// <summary>
        /// Test the matrix multiply method
        /// </summary>
        [TestMethod]
        public void TestMultiply()
        {
            foreach (var type1 in MatrixFactory.Types())
            {
                var m1 = MatrixFactory.Create(10, 10, type1);
                MatrixBase m2 = null;
                foreach (var type2 in MatrixFactory.Types())
                {
                    m2 = MatrixFactory.Create(10, 10, type2);
                    for (int x = 0; x < 10; x++)
                        for (int y = 0; y < 10; y++)
                        {
                            m1[x, y] = m2[x, y] = x + y;
                        }


                    var m3 = m1.Multiply(m2, MatrixType.NonOptimized);

                    var test = m3.AsText();
                    if (test != expected)
                    {
                        Assert.Fail("(test != expected) in " + m1.Type + " * " + m2.Type);
                    }
                }
            }
        }

        [TestMethod]
        public void TestOneNorm()
        {
            foreach (var type in MatrixFactory.Types())
            {
                var m1 = MatrixFactory.Create(10, 10, type);
                var m = TestValues(m1.Type);
                var onv = m.Norm(MatrixNormType.One_Norm);
                m = m.Add(1);
                var tnv = m.Norm(MatrixNormType.Two_Norm);
                Assert.IsTrue((onv + 10) == tnv);
            }
        }

        [TestMethod]
        public void TestScalarMultiply()
        {
            foreach (var type in MatrixFactory.Types())
            {
                var m = MatrixFactory.Create(10, 10, type);
                m.Fill(10);
                var m2 = m.Multiply(10);
                for (int column = 0; column < m.Columns; column++)
                    for (int row = 0; row < m.Rows; row++)
                        if (m2[column, row] != m.Rows * m.Columns)
                        {
                            Assert.Fail("Failed in " + m.Type);
                        }
            }
        }

        [TestMethod]
        public void TestMultiplyingByEachType()
        {
            foreach (var typeX in MatrixFactory.Types())
            {
                var m = MatrixFactory.Create(10, 10, typeX);
                m.Fill(10);
                foreach (var typeY in MatrixFactory.Types())
                {
                    var n = MatrixFactory.Create(10, 10, typeY);
                    n.Fill(11);

                    var x = m.Multiply(n, MatrixType.NonOptimized);

                    for (int i = 0; i < x.Rows * x.Columns; i++)
                    {
                        if (x[i] != 1100.0000)
                        {
                            Assert.Fail("i = " + i + " x[i] = " + x[i] + " in " + m.Type + " * " + n.Type);
                        }
                    }
                }
            }
        }

        private static void FillOrdered(MatrixBase m)
        {
            for (int column = 0; column < m.Columns; column++)
            {
                m[0, column] = column + 1;
            }

            for (int row = 1; row < m.Rows; row++)
            {
                for (int column = 0; column < m.Columns; column++)
                {
                    m[row, column] = m[row - 1, column] + 1;
                }
            }
        }

        private static MatrixBase AnswerOrdered()
        {
            var m = MatrixFactory.Create(20, 20, MatrixType.ColumnOptimized);
            var index = 0;
            foreach (int value in new[]{
                2870,3080,3290,3500,3710,3920,4130,4340,4550,4760,4970,5180,5390,5600,5810,6020,6230,6440,6650,6860,
                3080,3310,3540,3770,4000,4230,4460,4690,4920,5150,5380,5610,5840,6070,6300,6530,6760,6990,7220,7450,
                3290,3540,3790,4040,4290,4540,4790,5040,5290,5540,5790,6040,6290,6540,6790,7040,7290,7540,7790,8040,
                3500,3770,4040,4310,4580,4850,5120,5390,5660,5930,6200,6470,6740,7010,7280,7550,7820,8090,8360,8630,
                3710,4000,4290,4580,4870,5160,5450,5740,6030,6320,6610,6900,7190,7480,7770,8060,8350,8640,8930,9220,
                3920,4230,4540,4850,5160,5470,5780,6090,6400,6710,7020,7330,7640,7950,8260,8570,8880,9190,9500,9810,
                4130,4460,4790,5120,5450,5780,6110,6440,6770,7100,7430,7760,8090,8420,8750,9080,9410,9740,10070,10400,
                4340,4690,5040,5390,5740,6090,6440,6790,7140,7490,7840,8190,8540,8890,9240,9590,9940,10290,10640,10990,
                4550,4920,5290,5660,6030,6400,6770,7140,7510,7880,8250,8620,8990,9360,9730,10100,10470,10840,11210,11580,
                4760,5150,5540,5930,6320,6710,7100,7490,7880,8270,8660,9050,9440,9830,10220,10610,11000,11390,11780,12170,
                4970,5380,5790,6200,6610,7020,7430,7840,8250,8660,9070,9480,9890,10300,10710,11120,11530,11940,12350,12760,
                5180,5610,6040,6470,6900,7330,7760,8190,8620,9050,9480,9910,10340,10770,11200,11630,12060,12490,12920,13350,
                5390,5840,6290,6740,7190,7640,8090,8540,8990,9440,9890,10340,10790,11240,11690,12140,12590,13040,13490,13940,
                5600,6070,6540,7010,7480,7950,8420,8890,9360,9830,10300,10770,11240,11710,12180,12650,13120,13590,14060,14530,
                5810,6300,6790,7280,7770,8260,8750,9240,9730,10220,10710,11200,11690,12180,12670,13160,13650,14140,14630,15120,
                6020,6530,7040,7550,8060,8570,9080,9590,10100,10610,11120,11630,12140,12650,13160,13670,14180,14690,15200,15710,
                6230,6760,7290,7820,8350,8880,9410,9940,10470,11000,11530,12060,12590,13120,13650,14180,14710,15240,15770,16300,
                6440,6990,7540,8090,8640,9190,9740,10290,10840,11390,11940,12490,13040,13590,14140,14690,15240,15790,16340,16890,
                6650,7220,7790,8360,8930,9500,10070,10640,11210,11780,12350,12920,13490,14060,14630,15200,15770,16340,16910,17480,
                6860,7450,8040,8630,9220,9810,10400,10990,11580,12170,12760,13350,13940,14530,15120,15710,16300,16890,17480,18070})
            {
                m.Data[index++] = value;
            }
            return m;
        }

        [TestMethod]
        public void CopyToTest()
        {
            foreach (var typeX in MatrixFactory.Types())
            {
                var m = MatrixFactory.Create(10, 10, typeX);

                FillOrdered(m);

                var mCopy = MatrixFactory.Create(10, 10, typeX);
                m.CopyTo(mCopy);
                if (!m.ExactMatch(mCopy)) Assert.Fail($"!m.ExactMatch(mCopy) where m is a {m.Type} and mCopy is a {mCopy.Type}");
            }
        }

        [TestMethod]
        public void MultiplyTest2()
        {
            foreach (var typeX in MatrixFactory.Types())
            {
                var m1 = MatrixFactory.Create(20, 20, typeX);
                var m2 = MatrixFactory.Create(20, 20, typeX);

                FillOrdered(m1);
                FillOrdered(m2);

                var m3 = m1.Multiply(m2);

                var expected = AnswerOrdered();
                if (!m3.ExactMatch(expected))
                    Assert.Fail($"!m3.ExactMatch(expected) where m3 is a {m3.Type} and expected is a {expected.Type}");
            }
        }

    }
}