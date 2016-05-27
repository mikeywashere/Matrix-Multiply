using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Free.Matrix
{
    public enum MatrixType
    {
        RowOptimized,
        ColumnOptimized,
        NonPerormant
    }

    public static class MatrixFactory
    {

        public static IMatrix Create(int rows, int columns, MatrixType type)
        {
            switch (type)
            {
                case MatrixType.ColumnOptimized:
                    return new ColumnOptimizedMatrix(rows, columns);
                case MatrixType.RowOptimized:
                    return new RowOptimizedMatrix(rows, columns);
#if DEBUG
                case MatrixType.NonPerormant:
                    return new MatrixBaseNonPerformant(rows, columns);
#endif
                default:
                    return new MatrixBase(rows, columns);
            }
        }

        public static IMatrix CreateFrom(IMatrix m)
        {
            return Create(m.Rows, m.Columns, m.Type);
        }

        public static IMatrix CreateCopy(IMatrix m)
        {
            var copy = Create(m.Rows, m.Columns, m.Type);
            for (int row = 0; row < m.Rows; row++)
                for (int column = 0; column < m.Columns; column++)
                    copy[row, column] = m[row, column];
            return copy;
        }

        public static IMatrix CreateCopy(IMatrix m, MatrixType type)
        {
            var copy = Create(m.Rows, m.Columns, type);
            for (int row = 0; row < m.Rows; row++)
                for (int column = 0; column < m.Columns; column++)
                    copy[row, column] = m[row, column];
            return copy;
        }

        public static List<IMatrix> Create(int rows, int columns)
        {
            var list = new List<IMatrix>();
            foreach (var i in Enum.GetValues(typeof(MatrixType)))
            {
                list.Add(Create(rows, columns, (MatrixType) i));
            }
            return list;
        }
    }
}
