using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class CLC : Instruction
    {
        public override string Name => "CLC";
        public override string Description => "clear carry";
        public CLC() : base(new Implied()) { }

        public override void Execute(ExecutionState cpu)
        {
            cpu.ClearFlag(StateFlag.Carry);
        }
    }
}
