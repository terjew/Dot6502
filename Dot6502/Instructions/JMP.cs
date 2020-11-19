using Dot6502.AddressingModes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dot6502.Instructions
{
    class JMP : Instruction
    {
        public JMP(AddressingMode mode) : base(mode) { }

        public override string Name => "JMP";
        public override string Description => "jump";

        public override ushort Execute(ExecutionState state)
        {
            var jmpTarget = AddressingMode.Resolve(state) as MemoryPointer;
            state.PC = jmpTarget.Address;
            return 0;
        }
    }
}
