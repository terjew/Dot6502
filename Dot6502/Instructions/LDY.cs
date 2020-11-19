namespace Dot6502.Instructions
{
    class LDY : Instruction
    {
        public override string Name => "LDY";
        public override string Description => "load Y";
        public LDY(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var pointer = AddressingMode.Resolve(state);
            state.Y = pointer.Get();
            state.SetZeroFlag(state.Y);
            state.SetNegativeFlag(state.Y);
            return InstructionSize;
        }
    }
}
