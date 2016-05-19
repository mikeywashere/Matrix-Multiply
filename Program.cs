using Free.Matrix;
using Matrix.UnitTests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CSharp.Multithread.Matrix.Multiply
{
    public class Program
    {
        private static void Main(string[] args)
        {
            const int repeat = 3;
            const int size = 500;

            var allTimes = new List<long>();

            var m1 = new ColumnOptimizedMatrix(size, size);
            var m2 = new RowOptimizedMatrix(size, size);

            var m3 = new ColumnOptimizedMatrix(size, size);

            for (int i = 0; i < repeat; i++)
            {
                var swrf = Stopwatch.StartNew();
                m1.RandomFill(0, 100);
                m2.RandomFill(0, 100);
                swrf.Stop();
                Console.WriteLine($"Fill: {swrf.ElapsedMilliseconds:#,0}");

                Console.WriteLine("-");

                var sw = Stopwatch.StartNew();
                MatrixBase.MatrixMultiply(m1, m2, m3);
                sw.Stop();

                allTimes.Add(sw.ElapsedMilliseconds);

                Console.WriteLine($"Mult: {sw.ElapsedMilliseconds:#,0}");
            }

            var average = allTimes.Average();

            Console.WriteLine($"Average: {average:#,0.00}");

            //Console.WriteLine();

            //Console.WriteLine(m3.ToString());
            //Thread.Sleep(2000);
            Console.ReadKey();
        }
    }
}