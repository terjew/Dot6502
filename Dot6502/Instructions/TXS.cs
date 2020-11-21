using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class TXS : Instruction
    {
        public override string Name => "TXS";
        public override string Description => "transfer X to stack register";
        public TXS() : base(new Implied()) { }

        public override ushort Execute(ExecutionState state)
        {
            state.SR = state.X;
            return InstructionSize;
        }
    }
}
