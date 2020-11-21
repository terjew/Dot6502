namespace Dot6502.Instructions
{
    class ASL : Instruction
    {
        public override string Name => "ASL";
        public override string Description => "arithmetic shift left";
        public ASL(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var pointer = AddressingMode.Resolve(state);
            var operand = pointer.Get();
            var intResult = operand << 1;

            byte result = (byte)intResult;

            state.SetNegativeFlag(result);
            state.SetZeroFlag(result);
            state.SetCarryFlag(intResult > 255);

            pointer.Set(result);

            return InstructionSize;
        }
    }
}
