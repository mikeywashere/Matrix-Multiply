using System.Threading;
using System.Threading.Tasks;

namespace Free.Matrix
{
    public class RowOptimizedMatrix : MatrixBase
    {
        public RowOptimizedMatrix(int columns, int rows) : base(columns, rows)
        {
        }

        public override MatrixType Type => MatrixType.RowOptimized;

        public override MatrixBase Multiply(MatrixBase m, MatrixType type = MatrixType.NonOptimized)
        {
            // this is MatrixType.RowOptimized * MatrixType.ColumnOptimized

            if (m.Type != MatrixType.ColumnOptimized)
            {
                m = MatrixFactory.CreateCopy(m, MatrixType.ColumnOptimized);
            }

            var result = MatrixFactory.Create(m.Columns, m.Rows, type);

            Parallel.For(0, columns, (column) =>
            //for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < rows; row++)
                {
                    float val = 0;
                    var rowIndex = (column * rows);
                    var columnIndex = (row * columns);
                    for (int index = 0; index < rows; index++)
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
                            index = index + 9;
                        }
                        else
                            val += Data[rowIndex++] * m.Data[columnIndex++];
                    }
                    result[row, column] = val;
                }
            });
            return result;
        }
    }
}