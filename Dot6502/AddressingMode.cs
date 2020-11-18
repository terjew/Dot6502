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
        public abstract int OperandLength { get; }

        public abstract byte GetOperand(ExecutionState state);
    }

}
