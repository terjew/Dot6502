namespace Dot6502.Instructions
{
    class BPL : Instruction
    {
        public override string Name => "BPL";
        public override string Description => "branch on positive (negative clear)";
        public BPL(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            if (!state.TestFlag(StateFlag.Negative))
            {
                sbyte offset = unchecked((sbyte)AddressingMode.Resolve(state).Get());
                state.PC = (ushort)(state.PC + offset + InstructionSize);
                return 0;
            }
            return InstructionSize;
        }
    }
}
