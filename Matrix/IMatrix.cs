namespace Free.Matrix
{
    public interface IMatrix
    {
        float this[int x, int y]
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
    }
}