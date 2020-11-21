namespace Dot6502.Instructions
{
    class BVS : Instruction
    {
        public override string Name => "BVS";
        public override string Description => "branch on overflow set";
        public BVS(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            if (state.TestFlag(StateFlag.Overflow))
            {
                sbyte offset = unchecked((sbyte)AddressingMode.Resolve(state).Get());
                state.PC = (ushort)(state.PC + offset + InstructionSize);
                return 0;
            }
            return InstructionSize;
        }
    }
}
