using AoCCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Challenges
{
    internal class AoC7 : AoCTool
    {
        public AoC7(int num) : base(num)
        {
            var i = 0;
            Input = GetInput().Split(',').Select(int.Parse).ToList();
        }

        public List<int> Input { get; }

        public override string CalculateSimple()
        {
            var res = new List<Tuple<string, int>>();
            for (var a = 0; a <= 4; a++)
                for (var b = 0; b <= 4; b++)
                    for (var c = 0; c <= 4; c++)
                        for (var d = 0; d <= 4; d++)
                            for (var e = 0; e <= 4; e++)
                            {
                                var phase = new[] { a, b, c, d, e };
                                if (phase.Distinct().Count() < 5)
                                    continue;
                                var o = 0;
                                foreach (var p in phase)
                                {
                                    o = Calc(Input.Select(x => x).ToList(), p, o);
                                }
                                res.Add(new Tuple<string, int>("" + a + b + c + d + e, o));
                            }

            return res.OrderBy(x => x.Item2).Last().Item2 + "";
        }

        public int Calc(List<int> state, params int[] input)
        {
            var p = 0;
            var output = 0;
            var ic = 0;
            while (state[p] != 99)
            {
                var jp = -1;
                var ins = state[p];
                var op = ins % 100;
                var args = (ins + "").PadLeft(5, '0').Reverse().Skip(2).ToArray();
                int P(int o) => args[o - 1] == '0' ? state[state[p + o]] : state[p + o];
                void S(int o, int v) => state[state[o]] = v;
                switch (op)
                {
                    case 1:
                        S(p + 3, P(1) + P(2));
                        break;
                    case 2:
                        S(p + 3, P(1) * P(2));
                        break;
                    case 3:
                        S(p + 1, input[ic++]);
                        break;
                    case 4:
                        output = P(1);
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
                    p += new[] { 1, 4, 4, 2, 2, 3, 3, 4, 4 }[op];
            }

            return output;
        }

        public override string CalculateExtended()
        {
            var res = new List<Tuple<string, int>>();
            for (var a = 5; a <= 9; a++)
                for (var b = 5; b <= 9; b++)
                    for (var c = 5; c <= 9; c++)
                        for (var d = 5; d <= 9; d++)
                            for (var e = 5; e <= 9; e++)
                            {
                                var phase = new[] { a, b, c, d, e };
                                if (phase.Distinct().Count() < 5)
                                    continue;
                                var amps = new[] { 
                                    new Amp("A", Input, a),
                                    new Amp("B", Input, b),
                                    new Amp("C", Input, c),
                                    new Amp("D", Input, d),
                                    new Amp("E", Input, e) };
                                amps[0].Input.Enqueue(0);
                                var enumerators = amps.Select(x => x.Calc().GetEnumerator()).ToList();

                                var s = 0;
                                while (!amps[4].Finished)
                                {
                                    enumerators[s].MoveNext();
                                    amps[(s+1)%5].Input.Enqueue(enumerators[s].Current);
                                    s = (s+1)%5;
                                }
                                res.Add(new Tuple<string, int>("" + a + b + c + d + e, amps[4].LastOutput));
                            }

            return res.OrderBy(x => x.Item2).Last().Item2 + "";
        }
        
        public class Amp
        {
            public Queue<int> Input { get; } = new Queue<int>();
            public int LastOutput { get; private set; }
            public int Pointer { get; private set; } = 0;
            public List<int> State { get; }
            public string Name { get; }
            public bool Finished { get; set; } = false;

            public Amp(string name, List<int> state, int amp)
            {
                State = state.Select(x => x).ToList();
                Input.Enqueue(amp);
                Name = name;
            }

            private int GetInput()
            {
                return Input.Dequeue();
            }
            
            public IEnumerable<int> Calc()
            {
                while (State[Pointer] != 99)
                {
                    var jp = -1;
                    var ins = State[Pointer];
                    var op = ins % 100;
                    var args = (ins + "").PadLeft(5, '0').Reverse().Skip(2).ToArray();
                    int P(int o) => args[o - 1] == '0' ? State[State[Pointer + o]] : State[Pointer + o];
                    void S(int o, int v) => State[State[o]] = v;
                    switch (op)
                    {
                        case 1:
                            S(Pointer + 3, P(1) + P(2));
                            break;
                        case 2:
                            S(Pointer + 3, P(1) * P(2));
                            break;
                        case 3:
                            S(Pointer + 1, GetInput());
                            break;
                        case 4:
                            LastOutput = P(1);
                            yield return LastOutput;
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
                            S(Pointer + 3, P(1) < P(2) ? 1 : 0);
                            break;
                        case 8:
                            S(Pointer + 3, P(1) == P(2) ? 1 : 0);
                            break;
                    }

                    if (jp >= 0)
                        Pointer = jp;
                    else
                        Pointer += new[] { 1, 4, 4, 2, 2, 3, 3, 4, 4 }[op];
                }

                Finished = true;
            }
        }

        
    }
}