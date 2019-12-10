using System;
using System.Collections.Generic;
using System.Linq;
using AoCCore;

namespace Challenges
{
    public class AoC10 : AoCTool
    {
        public AoC10(int num) : base(num)
        {
        }

        public class Vector
        {
            public int X { get; }
            public int Y { get; }

            public Vector(int x, int y)
            {
                X = x;
                Y = y;
            }

            public Vector(Vector x, Vector y)
            {
                X = y.X-x.X;
                Y = (-1)*(y.Y-x.Y);
                From = x;
                To = y;
            }

            public Vector From { get; }

            public Vector To { get; }

            public int Distance(Vector x)
            {
                return Math.Abs(x.X - X) + Math.Abs(x.Y - Y);
            }
            
            protected bool Equals(Vector other)
            {
                return X == other.X && Y == other.Y;
            }

            public double Angle
            {
                get
                {
                    var an = Math.Atan2(X, Y);
                    return ((an > 0 ? an : Math.PI * 2 + an) * 360 / (2 * Math.PI))%360;
                }
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Vector) obj);
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
                return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Angle)}: {Angle}";
            }
        }
        
        public override string CalculateSimple()
        {
            var d = GetInputArr();
            var field = new List<Vector>();
            for (var y=0; y < d.Length; ++y)
            {
                var line = d[y];
                for (var x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                        field.Add(new Vector(x,y));
                }
            }
            
            var (key, value) = field.ToDictionary(x => x, x => GetViews(x, field)).OrderBy(x => x.Value).Last();

            return key + " " + value;
        }

        private int GetViews(Vector start, List<Vector> field)
        {
            var ds = field.Where(x => !Equals(x,start)).Select(x => new Vector(start, x).Angle).OrderBy(x => x).ToList();
            return ds.Distinct().Count();
        }

        public override string CalculateExtended()
        {
            var d = GetInputArr();
            var field = new List<Vector>();
            for (var y = 0; y < d.Length; ++y)
            {
                var line = d[y];
                for (var x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                        field.Add(new Vector(x, y));
                }
            }

            var station = field.ToDictionary(x => x, x => GetViews(x, field)).OrderBy(x => x.Value).Last().Key;

            var a = 0;
            while (field.Count() > 1)
            {
                var afield = field.Where(x => !Equals(station, x))
                    .Select(x => new Vector(station, x))
                    .OrderBy(x => x.Angle)
                    .ToLookup(x => x.Angle, x => x)
                    .Select(x => x.OrderBy(y => Math.Abs(y.X) + Math.Abs(y.Y)).First()).ToList();

                foreach (var asteroid in afield)
                {
                    field.Remove(asteroid.To);
                    if (++a == 200)
                        return $"{asteroid.To.X * 100 + asteroid.To.Y}";
                }
            }

            return "";
        }
    }


}