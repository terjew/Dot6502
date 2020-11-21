namespace Dot6502.Instructions
{
    class LSR : Instruction
    {
        public override string Name => "LSR";
        public override string Description => "logical shift right";
        public LSR(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var pointer = AddressingMode.Resolve(state);
            var operand = pointer.Get();
            var intResult = operand >> 1;

            byte result = (byte)intResult;

            state.SetZeroFlag(result);
            state.SetCarryFlag((operand & 1) == 1);

            pointer.Set(result);

            return InstructionSize;
        }
    }
}
