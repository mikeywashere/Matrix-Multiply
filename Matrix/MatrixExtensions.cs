using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Free.Matrix
{
    public static class MatrixExtensions
    {
        public static void RandomFill(this IMatrix matrix, double from, double to)
        {
            using (var rand = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()), true))
            {
                Parallel.For(0, matrix.Columns * matrix.Rows, (i) =>
                {
                    var d = rand.Value.NextDouble();
                    matrix[i] = (float)(d * (to - from) + from);
                });
            }
        }

    }
}
