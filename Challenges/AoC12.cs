using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using AoCCore;

namespace Challenges
{
    public class AoC12 : AoCTool
    {
        public AoC12(int num) : base(num)
        {
        }

        public override string CalculateSimple()
        {
            var moons = GetInputArr().Select(x => Regex.Match(x, "x=(?<x>[0-9-]+), y=(?<y>[0-9-]+), z=(?<z>[0-9-]+)>")).Select(x => new Moon(x)).ToList();
            for (var i = 0; i < 1000; i++)
            {
                foreach (var m in moons)
                {
                    foreach (var om in moons.Where(x => x != m))
                    {
                        m.V = m.V.Add(Diff(m.P, om.P));
                    }
                }

                foreach (var m in moons)
                {
                    m.Move();
                }
            }

            return moons.Select(x => x.Energy()).Sum()+"";
        }

        private Vector Diff(Vector x, Vector y)
        {
            return new Vector(y.X.CompareTo(x.X), y.Y.CompareTo(x.Y), y.Z.CompareTo(x.Z));
        }

        public enum MoonKind
        {
            X = 0,
            Y = 1,
            Z = 2,
            VX = 3,
            VY = 4,
            VZ = 5
        }

        public override string CalculateExtended()
        {
            var moons = GetInputArr()
                .Select(x => Regex.Match(x, "x=(?<x>[0-9-]+), y=(?<y>[0-9-]+), z=(?<z>[0-9-]+)>")).Select(x => new Moon(x)).ToList();
            var counter = new List<long>() {0,0,0};
            var ms = new List<int>() {0,1,2,3};
            var vxs = new List<int>() {0,1,2};
            var done = new List<bool>() {false, false, false};
            var moonPairs = GetPermutations(2, moons.ToArray()).ToList();
            var i = 0L;
            var initState = moons.Select(x => x.Clone()).ToArray();

            while (!done.All(x => x))
            {
                foreach (var mp in moonPairs)
                {
                    var p = mp[0];
                    var q = mp[1];
                    p.V = p.V.Add(Diff(p.P, q.P));
                }

                moons.ForEach(x => x.Move());
                foreach (var vx in vxs.Where(x => !done[x]))
                    counter[vx] = counter[vx] + 1;

                foreach (var vx in vxs)
                {
                    // All axis the same
                    if (!done[vx] && ms.All(m => initState[m].P[vx] == moons[m].P[vx] && initState[m].V[vx] == moons[m].V[vx]))
                    {
                        done[vx] = true;
                    }
                }
            }

            return counter.Aggregate(lcm) + "";
        }

        /// <summary>
        /// https://stackoverflow.com/questions/1952153/what-is-the-best-way-to-find-all-combinations-of-items-in-an-array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        static IEnumerable<T[]>
            GetPermutations<T>(int length, params T[] list)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutations(length - 1, list)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new T[] { t2 }).ToArray());
        }
        
        private long gcf(long a, long b)
        {
            while (b != 0)
            {
                var temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private long lcm(long a, long b)
        {
            return (a / gcf(a, b)) * b;
        }
    }

    public class Moon
    {
        public Vector P { get; private set; }
        public Vector V { get; set; }
        
        public int Energy() => P.Energy() * V.Energy();
        
        public Moon(Match m)
        {
            P = new Vector(int.Parse(m.Groups["x"].Value), int.Parse(m.Groups["y"].Value), int.Parse(m.Groups["z"].Value));
            V = new Vector(0,0,0);
        }

        private Moon(Vector p, Vector v)
        {
            P = p;
            V = v;
        }

        public void Move()
        {
            P = P.Add(V);
        }

        protected bool Equals(Moon other)
        {
            return P.Equals(other.P) && V.Equals(other.V);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Moon) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (P.GetHashCode() * 397) ^ V.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"pos=<x={P.X}, y={P.Y}, z={P.Z}>, vel=<x={V.X}, y={V.Y}, z={V.Z}>";
        }
        
        public Moon Clone()
        {
            return new Moon(P, V);
        }
    }

    public struct Vector
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        public int Energy() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        public Vector(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector Add(Vector v)
        {
            return new Vector(X + v.X, Y + v.Y, Z + v.Z);
        }

        public int this[int key]
        {
            get
            {
                return key switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    _ => throw new Exception()
                };
            }
        }

        public bool Equals(Vector other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                return hashCode;
            }
        }
    }
}