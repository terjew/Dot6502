using System;
using System.Collections.Generic;
using System.Text;

namespace Dot6502.AddressingModes
{
    class Relative : AddressingMode
    {
        public override string Name => "Relative";
        public override string ShortName => "rel";
        public override ushort OperandLength => 1;

        public override Pointer Resolve(ExecutionState state)
        {
            return new ImmediateValuePointer(state);
        }
    }
}
