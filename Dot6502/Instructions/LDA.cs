namespace Dot6502.Instructions
{
    class LDA : Instruction
    {
        public override string Name => "LDA";
        public override string Description => "load accumulator";
        public LDA(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var pointer = AddressingMode.Resolve(state);
            state.AC = pointer.Get();
            state.SetZeroFlag(state.AC);
            state.SetNegativeFlag(state.AC);
            return InstructionSize;
        }
    }
}
