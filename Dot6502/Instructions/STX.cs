namespace Dot6502.Instructions
{
    class STX : Instruction
    {
        public override string Name => "STX";
        public override string Description => "store X";
        public STX(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var pointer = AddressingMode.Resolve(state);
            pointer.Set(state.X);
            return InstructionSize;
        }
    }
}
