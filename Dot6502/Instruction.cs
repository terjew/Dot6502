using System;

namespace Dot6502
{
    public abstract class Instruction
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public AddressingMode AddressingMode { get; }
        public ushort InstructionSize => (ushort)(AddressingMode.OperandLength + 1);

        protected Instruction(AddressingMode mode)
        {
            AddressingMode = mode;
        }

        public abstract ushort Execute(ExecutionState state);
    }
}
