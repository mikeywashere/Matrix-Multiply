using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Free.Matrix
{
    public class MatrixBase : IMatrix
    {
        protected readonly float[] data;
        protected readonly int rows;
        protected readonly int columns;

        public MatrixBase(int x, int y)
        {
            data = new float[x * y];
            rows = x;
            columns = y;
        }

        public virtual float this[int row, int column]
        {
            get { return data[(column * rows) + row]; }
            set { data[(column * rows) + row] = value; }
        }

        public int Rows => rows;

        public int Columns => columns;

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int row = 0; row < Rows; row++)
            {
                sb.AppendFormat("{0}: ", row);
                for (int col = 0; col < Columns; col++)
                {
                    sb.AppendFormat("{0:#,0.0000},", this[row, col]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public void RandomFill(double from, double to)
        {
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 4
            };

            using (var rand = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()), true))
            {
                Parallel.For(0, rows * columns, parallelOptions, (i) =>
                {
                    var d = rand.Value.NextDouble();
                    data[i] = (float)(d * (to - from) + from);
                });
            }
        }


        /// <summary>
        /// Matrix Multiple
        /// </summary>
        /// <param name="m1">Column changes fast for this matrix</param>
        /// <param name="m2">Row changes fast for this matrix</param>
        /// <param name="res">Column changes fast for this matrix</param>
        public static void MatrixMultiply(IMatrix m1, IMatrix m2, IMatrix res)
        {
            var parallelOptions1 = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2
            };

            var parallelOptions2 = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 4
            };

            Parallel.For(0, m1.Rows, parallelOptions2, (row) =>
            {
                Parallel.For(0, m2.Columns, parallelOptions2, (cola) =>
                {
                    float val = 0;
                    for (int colb = 0; colb < m1.Columns; colb++)
                    {
                        val += m1[row, colb] * m2[colb, cola];
                    }
                    res[row, cola] = val;
                });
            });
        }
    }
}