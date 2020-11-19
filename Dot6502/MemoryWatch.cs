using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot6502
{
    public class MemoryWatch
    {
        public MemoryWatch(ushort start, ushort end, Action<ushort, byte> callback)
        {
            this.Start = start;
            this.End = end;
            this.Callback = callback;
        }

        public ushort Start { get; }
        public ushort End { get; }
        public Action<ushort, byte> Callback { get; }

        public bool IsInside(ushort address)
        {
            return address >= Start && address <= End;
        }
    }
}
