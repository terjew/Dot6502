using Dot6502.AddressingModes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dot6502.Instructions
{
    class PHP : Instruction
    {
        public PHP() : base(new Implied()) { }

        public override string Name => "PHP";
        public override string Description => "push processor status on stack";

        public override ushort Execute(ExecutionState state)
        {
            state.Push(state.SR);
            return InstructionSize;
        }
    }
}
