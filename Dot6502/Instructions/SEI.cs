using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class SEI : Instruction
    {
        public override string Name => "SEI";
        public override string Description => "set interrupt disable status";
        public SEI() : base(new Implied()) { }

        public override ushort Execute(ExecutionState state)
        {
            state.SetFlag(StateFlag.Interrupt);
            return InstructionSize;
        }
    }
}
