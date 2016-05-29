using System.Threading;
using System.Threading.Tasks;
using System;

namespace Free.Matrix
{
    public class ColumnOptimizedMatrix : MatrixBase
    {
        public ColumnOptimizedMatrix(int columns, int rows) : base(columns, rows)
        {
        }

        public override float this[int row, int column]
        {
            get { return Data[(row * columns) + column]; }
            set { Data[(row * columns) + column] = value; }
        }

        public override MatrixType Type => MatrixType.ColumnOptimized;

        public override MatrixBase Multiply(MatrixBase m, MatrixType type = MatrixType.ColumnOptimized)
        {
            if (m.Type != MatrixType.RowOptimized)
            {
                m = m.Reshape(MatrixType.RowOptimized);
            }

            var result = MatrixFactory.Create(m.Columns, m.Rows, MatrixType.ColumnOptimized);

            Parallel.For(0, rows, (row) =>
            //for (int row = 0; row < rows; row++)
            {
                var resultIndex = (row * columns);
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
                            index += 9;
                        }
                        else
                            val += Data[columnIndex++] * m.Data[rowIndex++];
                    }
                    result.Data[resultIndex] = val;
                    resultIndex++;
                }
            });
            return result.Reshape(type);
        }

        private void CopyToRowOptimized(MatrixBase m)
        {
            var columnIndex = 0;
            var rowIndex = 0;
            for (int row = 0; row < rows; row++)
            //Parallel.For(0, rows, (row) =>
            {
                for (int column = 0; column < columns; column++)
                {
                    m.Data[rowIndex++] = Data[columnIndex++];
                }
            }//);

        }

        public override MatrixBase Reshape(MatrixType type)
        {
            if (type == Type)
                return this;

            if (type == MatrixType.RowOptimized)
            {
                var fastCopy = MatrixFactory.Create(rows, columns, type);
                CopyToRowOptimized(fastCopy);
                return fastCopy;
            }

            var copy = MatrixFactory.Create(rows, columns, type);
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                    copy[row, column] = this[row, column];
            return copy;
        }

    }
}