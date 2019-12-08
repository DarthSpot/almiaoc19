using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using AoCCore;

namespace Challenges
{
    public class AoC8 : AoCTool
    {
        public AoC8(int num) : base(num)
        {

        }

        public override string CalculateSimple()
        {
            var layers = Regex.Matches(GetInput(), "\\d{150}").Select(x => x.Value).ToList();
            var fz = layers.OrderBy(x => x.Count(c => c == '0')).First();
            return ""+(fz.Count(c => c == '1') * fz.Count(c => c == '2'));
        }

        public override string CalculateExtended()
        {
            var layers = Regex.Matches(GetInput(), "\\d{150}").Select(x => x.Value).ToList();
            var b = new Bitmap(25, 6);
            for (var y = 0; y < b.Height; ++y)
            for (var x = 0; x < b.Width; ++x)
            {
                b.SetPixel(x,y, layers.Select(l => l[y * b.Width + x]).First(p => p != '2') == '0' ? Color.Black : Color.White);
            }

            b.Save("x.bmp");
            return "";
        }
    }

}