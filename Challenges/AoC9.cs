using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AoCCore;

namespace Challenges
{
    public class AoC9 : AoCTool
    {
        public AoC9(int i) : base(i)
        {
        }

        public override string CalculateSimple()
        {
            return Calc(new Memory(GetInput().Split(',').Select(long.Parse)), 1).Last()+"";
        }

        public override string CalculateExtended()
        {
            return Calc(new Memory(GetInput().Split(',').Select(long.Parse)), 2).Last() + "";
        }

        public IEnumerable<long> Calc(Memory memory, params long[] input)
        {
            var p = 0;
            var ic = 0;
            var rb = 0;
            while (memory[p] != 99)
            {
                var jp = -1;
                var ins = memory[p];
                var op = ins % 100;
                var args = (ins + "").PadLeft(5, '0').Reverse().Skip(2).ToArray();
                long P(int o)
                {
                    switch (args[o - 1])
                    {
                        case '0':
                            return memory[(int) memory[p + o]];
                        case '1':
                            return memory[p + o];
                        case '2':
                            return memory[(int) (rb + memory[p + o])];
                        default:
                            throw new Exception();
                    }
                }

                void S(int o, long v)
                {
                    switch (args[o-1])
                    {
                        case '0':
                            memory[(int)memory[p + o]] = v;
                            break;
                        case '2':
                            memory[(int)(rb + memory[p + o])] = v;
                            break;
                    }
                }

                switch (op)
                {
                    case 1:
                        S(3, P(1) + P(2));
                        break;
                    case 2:
                        S(3, P(1) * P(2));
                        break;
                    case 3:
                        S(1, input[ic++]);
                        break;
                    case 4:
                        yield return P(1);
                        break;
                    case 5:
                        if (P(1) != 0)
                            jp = (int)P(2);
                        break;
                    case 6:
                        if (P(1) == 0)
                            jp = (int)P(2);
                        break;
                    case 7:
                        S(3, P(1) < P(2) ? 1 : 0);
                        break;
                    case 8:
                        S(3, P(1) == P(2) ? 1 : 0);
                        break;
                    case 9:
                        rb += (int)P(1);
                        break;
                }

                if (jp >= 0)
                    p = jp;
                else
                    p += new[] { 1, 4, 4, 2, 2, 3, 3, 4, 4, 2}[op];
            }
        }

        public class Memory : Dictionary<int, long>
        {
            public Memory(IEnumerable<long> items)
            {
                var i = 0;
                foreach (var item in items)
                    Add(i++, item);
            }

            public new long this[int index]
            {
                get
                {
                     if (!ContainsKey(index))
                         Add(index, 0);
                     return base[index];
                }
                set => base[index] = value;
            }
        }
    }
}