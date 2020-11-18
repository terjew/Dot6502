using System;

namespace Dot6502.AddressingModes
{
    class Implied : AddressingMode
    {
        public override string Name => "Implied";
        public override string ShortName => "impl";
        public override int OperandLength => 0;

        public override byte GetOperand(ExecutionState state)
        {
            throw new NotSupportedException();
        }
    }
}
