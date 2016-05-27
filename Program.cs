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
        const int repeat = 3;
        const int size = 1000;

        static IMatrix co1 = new MatrixBaseNonPerformant(size, size);
        static IMatrix ro1 = new MatrixBaseNonPerformant(size, size);
        static MatrixBase co2 = new ColumnOptimizedMatrix(size, size);
        static MatrixBase ro2 = new RowOptimizedMatrix(size, size);

        static MatrixBase aco1 = new ColumnOptimizedMatrix(size, size);
        static MatrixBase aro1 = new RowOptimizedMatrix(size, size);

        public static void Initialize()
        {
            Console.WriteLine("Initializing");
            co1.RandomFill(0, size);
            co2.RandomFill(0, size);
            ro1.RandomFill(0, size);
            ro2.RandomFill(0, size);
            Console.WriteLine("Complete");
        }

        public static double RunTest(IMatrix m1, IMatrix m2, out IMatrix m3)
        {
            m3 = new MatrixBaseNonPerformant(1, 1);
            var allTimes = new List<long>();
            for (int i = 0; i < repeat; i++)
            {
                var sw = Stopwatch.StartNew();
                m3 = m1.Multiply(m2);
                sw.Stop();

                Console.WriteLine($"ms: {sw.ElapsedMilliseconds:#,0.00}");

                allTimes.Add(sw.ElapsedMilliseconds);
            }
            return allTimes.Average();
        }

        public static double RunTest2(IMatrix m1, IMatrix m2, out IMatrix m3)
        {
            m3 = new MatrixBaseNonPerformant(1, 1);
            var allTimes = new List<long>();
            for (int i = 0; i < repeat; i++)
            {
                var sw = Stopwatch.StartNew();
                m3 = m1.Multiply2(m2);
                sw.Stop();

                Console.WriteLine($"ms: {sw.ElapsedMilliseconds:#,0.00}");

                allTimes.Add(sw.ElapsedMilliseconds);
            }
            return allTimes.Average();
        }

        private static void RunATest(IMatrix m1, IMatrix m2, string message)
        {
            Console.WriteLine($"1: Running: {message}", message);
            var allTimes = new List<long>();
            IMatrix results;
            var average = RunTest(m1, m2, out results);
            Console.WriteLine($"{message}: {average:#,0.00}");
        }

        private static void RunATest2(IMatrix m1, IMatrix m2, string message)
        {
            Console.WriteLine($"2: Running: {message}", message);
            var allTimes = new List<long>();
            IMatrix results;
            var average = RunTest2(m1, m2, out results);
            Console.WriteLine($"{message}: {average:#,0.00}");
        }

        private static void Main(string[] args)
        {
            Initialize();

            RunATest(co1, ro1, "1");
            RunATest2(co1, ro1, "2");
            //RunATest(co1, ro1, aro1, "Optimized input and row output");

            //RunATest(co1, co2, aco1, "Non-Optimized, input 2 matrices by column and column output");
            //RunATest(co1, co2, aro1, "Non-Optimized, input 2 matrices by column and row output");

            //RunATest(ro1, ro2, aco1, "Non-Optimized, input 2 matrices by row and column output");
            //RunATest(ro1, ro2, aro1, "Non-Optimized, input 2 matrices by row and row output");

            //Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}