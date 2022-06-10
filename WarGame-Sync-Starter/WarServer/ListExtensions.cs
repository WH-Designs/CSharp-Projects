using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarServer
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rand = new Random();

            for (int i = list.Count - 1; i > 0; --i)
            {
                int index = rand.Next(i + 1);
                (list[i], list[index]) = (list[index], list[i]);
            }
        }
    }
}
