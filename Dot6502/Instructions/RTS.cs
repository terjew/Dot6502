using Dot6502.AddressingModes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dot6502.Instructions
{
    class RTS : Instruction
    {
        public RTS() : base(new Implied()) { }

        public override string Name => "RTS";
        public override string Description => "return from subroutine";

        public override ushort Execute(ExecutionState state)
        {
            var returnTarget = state.PopWord();
            state.PC = returnTarget;
            return 0;
        }
    }
}
