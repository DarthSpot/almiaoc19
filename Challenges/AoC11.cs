using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AoCCore;

namespace Challenges
{
    public class AoC11 : AoCTool
    {
        public AoC11(int num) : base(num)
        {
        }



        public struct C
        {
            public int X { get; set; }
            public int Y { get; set; }

            public C(int x, int y)
            {
                X = x;
                Y = y;
            }

            public bool Equals(C other)
            {
                return X == other.X && Y == other.Y;
            }

            public override bool Equals(object obj)
            {
                return obj is C other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (X * 397) ^ Y;
                }
            }

            public override string ToString()
            {
                return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
            }
        }

        public override string CalculateSimple()
        {
            var c = Calc(new Memory(GetInput().Split(',').Select(long.Parse))).GetEnumerator();

            var d = 0;
            var dx = new Dictionary<int, Func<C, C>>()
            {
                { 0, o => new C(o.X, o.Y-1) },
                { 1, o => new C(o.X+1, o.Y) },
                { 2, o => new C(o.X, o.Y+1) },
                { 3, o => new C(o.X-1, o.Y) },
            };

            var s = new C(0, 0);
            var h = new Dictionary<C, int>();

            Input.Enqueue(1);
            c.MoveNext();
            do
            {
                h[s] = (int)c.Current;
                c.MoveNext();
                var dir = (int)c.Current == 0 ? -1 : 1;
                d = (dir + d) % 4;
                if (d < 0)
                    d += 4;
                s = dx[d](s);
                var px = h.ContainsKey(s) ? h[s] : 0;
                Input.Enqueue(px);
            } while (c.MoveNext());

            var ml = h.Keys.Select(x => x.X).Min();
            var mr = h.Keys.Select(x => x.X).Max();
            var mu = h.Keys.Select(x => x.Y).Min();
            var mb = h.Keys.Select(x => x.Y).Max();

            var width = mr - ml + 1;
            var height = mb - mu + 1;

            var b = new Bitmap(width, height);
            foreach (var e in h)
            {
                b.SetPixel(e.Key.X - ml, e.Key.Y-mu, e.Value == 1 ? Color.White : Color.Black);
            }
            b.Save("out.bmp");

            return h.Count + "";
        }

        public Queue<long> Input { get; } = new Queue<long>();

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
                        S(1, Input.Dequeue());
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