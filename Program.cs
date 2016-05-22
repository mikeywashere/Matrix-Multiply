﻿using Free.Matrix;
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

        static MatrixBase co1 = new ColumnOptimizedMatrix(size, size);
        static MatrixBase ro1 = new RowOptimizedMatrix(size, size);
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

        public static double RunTest(IMatrix m1, IMatrix m2, IMatrix m3)
        {
            var allTimes = new List<long>();
            for (int i = 0; i < repeat; i++)
            {
                var sw = Stopwatch.StartNew();
                MatrixBase.MatrixMultiply(m1, m2, m3);
                sw.Stop();

                Console.WriteLine($"ms: {sw.ElapsedMilliseconds:#,0.00}");

                allTimes.Add(sw.ElapsedMilliseconds);
            }
            return allTimes.Average();
        }

        private static void RunATest(IMatrix m1, IMatrix m2, IMatrix m3, string message)
        {
            Console.WriteLine($"Running: {message}", message);
            var allTimes = new List<long>();
            var average = RunTest(m1, m2, m3);
            Console.WriteLine($"{message}: {average:#,0.00}");
        }

        private static void Main(string[] args)
        {
            Initialize();

            RunATest(co1, ro1, aco1, "Optimized input and column output");
            RunATest(co1, ro1, aro1, "Optimized input and row output");

            RunATest(co1, co2, aco1, "Non-Optimized, input 2 matrixes by column and column output");
            RunATest(co1, co2, aro1, "Non-Optimized, input 2 matrixes by column and row output");

            RunATest(ro1, ro2, aco1, "Non-Optimized, input 2 matrixes by row and column output");
            RunATest(ro1, ro2, aro1, "Non-Optimized, input 2 matrixes by row and row output");

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}