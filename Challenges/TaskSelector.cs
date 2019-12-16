using System.Collections.Generic;
using AoCCore;

namespace Challenges
{
    public class TaskSelector
    {
        private AoCTool[] _tools { get; set; }

        public TaskSelector()
        {
            _tools = new AoCTool[]
            {
                new AoC1(1),
                new AoC2(2),
                new AoC3(3),
                new AoC4(4),
                new AoC5(5),
                new AoC6(6),
                new AoC7(7),
                new AoC8(8),
                new AoC9(9),
                new AoC10(10),
                new AoC11(11),
                new AoC12(12),
                new AoC13(13),
            };
        }

        public AoCTool this[int task] => _tools.Length < task  || task < 0 ? null : _tools[task-1];
    }
}
