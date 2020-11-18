namespace Dot6502.AddressingModes
{
    class Immediate : AddressingMode
    {
        public override string Name => "Immediate";
        public override string ShortName => "#";
        public override int OperandLength => 1;

        public override byte GetOperand(ExecutionState state)
        {
            var operand = state.ReadByte((ushort)(state.PC + 1));
            return operand;
        }
    }
}
