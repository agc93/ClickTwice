using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickTwice.Publisher.Core
{
    internal static class CoreExtensions
    {
        internal static void Add<T>(this List<T> list, params T[] items)
        {
            list.AddRange(items);
        }
    }
}
