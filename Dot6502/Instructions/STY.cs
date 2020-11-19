namespace Dot6502.Instructions
{
    class STY : Instruction
    {
        public override string Name => "STY";
        public override string Description => "store Y";
        public STY(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var pointer = AddressingMode.Resolve(state);
            pointer.Set(state.Y);
            return InstructionSize;
        }
    }
}
