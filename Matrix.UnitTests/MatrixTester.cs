using Free.Matrix;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Matrix.UnitTests
{
    public class ColumnOptimizedMatrixTester : ColumnOptimizedMatrix
    {
        public ColumnOptimizedMatrixTester(int x, int y) : base(x, y)
        {
        }

        public float[] AllData => data;
    }

    public class RowOptimizedMatrixTester : RowOptimizedMatrix
    {
        public RowOptimizedMatrixTester(int x, int y) : base(x, y)
        {
        }

        public float[] AllData => data;
    }

    [TestClass]
    public class MatrixTester
    {
        [TestMethod]
        public void TestThatColumnOptimizedIsCorrect()
        {
            var i = 0;
            var matrix = new ColumnOptimizedMatrixTester(10, 10);
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
                var v = matrix.AllData[j];
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
            var matrix = new RowOptimizedMatrixTester(10, 10);
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
                var v = matrix.AllData[j];
                if (Convert.ToInt32(v) != j)
                {
                    Assert.Fail("OOPS");
                }
            }
        }
    }
}