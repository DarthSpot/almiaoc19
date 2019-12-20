using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using AoCCore;

namespace Challenges
{
    public class AoC16 : AoCTool
    {
        public AoC16(int num) : base(num)
        {
        }

        public override string CalculateSimple()
        {
            var origSignal = GetInput().Trim().Select(x => x-'0').ToList();
            var signal = origSignal;
            for (var i = 0; i < 100; ++i)
            {
                var n = 0;
                signal = signal.Select(x => (signal.Zip(Pattern(++n).Skip(1), (a, b) => a * b).Sum()+"").Last()-'0').ToList();
            }
            return string.Concat(signal.Take(8));
        }

        private IEnumerable<int> Pattern(int level)
        {
            var p = new[] {0, 1, 0, -1};
            while (true)
            {
                foreach (var e in p)
                {
                    foreach (var r in Enumerable.Repeat(e, level))
                        yield return r;
                }
            }
        }

        public override string CalculateExtended()
        {
            var origSignal = GetInput().Trim().Select(x => (x-'0')).ToList();
            var skippy = int.Parse(string.Concat(origSignal.Take(7)));
            var signal = Enumerable.Repeat(origSignal, 10000).SelectMany(x => x).ToList();
            var l = signal.Count;
            for (var i = 0; i < 100; ++i)
            {
                var sum = signal.Skip(skippy).Sum();
                signal = signal.Skip(skippy).Select(x =>
                {
                    var r = Math.Abs(sum) % 10;
                    sum -= x;
                    return r;
                }).ToList();
                skippy = 0;
            }
            return string.Concat(signal.Take(8));
        }
    }
}