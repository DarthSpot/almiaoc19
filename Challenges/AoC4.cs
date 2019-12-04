using System;
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
                if (!string.Equals(string.Concat(x.ToString().OrderBy(o => o)), x.ToString()))
                    continue;

                var xr = x.ToString().ToCharArray();
                var pr = new[] {0, 1, 2, 3, 4};
                if (pr.Any(q => xr[q] == xr[q+1]))
                    c++;
            }


            return c.ToString();

        }

        public override string CalculateExtended()
        {
            var c = 0;
            for (var x = 124075; x <= 580769; x++)
            {
                var xs = x.ToString();
                if (!string.Equals(string.Concat(xs.OrderBy(o => o)), xs))
                    continue;

                var xr = x.ToString().ToCharArray();
                var pr = new[] {0, 1, 2, 3, 4};
                if (pr.Any(pq =>
                    xr[pq] == xr[pq + 1] && (pq == 0 || xr[pq - 1] != xr[pq]) && (pq == 4 || xr[pq + 2] != xr[pq])))
                    c++;
            }


            return c.ToString();
        }
    }
}