using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AoCCore;

namespace Challenges
{
    public class AoC17 : AoCTool
    {
        public AoC17(int num) : base(num)
        {
        }

        public override string CalculateSimple()
        {
            var m = new Memory(GetInput().Split(',').Select(long.Parse));
            var x = 0;
            var y = 0;
            var d = new Dictionary<(int,int), char>();
            foreach (var e in Calc(m))
            {
                if (e == 10)
                {
                    y++;
                    x=0;
                }
                else
                {
                    d.Add((x++,y), (char)e);
                }
                Console.Write((char)e);
            }
            

            return d.Where(c => c.Value == '#' && IsIntersection(d,c.Key)).Sum(c => c.Key.Item1 * c.Key.Item2)+"";
        }

        private bool IsIntersection(Dictionary<(int, int), char> d, (int, int) pos)
        {
            var ns = new []
            {
                (pos.Item1, pos.Item2 + 1),
                (pos.Item1, pos.Item2 - 1),
                (pos.Item1 + 1, pos.Item2),
                (pos.Item1 - 1, pos.Item2),
            };
            return (ns.All(n => d.ContainsKey(n) && d[n] == d[pos]));
        }

        public override string CalculateExtended()
        {
            var m = new Memory(GetInput().Split(',').Select(long.Parse));
            var x = 0;
            var y = 0;
            var d = new Dictionary<(int,int), char>();
            foreach (var e in Calc(m))
            {
                if (e == 10)
                {
                    y++;
                    x=0;
                }
                else
                {
                    d.Add((x++,y), (char)e);
                }
            }

            var paths = FindAllPaths(d);

            return "";
        }

        private List<Path> FindAllPaths(Dictionary<(int, int), char> map)
        {
            var start = map.Single(x => x.Value == '^');
            var dir = 1;
            var paths = new ConcurrentQueue<Path>();
            paths.Enqueue(new Path(start.Key));

            IEnumerable<(int,int)> GetNeighbours(Path path)
            {
                var pos = path.CurrentPosition;

                if (path.Direction != 1)
                    yield return (pos.Item1, pos.Item2 + 1);
                if (path.Direction != 3)
                    yield return (pos.Item1, pos.Item2 - 1);
                if (path.Direction != 4)
                    yield return (pos.Item1 + 1, pos.Item2);
                if (path.Direction != 2)
                    yield return (pos.Item1 - 1, pos.Item2);
            };

            IEnumerable<(int,int)> FilterNeighbours(Path p)
            {
                return GetNeighbours(p).Where(x =>
                {
                    var isValid = map.ContainsKey(x) && map[x] == '#';
                    if (!isValid)
                        return false;
                    var hasVisited = p.Visited.Contains(x);
                    var isIntersection = IsIntersection(map, x);

                    return isIntersection || !hasVisited;
                });
            }

            var finishedPaths = new List<Path>();
            Console.WriteLine(map.Count(x => x.Value == '#'));
            while (!finishedPaths.Any())
            {
                var tmpPaths = paths.Distinct().ToArray();
                paths.Clear();
                tmpPaths.AsParallel().ForAll(path =>
                {
                    var neighbors = FilterNeighbours(path).ToList();
                    if (neighbors.Any())
                    {
                        foreach (var n in neighbors)
                        {
                            var np = path.Clone();
                            np.MoveTo(n);
                            paths.Enqueue(np);
                        }
                    }
                    else if (path.IsComplete(map))
                    {
                        finishedPaths.Add(path);
                    }
                });

                Console.Write($"\r{paths.Count} {paths.Average(x => x.Visited.Count)}");
            }

            return finishedPaths;
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
            public Queue<long> Input { get; } = new Queue<long>();

            public Memory(Memory other)
            {
                Pointer = other.Pointer;
                BaseValue = other.BaseValue;
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
                return $"{nameof(Pointer)}: {Pointer}, {nameof(BaseValue)}: {BaseValue}";
            }
        }
    }

    internal class Command
    {
        public char Turn { get; }
        public int Move { get; set; }

        public Command(char turn, int move = 1)
        {
            Turn = turn;
            Move = move;
        }

        public override string ToString()
        {
            return $"{Turn},{Move}";
        }

        public Command Clone()
        {
            return new Command(Turn, Move);
        }
    }

    internal class Path
    {
        public List<(int,int)> Visited { get; } = new List<(int, int)>();
        public List<Command> Commands { get; } = new List<Command>();

        public int Direction { get; set; } = 1;

        public (int, int) CurrentPosition => Visited.Last();

        (int, int) NextMove()
        {
            return Direction switch {
                1 => (CurrentPosition.Item1, CurrentPosition.Item2-1),
                2 => (CurrentPosition.Item1+1, CurrentPosition.Item2),
                3 => (CurrentPosition.Item1, CurrentPosition.Item2+1),
                4 => (CurrentPosition.Item1-1, CurrentPosition.Item2)
                };
        }

        private char Turn((int, int) newP)
        {
            var xd = CurrentPosition.Item1 - newP.Item1;
            var yd = CurrentPosition.Item2 - newP.Item2;
            switch (xd)
            {
                case 0 when yd == 1:
                    return Direction == 2 ? 'L' : 'R';
                case 0:
                    return Direction == 2 ? 'R' : 'L';
                case 1:
                    return Direction == 1 ? 'L' : 'R';
                default:
                    return Direction == 1 ? 'R' : 'L';
            }
        }

        void ChangeDir(char turn)
        {
            var r = turn == 'R' ? Direction : Direction - 2;
            Direction = ((r + 4) % 4) + 1;
        }

        public Path((int,int) startPos)
        {
            Visited.Add(startPos);
        }

        public Path()
        {
        }

        public Path Clone()
        {
            var p = new Path();
            foreach (var v in Visited)
                p.Visited.Add(v);
            foreach (var c in Commands)
                p.Commands.Add(c.Clone());

            p.Direction = Direction;

            return p;
        }

        public void MoveTo((int, int) pos)
        {
            if (Equals(pos, NextMove()))
                Commands.Last().Move++;
            else
            {
                var turn = Turn(pos);
                ChangeDir(turn);
                Commands.Add(new Command(turn));
            }
            Visited.Add(pos);
        }

        public override string ToString()
        {
            return !Commands.Any() ? "" : Commands.Select(x => x.ToString()).Aggregate((x, y) => $"{x},{y}");
        }

        public string EqualString => ToString();

        protected bool Equals(Path other)
        {
            return string.Equals(EqualString, other.EqualString, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Path) obj);
        }

        public override int GetHashCode()
        {
            return EqualString.GetHashCode();
        }

        public string Map => CreateMap();

        private string CreateMap()
        {
            var sb = new StringBuilder();
            for (var y = 0; y < 50; y++)
            {
                for (var x = 0; x < 100; x++)
                {
                    if (Visited.Contains((x, y)))
                        sb.Append(Visited.Count(c => Equals(c, (x,y))));
                    else
                        sb.Append('.');
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        public bool IsComplete(Dictionary<(int, int), char> map)
        {
            var points = map.Where(x => x.Value == '#').Select(x => x.Key).ToList();
            return points.All(x => Visited.Contains(x));
        }
    }
}