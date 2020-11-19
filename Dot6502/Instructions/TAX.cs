using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class TAX : Instruction
    {
        public override string Name => "TAX";
        public override string Description => "transfer accumulator to X";
        public TAX() : base(new Implied()) { }

        public override ushort Execute(ExecutionState state)
        {
            state.X = state.AC;
            state.SetNegativeFlag(state.X);
            state.SetZeroFlag(state.X);
            return InstructionSize;
        }
    }
}
