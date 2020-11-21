using Dot6502.AddressingModes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dot6502.Instructions
{
    class PLA : Instruction
    {
        public PLA() : base(new Implied()) { }

        public override string Name => "PLA";
        public override string Description => "pull accumulator from stack";

        public override ushort Execute(ExecutionState state)
        {
            state.AC = state.PopByte();
            state.SetNegativeFlag(state.AC);
            state.SetZeroFlag(state.AC);
            return InstructionSize;
        }
    }
}
