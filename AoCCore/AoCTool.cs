using System;
using System.Collections.Generic;
using System.Text;

namespace AoCCore
{
    public abstract class AoCTool
    {
        protected AoCTool(int num)
        {
            Num = num;
        }

        public int Num { get; }

        public virtual string CalculateSimple()
        {
            return "";
        }

        public virtual string CalculateExtended()
        {
            return "";
        }

        protected string[] GetInputArr()
        {
            return AoCHelper.ReadFileArr(Num);
        }

        protected string GetInput()
        {
            return AoCHelper.ReadFile(Num);
        }

        protected virtual string[] GetTestInputArr()
        {
            return null;
        }
    }
}
