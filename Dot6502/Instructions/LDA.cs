namespace Dot6502.Instructions
{
    class LDA : Instruction
    {
        public override string Name => "LDA";
        public override string Description => "load accumulator";
        public LDA(AddressingMode mode) : base(mode) { }

        public override void Execute(ExecutionState state)
        {
            var operand = AddressingMode.GetOperand(state);
            state.AC = operand;
        }
    }
}
