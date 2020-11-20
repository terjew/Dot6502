namespace Dot6502.Instructions
{
    class BNE : Instruction
    {
        public override string Name => "BNE";
        public override string Description => "branch on not equal (zero clear)";
        public BNE(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            if (!state.TestFlag(StateFlag.Zero))
            {
                int offset = AddressingMode.Resolve(state).Get();
                if (offset > 127)
                {
                    offset = offset - 256;
                }
                state.PC = (ushort)(state.PC + offset + InstructionSize);
                return 0;
            }
            return InstructionSize;
        }
    }
}
