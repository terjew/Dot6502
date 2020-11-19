using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class TXA : Instruction
    {
        public override string Name => "TXA";
        public override string Description => "transfer X to accumulator";
        public TXA() : base(new Implied()) { }

        public override ushort Execute(ExecutionState state)
        {
            state.AC = state.X;
            state.SetNegativeFlag(state.AC);
            state.SetZeroFlag(state.AC);
            return InstructionSize;
        }
    }
}
