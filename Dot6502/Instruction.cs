using System;

namespace Dot6502
{
    public abstract class Instruction
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public AddressingMode AddressingMode { get; }
        public int InstructionSize => AddressingMode.OperandLength + 1;

        protected Instruction(AddressingMode mode)
        {
            AddressingMode = mode;
        }

        public abstract void Execute(ExecutionState cpu);
    }
}
