using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Free.Matrix
{
    public class FloatPointerEnumerator : IEnumerator
    {
        unsafe protected float* pointer;
        unsafe protected float* first;
        unsafe protected float* last;

        public FloatPointerEnumerator(IntPtr pointer, IntPtr last)
        {
            unsafe
            {
                this.pointer = this.first = (float*)pointer;
                this.last = (float*)last;
            }
        }

        public object Current
        {
            get
            {
                unsafe
                {
                    return (object)(*pointer);
                }
            }
        }

        public bool MoveNext()
        {
            unsafe
            {
                if (pointer == last)
                    return false;
                pointer++;
                return true;
            }
        }

        public void Reset()
        {
            unsafe
            {
                pointer = first;
            }
        }
    }

    public class ColumnOptimizedDirectMappedMatrix : MatrixBase, IDisposable
    {
        unsafe protected float* floatArray;
        unsafe protected float* firstFloat;

        public ColumnOptimizedDirectMappedMatrix(int x, int y) : base()
        {
            rows = x;
            columns = y;
            unsafe
            {
                // adding 64 bytes to help with allignment finding
                floatArray = (float*) Marshal.AllocHGlobal(x * y * sizeof(float) + 64);
                for (int i = 0; i < 64; i++)
                {
                    var value = ((int) floatArray + i);
                    if ((value & 63) == 0)
                    {
                        firstFloat = floatArray + i;
                        continue;
                    }
                }
                firstFloat = firstFloat == null ? floatArray : firstFloat;
            }
        }

        private int index(int row, int column)
        {
            return (row * columns) + column;
        }

        public override float this[int row, int column]
        {
            get { unsafe { return firstFloat[(row * columns) + column]; } }
            set { unsafe { firstFloat[(row * columns) + column] = value; } }
        }

        public override float this[int index]
        {
            get { unsafe { return firstFloat[index]; } }
            set { unsafe { firstFloat[index] = value; } }
        }

        public override MatrixType Type => MatrixType.ColumnOptimizedDirectMappedMatrix;

        public override IMatrix Multiply(IMatrix m, MatrixType type = MatrixType.NonOptimized)
        {
            var result = MatrixFactory.Create(m.Columns, m.Rows, type);

            Parallel.For(0, rows, (row) =>
            //for (int row = 0; row < rows; row++)
            {
                unsafe
                {
                    for (int cola = 0; cola < m.Columns; cola++)
                    {
                        var floatptr = firstFloat + (row * columns);
                        float val = 0;
                        for (int colb = 0; colb < columns; colb++)
                        {
                            val += *floatptr * m[colb, cola];
                            floatptr++;
                        }
                        result[row, cola] = val;
                    }
                }
            });
            return result;
        }

        public override IMatrix Multiply(RowOptimizedMatrix m, MatrixType type = MatrixType.NonOptimized)
        {
            var result = MatrixFactory.Create(m.Columns, m.Rows, type);

            Parallel.For(0, rows, (row) =>
            //for (int row = 0; row < rows; row++)
            {
                unsafe
                {
                    for (int cola = 0; cola < m.Columns; cola++)
                    {
                        var floatptr = firstFloat + (row * columns);
                        float val = 0;
                        var tempb = (cola * rows);
                        for (int colb = 0; colb < columns; colb++)
                        {
                            // row optimized
                            // colb, cola
                            // (cola * rows) + colb
                            // (column * rows) + row
                            val += *floatptr * m[tempb + colb];
                            floatptr++;
                        }
                        result[row, cola] = val;
                    }
                }
            });
            return result;
        }

        public new FloatPointerEnumerator GetEnumerator()
        {
            unsafe
            {
                return new FloatPointerEnumerator((IntPtr)firstFloat, (IntPtr) firstFloat + ((rows * columns) - 1));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            unsafe
            {
                if (floatArray == null)
                {
                    // release unmanaged memory
                    Marshal.FreeHGlobal((IntPtr) floatArray); 
                }
                if (disposing)
                {
                    // release other disposable objects
                }
            }
        }

        ~ColumnOptimizedDirectMappedMatrix()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}