using System;
using System.Text;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Linq.Expressions;

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
    public class MatrixBase
    {
        public readonly float[] Data;
        protected int rows;
        protected int columns;
        protected readonly Dictionary<MatrixNormType, Func<float>> NormVTable = new Dictionary<MatrixNormType, Func<float>>();

        public MatrixBase()
        {
            NormVTable.Add(MatrixNormType.One_Norm, OneNorm);
            NormVTable.Add(MatrixNormType.Two_Norm, TwoNorm);
        }

        public MatrixBase(int x, int y) : this()
        {
            Data = new float[x * y];
            rows = x;
            columns = y;
        }

        public virtual MatrixType Type => MatrixType.NonOptimized;

        public virtual float this[int index]
        {
            get { return Data[index]; }
            set { Data[index] = value; }
        }

        public virtual float this[int row, int column]
        {
            get { return Data[(column * rows) + row]; }
            set { Data[(column * rows) + row] = value; }
        }

        public virtual int Rows => rows;

        public virtual int Columns => columns;

        public string AsText()
        {
            var sb = new StringBuilder();
            for (int row = 0; row < rows; row++)
            {
                sb.AppendFormat("{0}: ", row);
                for (int col = 0; col < Columns; col++)
                {
                    if (row + col > 0 && col < columns && col > 0) sb.Append(",");
                    sb.AppendFormat("{0:#,0.0000}", this[row, col]);
                }
                if (row + 1 != rows)
                    sb.AppendLine();
            }
            return sb.ToString();
        }

        public virtual void CopyTo(MatrixBase m)
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
        public virtual void Fill(float value)
        {
            using (var rand = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()), true))
            {
                Parallel.For(0, rows * columns, (i) =>
                {
                    this[i] = value;
                });
            }
        }

        /// <summary>
        /// Matrix Multiple
        /// </summary>
        /// <param name="m">Row changes fast for this matrix</param>
        /// <param name="type">todo: describe type parameter on Multiply</param>
        public virtual MatrixBase Multiply(MatrixBase m, MatrixType type = MatrixType.NonOptimized)
        {
            var result = MatrixFactory.Create(m.Columns, m.Rows, type);
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    float val = 0;
                    for (int index = 0; index < columns; index++)
                    {
                        val += this[row, index] * m[index, column];
                    }
                    result[row, column] = val;
                }
            }
            return result;
        }

        /// <summary>
        /// Compute the 1-norm of the matrix
        /// </summary>
        /// <returns>1-norm</returns>
        protected virtual float OneNorm()
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
        protected virtual float TwoNorm()
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
        public virtual MatrixBase Multiply(float value)
        {
            MatrixBase result = new ColumnOptimizedMatrix(rows, columns);
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
        public virtual MatrixBase Add(float value)
        {
            MatrixBase result = new ColumnOptimizedMatrix(rows, columns);
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                    result[column, row] = this[column, row] + value;
            return result;
        }

        public IEnumerator GetEnumerator()
        {
            return Data.GetEnumerator();
        }
    }
}