using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class CLI : Instruction
    {
        public override string Name => "CLI";
        public override string Description => "clear interupt disable";
        public CLI() : base(new Implied()) { }

        public override ushort Execute(ExecutionState cpu)
        {
            cpu.ClearFlag(StateFlag.Interrupt);
            return InstructionSize;
        }
    }
}
