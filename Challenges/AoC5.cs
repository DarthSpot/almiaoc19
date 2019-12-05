using System;
using System.Collections.Generic;
using System.Linq;
using AoCCore;

namespace Challenges
{
    public class AoC5 : AoCTool
    {
        public AoC5(int num) : base(num)
        {
        }

        public override string CalculateSimple()
        {
            return Calc(1);
        }
        
        public override string CalculateExtended()
        {
            return Calc(5);
        }

        private string Calc(int input)
        {
            var i = 0;
            var d = GetInput().Split(',').ToDictionary(x => i++, int.Parse);
            var output = string.Empty;
            var p = 0;
            while (d[p] != 99)
            {
                var jp = -1;
                var ins = d[p];
                var op = ins % 100;
                var args = (ins + "").PadLeft(5, '0').Reverse().Skip(2).ToArray();
                int P(int o) => args[o - 1] == '0' ? d[d[p + o]] : d[p + o];
                void S(int o, int v) => d[d[o]] = v;

                switch (op)
                {
                    case 1:
                        S(p + 3, P(1) + P(2));
                        break;
                    case 2:
                        S(p + 3, P(1) * P(2));
                        break;
                    case 3:
                        S(p + 3, input);
                        break;
                    case 4:
                        output = P(1) + "";
                        break;
                    case 5:
                        if (P(1) != 0)
                            jp = P(2);
                        break;
                    case 6:
                        if (P(1) == 0)
                            jp = P(2);
                        break;
                    case 7:
                        S(p + 3, P(1) < P(2) ? 1 : 0);
                        break;
                    case 8:
                        S(p + 3, P(1) == P(2) ? 1 : 0);
                        break;
                }

                if (jp >= 0)
                    p = jp;
                else
                    p += new[] {1, 4, 4, 2, 2, 3, 3, 4, 4}[op];
            }
            return output;
        }
    }
}