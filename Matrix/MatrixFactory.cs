using System;
using System.Collections.Generic;
using System.Linq;

namespace Free.Matrix
{
    public enum MatrixType
    {
        RowOptimized,
        ColumnOptimized,
        NonOptimized
    }

    public static class MatrixFactory
    {
        public static MatrixBase Create(int rows, int columns, MatrixType type)
        {
            switch (type)
            {
                case MatrixType.ColumnOptimized:
                    {
                        return new ColumnOptimizedMatrix(rows, columns);
                    }
                case MatrixType.RowOptimized:
                    {
                        return new RowOptimizedMatrix(rows, columns);
                    }
                default:
                    {
                        return new MatrixBase(rows, columns);
                    }
            }
        }

        public static MatrixBase CreateFrom(MatrixBase m)
        {
            return Create(m.Rows, m.Columns, m.Type);
        }

        public static MatrixBase CreateCopy(MatrixBase m)
        {
            var copy = Create(m.Rows, m.Columns, m.Type);
            for (int row = 0; row < m.Rows; row++)
                for (int column = 0; column < m.Columns; column++)
                    copy[row, column] = m[row, column];
            return copy;
        }

        public static MatrixBase CreateCopy(MatrixBase m, MatrixType type)
        {
            var copy = Create(m.Rows, m.Columns, type);
            for (int row = 0; row < m.Rows; row++)
                for (int column = 0; column < m.Columns; column++)
                    copy[row, column] = m[row, column];
            return copy;
        }

        public static List<MatrixType> Types()
        {
            return new List<MatrixType>(Enum.GetValues(typeof(MatrixType)).OfType<MatrixType>());
        }
    }
}