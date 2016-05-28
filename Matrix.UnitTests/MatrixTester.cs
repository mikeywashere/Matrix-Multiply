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

        private static string expected =@"0: 285.0000,330.0000,375.0000,420.0000,465.0000,510.0000,555.0000,600.0000,645.0000,690.0000
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
    }
}