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

        int s = 124075;
        int e = 580769;
        public override string CalculateSimple() => Enumerable.Range(s, e-s).Count(x => S(x+"") && Regex.Matches(x+"", "(.)\\1{1,}").Any())+"";
        public override string CalculateExtended() => Enumerable.Range(s, e-s).Count(x => S(x+"") && Regex.Matches(x+"", "(.)\\1{1,}").Any(m => m.Length == 2))+"";
        private bool S(string s) => string.Concat(s.OrderBy(x => x)).Equals(s);
    }
}