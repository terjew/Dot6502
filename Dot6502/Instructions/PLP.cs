using Dot6502.AddressingModes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dot6502.Instructions
{
    class PLP : Instruction
    {
        public PLP() : base(new Implied()) { }

        public override string Name => "PLP";
        public override string Description => "pull processor status from stack";

        public override ushort Execute(ExecutionState state)
        {
            state.SR = state.PopByte();
            return InstructionSize;
        }
    }
}
