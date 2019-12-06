using System.Collections.Generic;
using AoCCore;

namespace Challenges
{
    public class TaskSelector
    {
        private readonly Dictionary<int, AoCTool> _tools = new Dictionary<int, AoCTool>();

        public TaskSelector()
        {
            _tools.Add(1, new AoC1(1)); 
            _tools.Add(2, new AoC2(2));
            _tools.Add(3, new AoC3(3));
            _tools.Add(4, new AoC4(4));
            _tools.Add(5, new AoC5(5));
            _tools.Add(6, new AoC6(6));
        }

        public AoCTool this[int task] => _tools.ContainsKey(task) ? _tools[task] : null;
    }
}
