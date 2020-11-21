using Dot6502.AddressingModes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dot6502.Instructions
{
    class PHA : Instruction
    {
        public PHA() : base(new Implied()) { }

        public override string Name => "PHA";
        public override string Description => "push accumulator on stack";

        public override ushort Execute(ExecutionState state)
        {
            state.Push(state.AC);
            return InstructionSize;
        }
    }
}
