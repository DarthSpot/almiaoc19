using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AoCCore;

namespace Challenges
{
    public class AoC13 : AoCTool
    {
        public AoC13(int num) : base(num)
        {
        }

        public override string CalculateSimple()
        {
            var c = Calc(new Memory(GetInput().Split(',').Select(long.Parse))).ToList();
            var field = new List<(long, long, long)>();
            for (var x = 0; x < c.Count; x += 3)
                field.Add((c[x], c[x+1], c[x+2]));

            return field.Count(x => x.Item3 == 2) + "";
        }

        public override string CalculateExtended()
        {
            var score = 0L;
            var i = new Regex("abc");

            Console.Clear();
            var f = false;
            var m = new Memory(GetInput().Split(',').Select(long.Parse));
            m[0] = 2;
            var c = Calc(m).GetEnumerator();
            var paddlePos = -1L;
            var wt = 0;
            while (c.MoveNext())
            {
                var x = c.Current;
                c.MoveNext();
                var y = c.Current;
                c.MoveNext();
                var v = c.Current;

                if (x == -1 && y == 0 && v != 0)
                    score = v;
                else if (x > 0 && y > 0)
                {
                    Console.SetCursorPosition((int) x, (int) y);
                    switch (v)
                    {
                        case 0:
                            Console.Write(' ');
                            break;
                        case 1:
                            Console.Write('[');
                            break;
                        case 2:
                            Console.Write('X');
                            break;
                        case 3:
                            Console.Write('T');
                            wt = 5;
                            paddlePos = x;
                            break;
                        case 4:
                            Console.Write('O');
                            if (x > paddlePos)
                                Input = 1;
                            else if (x < paddlePos)
                                Input = -1;
                            else
                                Input = 0;
                            break;
                    }
                }
                Thread.Sleep(wt);
            }
            Console.WriteLine();
            return score+"";
        }

        public long Input { get; set; } = 0;

        public IEnumerable<long> Calc(Memory memory)
        {
            var p = 0;
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
                            return memory[(int)memory[p + o]];
                        case '1':
                            return memory[p + o];
                        case '2':
                            return memory[(int)(rb + memory[p + o])];
                        default:
                            throw new Exception();
                    }
                }

                void S(int o, long v)
                {
                    switch (args[o - 1])
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
                        S(1, Input);
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
                    p += new[] { 1, 4, 4, 2, 2, 3, 3, 4, 4, 2 }[op];
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