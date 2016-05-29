using Free.Matrix;
using Matrix.UnitTests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace CSharp.Multithread.Matrix.Multiply
{
    public class Program
    {
        const int repeat = 3;
        const int size = 1000;

        public static double RunTest(MatrixBase m1, MatrixBase m2, MatrixType type)
        {
            var allTimes = new List<long>();
            for (int i = 0; i < repeat; i++)
            {
                var sw = Stopwatch.StartNew();

                MatrixBase result = null;

                result = m1.Multiply(m2, type);

                sw.Stop();

                Console.WriteLine($"ms: {sw.ElapsedMilliseconds:#,0.00}");

                allTimes.Add(sw.ElapsedMilliseconds);
            }
            return allTimes.Average();
        }

        private static double RunATest(MatrixBase m1, MatrixBase m2, MatrixType type, string message)
        {
            Console.WriteLine($"Running: {message}");
            var allTimes = new List<long>();
            var average = Math.Round(RunTest(m1, m2, type), 2);
            Console.WriteLine($"{message}: {average:#,0.00}\r\n");
            return average;
        }

        private static Dictionary<string, double> scores = new Dictionary<string, double>();

        private static void Main(string[] args)
        {
            var types = from item in MatrixFactory.Types()
                        // where item != MatrixType.NonOptimized 
                        select item;
            foreach (var type1 in types)
            {
                var m1 = MatrixFactory.Create(size, size, type1);
                foreach (var type2 in types)
                {
                    var m2 = MatrixFactory.Create(size, size, type2);
                    foreach (var type in types)
                    {
                        MatrixType t = MatrixType.NonOptimized;

                        var key = $"{t} = {m1.Type} * {m2.Type}";
                        if (scores.ContainsKey(key))
                            continue;

                        m1.RandomFill(0, Convert.ToDouble(size));
                        m2.RandomFill(0, Convert.ToDouble(size));
                        
                        var avg = RunATest(m1, m2, t, $"{m1.Type} * {m2.Type} = {type}");

                        scores.Add(key, avg);
                    }
                }
            }

            Console.WriteLine("\r\n----------------------------------------\r\nScore:\r\n");
            var l = from item in scores orderby item.Value select item;

            foreach (var item in l)
            {
                Console.WriteLine($"{item.Key} - {item.Value:#,0.00}");
            }

            Console.WriteLine("\r\n----------------------------------------\r\n");

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}