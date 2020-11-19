namespace Dot6502.Instructions
{
    class LDX : Instruction
    {
        public override string Name => "LDX";
        public override string Description => "load X";
        public LDX(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var pointer = AddressingMode.Resolve(state);
            state.X = pointer.Get();
            state.SetZeroFlag(state.X);
            state.SetNegativeFlag(state.X);
            return InstructionSize;
        }
    }
}
