using Dot6502.AddressingModes;
using System;
using System.Collections.Generic;
using System.Text;

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
    }

}
