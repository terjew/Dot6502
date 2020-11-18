namespace Dot6502.AddressingModes
{
    class Accumulator : AddressingMode
    {
        public override string Name => "Accumulator";
        public override string ShortName => "A";
        public override int OperandLength => 0;

        public override byte GetOperand(ExecutionState state)
        {
            return state.AC;
        }
    }
}
