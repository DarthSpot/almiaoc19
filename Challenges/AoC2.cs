using System;
using System.Collections.Generic;
using System.Linq;
using AoCCore;

namespace Challenges
{
    public class AoC2 : AoCTool
    {
        public AoC2(int num) : base(num)
        {
            
        }

        private static Func<int, int, int> o(int c) => c switch
            {
                1 => (x, y) => x + y,
                2 => (x, y) => x * y,
            };


        public override string CalculateSimple()
        {
            var i = 0;
            var p = 0;
            var d = GetInput().Split(',').ToDictionary(x => i++, int.Parse);
            d[1] = 12;
            d[2] = 2;
            while (d[p]!=99)
            {
                d[d[p + 3]] = o(d[p])(d[d[p + 1]], d[d[p + 2]]);
                p += 4;
            }

            return d[0].ToString();
        }

        public override string CalculateExtended()
        {
            
            var data = GetInput().Split(',');
            for (var a = 0; a < 100; a++)
            {
                for (var b = 0; b < 100; b++)
                {
                    var i = 0;
                    var p = 0;
                    var d = data.ToDictionary(x => i++, int.Parse);
                    d[1] = a;
                    d[2] = b;
                    while (d[p] != 99)
                    {
                        d[d[p + 3]] = o(d[p])(d[d[p + 1]], d[d[p + 2]]);
                        p += 4;
                    }

                    if (d[0] == 19690720)
                        return (100 * a + b).ToString();
                }
            }

            return null;
        }
    }

}