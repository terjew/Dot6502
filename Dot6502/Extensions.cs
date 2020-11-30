using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot6502
{
    public static class Extensions
    {
        public static ushort ReadWord(this byte[] arr, int pos)
        {
            return (ushort)((arr[pos + 1] << 8) + arr[pos]);
        }
    }
}
