using System.Collections;
using System.Linq;

namespace Free.Matrix
{
    public interface IMatrix : IEnumerable
    {
        MatrixType Type { get; }

        float this[int x, int y]
        {
            get;
            set;
        }

        float this[int index]
        {
            get;
            set;
        }

        int Rows
        {
            get;
        }

        int Columns
        {
            get;
        }

        /// <summary>
        /// Multiply a matrix by matrix returning a third matrix
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        IMatrix Multiply(IMatrix m, MatrixType type);

        IMatrix Multiply(RowOptimizedMatrix m, MatrixType type = MatrixType.NonOptimized);

        /// <summary>
        /// Fill a matrix with a value
        /// </summary>
        /// <param name="value"></param>
        void Fill(float value);

        /// <summary>
        /// Multiply a scalar value to a matrix
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IMatrix Multiply(float value);

        /// <summary>
        /// Add a scalar value to a matrix
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IMatrix Add(float value);

        /// <summary>
        /// Return the "norm" specified by MatrixNormType norm
        /// </summary>
        /// <param name="norm">Type of norm to calculate</param>
        /// <returns>norm</returns>
        float Norm(MatrixNormType norm);

        void CopyTo(IMatrix m);
    }
}