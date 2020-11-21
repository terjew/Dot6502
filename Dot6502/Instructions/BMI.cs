namespace Dot6502.Instructions
{
    class BMI : Instruction
    {
        public override string Name => "BMI";
        public override string Description => "branch on minus (negative set)";
        public BMI(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            if (state.TestFlag(StateFlag.Negative))
            {
                sbyte offset = unchecked((sbyte)AddressingMode.Resolve(state).Get());
                state.PC = (ushort)(state.PC + offset + InstructionSize);
                return 0;
            }
            return InstructionSize;
        }
    }
}
