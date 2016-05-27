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
            get { return data[(row * columns) + column]; }
            set { data[(row * columns) + column] = value; }
        }

        public override MatrixType Type => MatrixType.ColumnOptimized;
    }
}