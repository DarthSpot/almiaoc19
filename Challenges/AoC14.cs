using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AoCCore;

namespace Challenges
{
    public class AoC14 : AoCTool
    {
        public AoC14(int num) : base(num)
        {
        }

        public override string CalculateSimple()
        {
            var input = GetInputArr().Select(x => Regex.Match(x, "( ?(?<x>\\d+ [A-Z]+),?)+ => (?<y>\\d+ [A-Z]+)"))
                .ToDictionary(x =>
                {
                    var e = x.Groups["y"].Value.Split(' '); 
                    return (c: long.Parse(e[0]), n: e[1]);
                }, y => y.Groups["x"].Captures.Select(x =>
                {
                    var e = x.Value.Split(' ');
                    return (c: long.Parse(e[0]), n: e[1]);
                }).ToList());
            var inv = new Inventory();
            inv["ORE"] = 0;
            GenerateFuel(input, inv, "FUEL", 1);
            return Math.Abs(inv["ORE"])+"";
        }

        private void GenerateFuel(Dictionary<(long c, string n), List<(long c, string n)>> tree, Inventory inv, string p, long count)
        {
            if (p == "ORE")
            {
                inv[p] -= count;
            }
            else
            {
                var x = tree.Single(t => string.Equals(t.Key.n, p));

                if (inv[p] < count)
                {
                    var produceAmount = S(count - inv[p], x.Key.c);
                    foreach (var v in x.Value)
                    {
                        GenerateFuel(tree, inv, v.n, v.c*produceAmount);
                    }

                    inv[p] += produceAmount * x.Key.c;
                }

                inv[p] -= count;
            }
        }

        private long S(long count, long steps)
        {
            var div = (double) count / steps;
            return (long) Math.Ceiling(div);
        }

        public override string CalculateExtended()
        {
            var input = GetInputArr().Select(x => Regex.Match(x, "( ?(?<x>\\d+ [A-Z]+),?)+ => (?<y>\\d+ [A-Z]+)"))
                .ToDictionary(x =>
                {
                    var e = x.Groups["y"].Value.Split(' '); 
                    return (c: long.Parse(e[0]), n: e[1]);
                }, y => y.Groups["x"].Captures.Select(x =>
                {
                    var e = x.Value.Split(' ');
                    return (c: long.Parse(e[0]), n: e[1]);
                }).ToList());

            var ore = 0L;
            var f = 100000L;
            var oreMax = 1000000000000;
            var bigger = 0L;
            var smaller = 0L;
            while (bigger != smaller +1)
            {
                var inv = new Inventory();
                GenerateFuel(input, inv, "FUEL", f);
                ore = Math.Abs(inv["ORE"]);

                if (bigger <= 0L)
                {
                    if (ore < oreMax)
                    {
                        smaller = f;
                        f *= 10;
                    }
                    else
                    {
                        bigger = f;
                        f /= 2;
                    }
                }
                else
                {
                    if (ore > oreMax)
                    {
                        if (f < bigger)
                        {
                            bigger = f;
                        }

                        f = f - (bigger - smaller) / 2;

                    }
                    else if (ore < oreMax)
                    {
                        if (f > smaller)
                            smaller = f;

                        f = f + (bigger - smaller) / 2;
                    }
                }
            }

            return smaller+"";
        }
    }

    public class Inventory : Dictionary<string, long>
    {
        public new long this[string key]
        {
            get
            {
                 if (!ContainsKey(key))
                     Add(key, 0);
                 return base[key];
            }
            set => base[key] = value;
        }
    }
}