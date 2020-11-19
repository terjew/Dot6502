using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class TAY : Instruction
    {
        public override string Name => "TAY";
        public override string Description => "transfer accumulator to Y";
        public TAY() : base(new Implied()) { }

        public override ushort Execute(ExecutionState state)
        {
            state.Y = state.AC;
            state.SetNegativeFlag(state.Y);
            state.SetZeroFlag(state.Y);
            return InstructionSize;
        }
    }
}
