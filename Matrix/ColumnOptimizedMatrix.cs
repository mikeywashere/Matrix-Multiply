using System.Threading;
using System.Threading.Tasks;
using System;

namespace Free.Matrix
{
    public class ColumnOptimizedMatrix : MatrixBase
    {
        public ColumnOptimizedMatrix(int x, int y) : base(x, y)
        {
        }

        public override float this[int row, int column]
        {
            get { return Data[(row * columns) + column]; }
            set { Data[(row * columns) + column] = value; }
        }

        public override MatrixType Type => MatrixType.ColumnOptimized;

        public override MatrixBase Multiply(MatrixBase m, MatrixType type = MatrixType.NonOptimized)
        {
            if (m.Type == MatrixType.RowOptimized)
                return MultiplyRowOptimized(m, type);

            // this is MatrixType.ColumnOptimized * MatrixType.ColumnOptimized

            var result = MatrixFactory.Create(m.Columns, m.Rows, type);

            Parallel.For(0, rows, (row) =>
            //for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    var columnIndex = (row * columns);
                    var rowIndex = column;

                    float val = 0;
                    for (int index = 0; index < columns; index++)
                    {
                        if (index < columns - 10)
                        {
                            val += Data[columnIndex++] * m.Data[rowIndex];
                            rowIndex += columns;
                            val += Data[columnIndex++] * m.Data[rowIndex];
                            rowIndex += columns;
                            val += Data[columnIndex++] * m.Data[rowIndex];
                            rowIndex += columns;
                            val += Data[columnIndex++] * m.Data[rowIndex];
                            rowIndex += columns;
                            val += Data[columnIndex++] * m.Data[rowIndex];
                            rowIndex += columns;
                            val += Data[columnIndex++] * m.Data[rowIndex];
                            rowIndex += columns;
                            val += Data[columnIndex++] * m.Data[rowIndex];
                            rowIndex += columns;
                            val += Data[columnIndex++] * m.Data[rowIndex];
                            rowIndex += columns;
                            val += Data[columnIndex++] * m.Data[rowIndex];
                            rowIndex += columns;
                            val += Data[columnIndex++] * m.Data[rowIndex];
                            rowIndex += columns;
                            index += 10;
                        }
                        else
                        {
                            val += Data[columnIndex++] * m.Data[rowIndex];
                            rowIndex += columns;
                        }
                    }
                    result[row, column] = val;
                }
            });
            return result;
        }

        private MatrixBase MultiplyRowOptimized(MatrixBase m, MatrixType type = MatrixType.NonOptimized)
        {
            // this is MatrixType.ColumnOptimized * MatrixType.RowOptimized

            var result = MatrixFactory.Create(m.Columns, m.Rows, type);

            Parallel.For(0, rows, (row) =>
            //for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    var columnIndex = (row * columns);
                    var rowIndex = (column * rows);

                    float val = 0;
                    for (int index = 0; index < columns; index++)
                    {
                        if (index < columns - 10)
                        {
                            val += Data[columnIndex++] * m.Data[rowIndex++];
                            val += Data[columnIndex++] * m.Data[rowIndex++];
                            val += Data[columnIndex++] * m.Data[rowIndex++];
                            val += Data[columnIndex++] * m.Data[rowIndex++];
                            val += Data[columnIndex++] * m.Data[rowIndex++];
                            val += Data[columnIndex++] * m.Data[rowIndex++];
                            val += Data[columnIndex++] * m.Data[rowIndex++];
                            val += Data[columnIndex++] * m.Data[rowIndex++];
                            val += Data[columnIndex++] * m.Data[rowIndex++];
                            val += Data[columnIndex++] * m.Data[rowIndex++];
                            index += 10;
                        }
                        else
                            val += Data[columnIndex++] * m.Data[rowIndex++];
                    }
                    result[row, column] = val;
                }
            });
            return result;
        }

    }
}