namespace Dot6502.Instructions
{
    class BVC : Instruction
    {
        public override string Name => "BVC";
        public override string Description => "branch on overflow clear";
        public BVC(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            if (!state.TestFlag(StateFlag.Overflow))
            {
                sbyte offset = unchecked((sbyte)AddressingMode.Resolve(state).Get());
                state.PC = (ushort)(state.PC + offset + InstructionSize);
                return 0;
            }
            return InstructionSize;
        }
    }
}
