using System;
using System.Collections.Generic;
using System.Text;

namespace Dot6502.AddressingModes
{
    class Relative : AddressingMode
    {
        public override string Name => "Relative";
        public override string ShortName => "rel";
        public override int OperandLength => 1;

        public override byte GetOperand(ExecutionState state)
        {
            var operand = state.ReadByte((ushort)(state.PC + 1));
            return operand;
        }
    }
}
