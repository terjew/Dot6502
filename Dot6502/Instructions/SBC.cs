namespace Dot6502.Instructions
{
    class SBC : Instruction
    {
        public override string Name => "SBC";
        public override string Description => "subtract with carry";
        public SBC(AddressingMode mode) : base(mode) { }

        public override ushort Execute(ExecutionState state)
        {
            var carryIn = state.TestFlag(StateFlag.Carry) ? 1 : 0;
            var operand = AddressingMode.Resolve(state).Get();
            var unsignedResult = state.AC + (~operand) + carryIn;
            byte byteResult = (byte)unsignedResult;

            state.SetCarryFlag(unsignedResult > 255);
            state.SetZeroFlag(byteResult == 0);
            state.SetNegativeFlag(byteResult);
            state.SetOverflowFlag(state.AC, operand, byteResult);

            state.AC = byteResult;
            return InstructionSize;
        }
    }
}
