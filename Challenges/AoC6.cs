using System.Collections.Generic;
using System.Linq;
using AoCCore;

namespace Challenges
{
    public class AoC6 : AoCTool
    {
        public AoC6(int num) : base(num)
        {
        }

        public override string CalculateSimple()
        {
            var l = GetInputArr().Select(x => x.Split(')')).ToLookup(x => x[0], x => x[1]);
            return l.Select(x => C(l,x.Key)).Sum()+"";
        }

        private int C(ILookup<string, string> l, string r)
        {
            if (!l.Contains(r))
                return 0;
            return l[r].Count() + l[r].Select(x => C(l, x)).Sum();
        }

        public override string CalculateExtended()
        {
            var l = GetInputArr().Select(x => x.Split(')')).ToDictionary(x => x[1], x => x[0]);
            var yp = P(l,"YOU").ToList();
            var sp = P(l,"SAN").ToList();
            var m = yp.First(n => sp.Contains(n));
            return ""+(yp.TakeWhile(n => n != m).Count() + sp.TakeWhile(n => n != m).Count());

        }

        private IEnumerable<string> P(Dictionary<string, string> d, string x)
        {
            while (d.ContainsKey(x))
            {
                yield return x = d[x];
            }
        }
    }
}