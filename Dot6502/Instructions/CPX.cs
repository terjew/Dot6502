namespace Dot6502.Instructions
{
    class CPX : Instruction
    {
        public override string Name => "CPX";
        public override string Description => "compare (with X)";
        public CPX(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var operand = AddressingMode.Resolve(state).Get();

            int result = state.X - operand;
            byte byteResult = (byte)result;

            state.SetNegativeFlag(byteResult);
            state.SetZeroFlag(byteResult);

            if (result > 255 || result < -255) state.SetFlag(StateFlag.Carry);
            else state.ClearFlag(StateFlag.Carry);

            return InstructionSize;
        }
    }
}
