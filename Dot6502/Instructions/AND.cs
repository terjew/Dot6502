using System;
using System.Collections.Generic;
using System.Text;

namespace Dot6502.Instructions
{
    class AND : Instruction
    {
        public override string Name => "AND";
        public override string Description => "and (with accumulator)";
        public AND(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var operand = AddressingMode.Resolve(state).Get();
            var result = (byte)(state.AC & operand);

            state.SetNegativeFlag(result);
            state.SetZeroFlag(result);

            state.AC = result;
            return InstructionSize;
        }
    }
}
