using System;
using System.Collections.Generic;
using System.Text;

namespace 控制台游戏
{
    class RandomHelper
    {
        static Random rnd = new Random();

        public static int GetRandom(int min, int max)
        {
            if (min < 0 || min > max)
                return min;

            return rnd.Next(min, max);
        }
    }
}
