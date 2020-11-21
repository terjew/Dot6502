using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class TSX : Instruction
    {
        public override string Name => "TSX";
        public override string Description => "transfer stack register to X";
        public TSX() : base(new Implied()) { }

        public override ushort Execute(ExecutionState state)
        {
            state.X = state.SP;
            state.SetNegativeFlag(state.X);
            state.SetZeroFlag(state.X);
            return InstructionSize;
        }
    }
}
