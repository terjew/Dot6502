namespace Dot6502.Instructions
{
    class ADC : Instruction
    {
        public override string Name => "ADC";
        public override string Description => "add with carry";
        public ADC(AddressingMode mode) : base(mode) { }

        public override void Execute(ExecutionState state)
        {
            var carryIn = state.TestFlag(StateFlag.Carry) ? 1 : 0;
            var operand = AddressingMode.GetOperand(state);
            var result = state.AC + operand + carryIn;
            bool carry = result > 255;
            if (carry) state.SetFlag(StateFlag.Carry);
            else state.ClearFlag(StateFlag.Carry);
            //FIXME: handle overflow
            state.AC = (byte)result;
        }
    }
}
