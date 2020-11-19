using Dot6502.AddressingModes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dot6502.Instructions
{
    class JSR : Instruction
    {
        public JSR(AddressingMode mode) : base(mode) { }

        public override string Name => "JSR";
        public override string Description => "jump subroutine";

        public override ushort Execute(ExecutionState state)
        {
            var jmpTarget = AddressingMode.Resolve(state) as MemoryPointer;
            state.Push((ushort)(state.PC + InstructionSize));
            state.PC = jmpTarget.Address;
            return 0;
        }
    }
}
