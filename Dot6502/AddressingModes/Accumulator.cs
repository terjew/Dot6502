using System;

namespace Dot6502.AddressingModes
{
    class Accumulator : AddressingMode
    {
        public override string Name => "Accumulator";
        public override string ShortName => "A";
        public override ushort OperandLength => 0;
        public override Pointer Resolve(ExecutionState state)
        {
            return new ACPointer(state);
        }

        public override string Disassemble(byte[] mem, int pc)
        {
            return "A";
        }
    }
}
