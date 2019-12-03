using System;
using System.Linq;
using AoCCore;

namespace Challenges
{
    public class AoC1 : AoCTool
    {
        public AoC1(int num) : base(num)
        {
        }

        public override string CalculateSimple()
        {
            return GetInputArr().Select(x => long.Parse(x) / 3 - 2).Sum().ToString();
        }

        public override string CalculateExtended()
        {
            return GetInputArr().Select(x => F(long.Parse(x))).Sum().ToString();
        }

        private static long F(long x)
        {
            var res = x / 3 - 2;
            return res <= 0 ? 0 : res + F(res);
        }
    }
}