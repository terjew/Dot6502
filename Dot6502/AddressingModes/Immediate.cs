using System;

namespace Dot6502.AddressingModes
{
    class Immediate : AddressingMode
    {
        public override string Name => "Immediate";
        public override string ShortName => "#";
        public override ushort OperandLength => 1;
        public override Pointer Resolve(ExecutionState state)
        {
            return new ImmediateValuePointer(state);
        }
    }
}
