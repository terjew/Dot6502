using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class SED : Instruction
    {
        public override string Name => "SED";
        public override string Description => "set decimal mode";
        public SED() : base(new Implied()) { }

        public override ushort Execute(ExecutionState state)
        {
            state.SetFlag(StateFlag.Decimal);
            return InstructionSize;
        }
    }
}
