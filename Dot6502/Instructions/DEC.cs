namespace Dot6502.Instructions
{
    class DEC : Instruction
    {
        public override string Name => "DEC";
        public override string Description => "decrement memory";
        public DEC(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var pointer = AddressingMode.Resolve(state);
            var operand = pointer.Get();
            var intResult = operand - 1;
            byte result = (byte)intResult;

            state.SetNegativeFlag(result);
            state.SetZeroFlag(result);
            pointer.Set(result);

            return InstructionSize;
        }
    }
}
