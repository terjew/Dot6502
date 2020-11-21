using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class CLV : Instruction
    {
        public override string Name => "CLV";
        public override string Description => "clear overflow";
        public CLV() : base(new Implied()) { }

        public override ushort Execute(ExecutionState cpu)
        {
            cpu.ClearFlag(StateFlag.Overflow);
            return InstructionSize;
        }
    }
}
