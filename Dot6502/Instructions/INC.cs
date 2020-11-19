namespace Dot6502.Instructions
{
    class INC : Instruction
    {
        public override string Name => "INC";
        public override string Description => "increment memory";
        public INC(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var pointer = AddressingMode.Resolve(state);
            var operand = pointer.Get();
            var intResult = operand + 1;
            byte result = (byte)intResult;

            state.SetNegativeFlag(result);
            state.SetZeroFlag(result);
            pointer.Set(result);

            return InstructionSize;
        }
    }
}
