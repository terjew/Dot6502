using Dot6502.AddressingModes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Dot6502Tests")]
namespace Dot6502
{
    public enum IndexMode
    {
        None,
        X,
        Y
    }

    public abstract class AddressingMode
    {
        public abstract string Name { get; }
        public abstract string ShortName { get; }
        public abstract ushort OperandLength { get; }

        public virtual Pointer Resolve(ExecutionState state)
        {
            return new InvalidPointer();
        }

        public abstract string Disassemble(byte[] mem, int pc);
    }

}
