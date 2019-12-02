using System;
using System.Diagnostics;
using Challenges;

namespace AoCRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new TaskSelector();

            var task = 2;

            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine(t[task].CalculateSimple());
            Console.WriteLine(sw.Elapsed);
            sw.Restart();
            Console.WriteLine(t[task].CalculateExtended());
            Console.WriteLine(sw.Elapsed);
            sw.Stop();

            Console.ReadKey();
        }
    }


}
