namespace Dot6502.Instructions
{
    class STA : Instruction
    {
        public override string Name => "STA";
        public override string Description => "store accumulator";
        public STA(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var pointer = AddressingMode.Resolve(state);
            pointer.Set(state.AC);
            return InstructionSize;
        }
    }
}
