using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using AoCCore;

namespace Challenges
{
    public class AoC4 : AoCTool
    {
        public AoC4(int num) : base(num)
        {
        }

        public override string CalculateSimple()
        {
            var c = 0;
            for (var x = 124075; x <= 580769; x++)
            {
                var xs = x + "";
                if (!S(xs))
                    continue;

                if (Regex.Matches(xs, "(.)\\1{1,}").Any())
                    c++;
            }
            return c+"";
        }

        public override string CalculateExtended()
        {
            var c = 0;
            for (var x = 124075; x <= 580769; x++)
            {
                var xs = x + "";
                if (!S(xs))
                    continue;

                if (Regex.Matches(xs, "(.)\\1{1,}").Any(m => m.Length == 2))
                    c++;
            }
            return c+"";
        }

        private bool S(string s)
        {
            return string.Concat(s.OrderBy(x => x)).Equals(s);
        }
    }
}