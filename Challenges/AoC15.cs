using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AoCCore;

namespace Challenges
{
    public class AoC15 : AoCTool
    {
        public AoC15(int num) : base(num)
        {
        }

        public override string CalculateSimple()
        {
            var m = new Memory(GetInput().Split(',').Select(long.Parse));

            (int, int) P((int, int) p, int dir) => dir switch
                {
                1 => (p.Item1, p.Item2 - 1),
                2 => (p.Item1, p.Item2 + 1),
                3 => (p.Item1 - 1, p.Item2),
                4 => (p.Item1 + 1, p.Item2),
                };

            var walls = new HashSet<(int, int)>();
            var d = new Dictionary<(int, int), (Memory, int, int)>();
            var beenThere = new Dictionary<(int,int), int>();
            d.Add((0, 0), (m, 0, 1));

            while (d.All(x => x.Value.Item3 != 2))
            {
                foreach (var mem in d.Where(x => x.Value.Item3 == 1).ToArray())
                {
                    for (var e = 0; e < 4; e++)
                    {
                        var np = P(mem.Key, e + 1);
                        if (!beenThere.ContainsKey(np))
                        {
                            var mc = mem.Value.Item1.Clone();
                            mc.Input.Enqueue(e + 1);
                            var status = Calc(mc).First();
                            var l = mem.Value.Item2 + 1;
                            if (!d.ContainsKey(np) || d[np].Item2 > l)
                                d.Add(np, (mc, l, (int)status));
                        }
                    }

                    beenThere.Add(mem.Key, mem.Value.Item3);
                    d.Remove(mem.Key);
                }
            }
            return d.Single(x => x.Value.Item3 == 2).Value.Item2+"";
        }
        
        public override string CalculateExtended()
        {
            var m = new Memory(GetInput().Split(',').Select(long.Parse));

            (int, int) P((int, int) p, int dir) => dir switch
                {
                1 => (p.Item1, p.Item2 - 1),
                2 => (p.Item1, p.Item2 + 1),
                3 => (p.Item1 - 1, p.Item2),
                4 => (p.Item1 + 1, p.Item2),
                };

            var walls = new HashSet<(int, int)>();
            var d = new Dictionary<(int, int), (Memory, int)>();
            var beenThere = new Dictionary<(int,int), int>();
            d.Add((0, 0), (m, 1));

            while (d.Any())
            {
                foreach (var mem in d.ToArray())
                {
                    for (var e = 0; e < 4; e++)
                    {
                        var np = P(mem.Key, e + 1);
                        if (!beenThere.ContainsKey(np))
                        {
                            var mc = mem.Value.Item1.Clone();
                            mc.Input.Enqueue(e + 1);
                            var status = Calc(mc).First();
                            var l = mem.Value.Item2 + 1;
                            beenThere.Add(np, (int)status);
                            if (status != 0 && !d.ContainsKey(np))
                                d.Add(np, (mc, (int)status));
                        }
                    }
                    d.Remove(mem.Key);
                }
            }

            var s = beenThere.Single(x => x.Value == 2).Key;
            var tmp = new HashSet<(int,int)>();
            var flooded = new HashSet<(int,int)>();
            tmp.Add(s);
            var steps = 0;
            while (tmp.Any())
            {
                foreach (var p in tmp.ToArray())
                {
                    for (var e = 0; e < 4; e++)
                    {
                        var np = P(p, e + 1);
                        if (!flooded.Contains(np))
                        {
                            if (beenThere[np] > 0)
                                tmp.Add(np);
                        }
                    }
                    tmp.Remove(p);
                    flooded.Add(p);
                }
                steps++;
            }
            
            return steps-1+"";
        }

        
        public IEnumerable<long> Calc(Memory memory)
        {
            var output = new Queue<long>();
            while (memory[memory.Pointer] != 99)
            {
                var jp = -1;
                var ins = memory[memory.Pointer];
                var op = ins % 100;
                var args = (ins + "").PadLeft(5, '0').Reverse().Skip(2).ToArray();
                long P(int o)
                {
                    switch (args[o - 1])
                    {
                        case '0':
                            return memory[(int)memory[memory.Pointer + o]];
                        case '1':
                            return memory[memory.Pointer + o];
                        case '2':
                            return memory[(int)(memory.BaseValue + memory[memory.Pointer + o])];
                        default:
                            throw new Exception();
                    }
                }

                void S(int o, long v)
                {
                    switch (args[o - 1])
                    {
                        case '0':
                            memory[(int)memory[memory.Pointer + o]] = v;
                            break;
                        case '2':
                            memory[(int)(memory.BaseValue + memory[memory.Pointer + o])] = v;
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
                        S(1, memory.Input.Dequeue());
                        break;
                    case 4:
                        output.Enqueue(P(1));
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
                        memory.BaseValue += (int)P(1);
                        break;
                }

                if (jp >= 0)
                    memory.Pointer = jp;
                else
                    memory.Pointer += new[] { 1, 4, 4, 2, 2, 3, 3, 4, 4, 2 }[op];

                if (output.Any())
                    yield return output.Dequeue();

            }
        }

        public class Memory : Dictionary<int, long>
        {
            public int Pointer { get; set; } = 0;
            public int BaseValue { get; set; } = 0;
            private long _accesses = 0;
            public Queue<long> Input { get; } = new Queue<long>();

            public Memory(Memory other)
            {
                Pointer = other.Pointer;
                BaseValue = other.BaseValue;
                _accesses = other._accesses;
                foreach (var e in other)
                    Add(e.Key, e.Value);
            }

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
                    _accesses++;
                    if (!ContainsKey(index))
                        Add(index, 0);
                    return base[index];
                }
                set => base[index] = value;
            }

            public Memory Clone()
            {
                return new Memory(this);
            }

            public override string ToString()
            {
                return $"{nameof(_accesses)}: {_accesses}, {nameof(Pointer)}: {Pointer}, {nameof(BaseValue)}: {BaseValue}";
            }
        }
    }
}