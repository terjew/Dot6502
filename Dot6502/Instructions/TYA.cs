using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class TYA : Instruction
    {
        public override string Name => "TYA";
        public override string Description => "transfer Y to accumulator";
        public TYA() : base(new Implied()) { }

        public override ushort Execute(ExecutionState state)
        {
            state.AC = state.Y;
            state.SetNegativeFlag(state.AC);
            state.SetZeroFlag(state.AC);
            return InstructionSize;
        }
    }
}
