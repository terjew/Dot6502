using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class SEC : Instruction
    {
        public override string Name => "SEC";
        public override string Description => "set carry";
        public SEC() : base(new Implied()) { }

        public override void Execute(ExecutionState state)
        {
            state.SetFlag(StateFlag.Carry);
        }
    }
}
