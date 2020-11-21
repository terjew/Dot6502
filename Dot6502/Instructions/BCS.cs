namespace Dot6502.Instructions
{
    class BCS : Instruction
    {
        public override string Name => "BCS";
        public override string Description => "branch on carry set";
        public BCS(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            if (state.TestFlag(StateFlag.Carry))
            {
                sbyte offset = unchecked((sbyte)AddressingMode.Resolve(state).Get());
                state.PC = (ushort)(state.PC + offset + InstructionSize);
                return 0;
            }
            return InstructionSize;
        }
    }
}
