using System;
using System.Collections.Generic;
using System.Linq;
using AoCCore;

namespace Challenges
{
    public class AoC5 : AoCTool
    {
        public AoC5(int num) : base(num)
        {
        }
        
        private int G(Dictionary<int, int> d, char c, int x) => c switch
        {
            '0' => d[x],
            '1' => x
        };

        public override string CalculateSimple()
        {
            var i = 0;
            var p = 0;
            var d = GetInput().Split(',').ToDictionary(x => i++, int.Parse);
            var input = 1;
            var output = string.Empty;

            while (d[p] != 99)
            {
                var ins = d[p];
                var op = ins%100;
                var par = (ins + "").PadLeft(5, '0').Reverse().Skip(2).ToArray();
                switch(op) 
                {
                    case 1: 
                        d[d[p + 3]] = G(d, par[0],d[p+1]) + G(d, par[1],d[p+2]);
                        p += 4;
                        break;
                    case 2:
                        d[d[p + 3]] = G(d, par[0],d[p+1]) * G(d, par[1],d[p+2]);
                        p += 4;
                        break;
                    case 3:
                        d[d[p + 1]] = input;
                        p += 2;
                        break;
                    case 4:
                        output = d[d[p + 1]]+"";
                        p += 2;
                        break;
                };
            }

            return output;
        }
        
        public override string CalculateExtended()
        {
            var i = 0;
            var p = 0;
            var d = GetInput().Split(',').ToDictionary(x => i++, int.Parse);
            var input = 5;
            var output = string.Empty;
            while (d[p] != 99)
            {
                var ins = new Instruction(d, p);
                p = ins.NextPosition;
                switch(ins.OpCode) 
                {
                    case 1:
                        d[ins.TargetPosition] = ins.Params[0].Value + ins.Params[1].Value;
                        break;
                    case 2:
                        d[ins.TargetPosition] = ins.Params[0].Value * ins.Params[1].Value;
                        break;
                    case 3:
                        d[ins.TargetPosition] = input;
                        break;
                    case 4:
                        output = d[ins.TargetPosition]+"";
                        Console.WriteLine(output);
                        break;
                    case 5:
                        if (ins.Params[0].Value != 0)
                            p = ins.Params[1].Value;
                        break;
                    case 6:
                        if (ins.Params[0].Value == 0)
                            p = ins.Params[1].Value;
                        break;
                    case 7:
                        d[ins.TargetPosition] = ins.Params[0].Value < ins.Params[1].Value ? 1 : 0;
                        break;
                    case 8:
                        d[ins.TargetPosition] = ins.Params[0].Value == ins.Params[1].Value ? 1 : 0;
                        break;
                };
            }

            return output;
        }
    }

    public class Instruction
    {
        private Dictionary<int, int> CommandLength { get; } = new Dictionary<int, int>()
        {
            {1, 3},
            {2, 3},
            {3, 1},
            {4, 1},
            {5, 2},
            {6, 2},
            {7, 3},
            {8, 3},
        };

        public int OpCode { get; }
        public List<Param> Params { get; } = new List<Param>();
        private Param TargetParameter => Params.LastOrDefault();
        public int TargetPosition => TargetParameter.ParamValue;
        public int Pos { get; }
        public int NextPosition => TargetParameter.Value == Pos ? Pos : TargetParameter.Position + 1;
        public int FullInstruction { get; }

        public Instruction(Dictionary<int, int> d, int p)
        {
            Pos = p;
            FullInstruction = d[p];
            OpCode = FullInstruction % 100;
            var par = (FullInstruction + "").PadLeft(5, '0').Reverse().Skip(2).ToArray();
            for (var px = p + 1; px <= p + CommandLength[OpCode]; px++)
                Params.Add(new Param(d, px, par[px - p - 1]));
        }

        public override string ToString()
        {
            return Pos + ": " + FullInstruction + "," + Params.Select(x => x.ParamValue.ToString()).Aggregate((x, y) => x + "," + y);
        }
    }

    public class Param
    {
        public char Mode { get; }
        public int Value { get; }
        public int ParamValue { get; }
        public int Position { get; }

        public Param(Dictionary<int, int> d, int position, char m)
        {
            Position = position;
            ParamValue = d[position];
            Mode = m;
            Value = m == '0' ? d[ParamValue] : ParamValue;
        }

        public override string ToString()
        {
            return $"{nameof(Position)}: {Position}, {nameof(ParamValue)}: {ParamValue}, {nameof(Mode)}: {Mode}, {nameof(Value)}: {Value}";
        }
    }
}