using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processing_Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Match: " + GetMatchValue(Data.original, Data.original));
            Console.WriteLine("Match: " + GetMatchValue(Data.original, Data.plus5));
            Console.WriteLine("Match: " + GetMatchValue(Data.original, Data.minus15));
            Console.WriteLine("Match: " + GetMatchValue(Data.original, Data.abs));
            Console.WriteLine("Match: " + GetMatchValue(Data.original, Data.plusRand10));
            Console.WriteLine("Match: " + GetMatchValue(Data.original, Data.minus15Rand10));
            Console.WriteLine("Match: " + GetMatchValue(Data.original, Data.rand10));
            Console.WriteLine("Match: " + GetMatchValue(Data.original, Data.times3p5));
            Console.WriteLine("Match: " + GetMatchValue(Data.original, Data.times0p25));

            Console.Read();
        }

        static double GetMatchValue(double[] refEvent, double[] current)
        {
            int N = refEvent.Length;
            double R = 0, norm = (1 / (double)N);

            for (int i = 0; i < N; i++)
                R += Math.Abs(refEvent[i] - current[i]);

            R *= norm;

            return R;
        }
    }
}
