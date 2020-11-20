namespace Dot6502.Instructions
{
    class CMP : Instruction
    {
        public override string Name => "CMP";
        public override string Description => "compare (with accumulator)";
        public CMP(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var operand = AddressingMode.Resolve(state).Get();
            state.SetNegativeFlag(state.AC < operand);
            state.SetZeroFlag(state.AC == operand);
            state.SetCarryFlag(state.AC >= operand);
            return InstructionSize;
        }
    }

}
