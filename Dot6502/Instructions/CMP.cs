namespace Dot6502.Instructions
{
    class CMP : Instruction
    {
        public override string Name => "CMP";
        public override string Description => "compare (with accumulator)";
        public CMP(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var operand = AddressingMode.Resolve(state).Get();

            int result = state.AC - operand;
            byte byteResult = (byte)result;

            state.SetNegativeFlag(byteResult);
            state.SetZeroFlag(byteResult);

            if (result > 255 || result < -255) state.SetFlag(StateFlag.Carry);
            else state.ClearFlag(StateFlag.Carry);

            return InstructionSize;
        }
    }
}
