using System;
using System.Collections.Generic;
using System.Linq;
using AoCCore;

namespace Challenges
{
    public class AoC3 : AoCTool
    {
        public AoC3(int num) : base(num)
        {
        }

        public override string CalculateSimple()
        {
            var input = GetInputArr();
            var a = input[0].Split(',');
            var b = input[1].Split(',');
            return ToPath(a).Intersect(ToPath(b)).Select(x => Math.Abs(x.RelativeX) + Math.Abs(x.RelativeY)).OrderBy(x => x).First()
                .ToString();
        }

        private Dictionary<char, Func<Coord, Coord>> pd = new Dictionary<char, Func<Coord, Coord>>()
        {
            {'U', x => new Coord(x.RelativeX, x.RelativeY+1) },
            {'D', x => new Coord(x.RelativeX, x.RelativeY-1) },
            {'R', x => new Coord(x.RelativeX+1, x.RelativeY) },
            {'L', x => new Coord(x.RelativeX-1, x.RelativeY) }
        };

        private IEnumerable<Coord> ToPath(string[] path)
        {
            var p = new Coord(0, 0);
            foreach (var e in path)
            {
                var dir = e[0];
                var len = int.Parse(string.Concat(e.Skip(1)));
                for (var i = len; i > 0; i--)
                {
                    p = pd[dir](p);
                    yield return p;
                }
            }
        }

        private IEnumerable<Tuple<Coord, int>> ToPath2(string[] path)
        {
            var p = new Coord(0, 0);
            var s = 0;
            foreach (var e in path)
            {
                var dir = e[0];
                var len = int.Parse(string.Concat(e.Skip(1)));
                for (var i = len; i > 0; i--)
                {
                    p = pd[dir](p);
                    yield return new Tuple<Coord, int>(p, ++s);
                }
            }
        }

        public override string CalculateExtended()
        {
            var input = GetInputArr();
            var a = input[0].Split(',');
            var b = input[1].Split(',');
            var ap = ToPath2(a).ToLookup(x => x.Item1, x => x.Item2).ToDictionary(x => x.Key, x => x.OrderBy(p => p).First());
            var bp = ToPath2(b).ToLookup(x => x.Item1, x => x.Item2).ToDictionary(x => x.Key, x => x.OrderBy(p => p).First());
            return ap.Keys.Intersect(bp.Keys).Select(x => ap[x] + bp[x]).OrderBy(x => x).First().ToString();
        }

        private class Coord
        {
            public Coord(int rx, int ry)
            {
                RelativeX = rx;
                RelativeY = ry;
            }
            
            public int RelativeX { get; set; }

            public int RelativeY { get; set; }

            protected bool Equals(Coord other)
            {
                return RelativeX == other.RelativeX && RelativeY == other.RelativeY;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Coord) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (RelativeX * 397) ^ RelativeY;
                }
            }
        }
    }
}