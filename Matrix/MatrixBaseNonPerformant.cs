using System;
using System.Text;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace Free.Matrix
{
    /// <summary>
    /// This class is where all of the non optimmized/performant matrix code lives.
    /// This is where we make the code work, then we place optimized versions of
    /// that code in the "shipping" versions of the code. Tests are run against all
    /// of the overridden methods of any inherited class.
    /// 
    /// Nothing should inherit from this class - this is an example of how to do
    /// matrix math correctly.
    /// </summary>
    public class MatrixBaseNonPerformant : IMatrix
    {
        protected readonly float[] data;
        protected readonly int rows;
        protected readonly int columns;
        protected readonly Dictionary<MatrixNormType, Func<float>> NormVTable = new Dictionary<MatrixNormType, Func<float>>();

        public MatrixBaseNonPerformant(int x, int y)
        {
            data = new float[x * y];
            rows = x;
            columns = y;
            NormVTable.Add(MatrixNormType.One_Norm, OneNorm);
            NormVTable.Add(MatrixNormType.Two_Norm, TwoNorm);
        }

        public MatrixType Type => MatrixType.NonPerormant;

        public float this[int index]
        {
            get { return data[index]; }
            set { data[index] = value; }
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

        public void CopyTo(IMatrix m)
        {
            if (m == null)
                throw new ArgumentNullException(nameof(m));

            for (int row = 0; row < Rows; row++)
                for (int column = 0; column < Columns; column++)
                    m[row, column] = this[row, column];
        }

        /// <summary>
        /// Fill a matrix with a value
        /// </summary>
        /// <param name="value">value to fill the matrix with</param>
        public void Fill(float value)
        {
            using (var rand = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()), true))
            {
                Parallel.For(0, data.Length, (i) =>
                {
                    data[i] = value;
                });
            }
        }

        /// <summary>
        /// Matrix Multiple
        /// </summary>
        /// <param name="m2">Row changes fast for this matrix</param>
        public virtual IMatrix Multiply(IMatrix m2)
        {
            IMatrix m3 = new MatrixBase(m2.Columns, m2.Rows);
            for (int row = 0; row < this.rows; row++)
            {
                var tempc = (row * this.columns);
                for (int cola = 0; cola < m2.Columns; cola++)
                {
                    float val = 0;
                    for (int colb = 0; colb < this.Columns; colb++)
                    {
                        val += this[row, colb] * m2[colb, cola];
                    }
                    m3[tempc + cola] = val;
                }
            }
            return m3;
        }

        public virtual IMatrix Multiply2(IMatrix m2)
        {
            IMatrix m3 = new MatrixBase(m2.Columns, m2.Rows);
            for (int row = 0; row < this.rows; row++)
            {
                var tempc = (row * this.columns);
                for (int cola = 0; cola < m2.Columns; cola++)
                {
                    float val = 0;
                    for (int colb = 0; colb < this.Columns; colb++)
                    {
                        //(column * rows) + row <-- row optimized
                        //(row * columns) + column <-- col optimized
                        // both the same non-optimized so access is the same
                        val += this[(colb * rows) + row] * m2[(cola * columns) + colb];
                    }
                    m3[tempc + cola] = val;
                }
            }
            return m3;
        }

        /// <summary>
        /// Compute the 1-norm of the matrix
        /// </summary>
        /// <returns>1-norm</returns>
        private float OneNorm()
        {
            var results = new ConcurrentBag<float>();
            for (int column = 0; column < columns; column++)
            {
                var value = default(float);
                for (int row = 0; row < rows; row++)
                {
                    value += Math.Abs(this[column, row]);
                }
                results.Add(value);
            }
            return results.Max();
        }

        /// <summary>
        /// Compute the 2-norm of the matrix
        /// </summary>
        /// <returns>2-norm</returns>
        private float TwoNorm()
        {
            var results = new ConcurrentBag<float>();
            for (int row = 0; row < rows; row++)
            {
                var value = default(float);
                for (int column = 0; column < rows; column++)
                {
                    value += Math.Abs(this[column, row]);
                }
                results.Add(value);
            }
            return results.Max();
        }

        /// <summary>
        /// Compute the "MatrixNormType" norm
        /// </summary>
        /// <param name="norm">MatrixNormType to compute</param>
        /// <returns></returns>
        public float Norm(MatrixNormType norm)
        {
            return NormVTable[norm].Invoke();
        }

        /// <summary>
        /// Scalar multiply
        /// </summary>
        /// <param name="value">Value to multiply</param>
        /// <returns>Matrix multiplication results</returns>
        public IMatrix Multiply(float value)
        {
            IMatrix result = new ColumnOptimizedMatrix(rows, columns);
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                    result[column, row] = this[column, row] * value;
            return result;
        }

        /// <summary>
        /// Scalar add
        /// </summary>
        /// <param name="value">Value to add</param>
        /// <returns>Matrix addition results</returns>
        public IMatrix Add(float value)
        {
            IMatrix result = new ColumnOptimizedMatrix(rows, columns);
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                    result[column, row] = this[column, row] + value;
            return result;
        }

        public IEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }
    }
}