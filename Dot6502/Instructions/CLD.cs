using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class CLD : Instruction
    {
        public override string Name => "CLD";
        public override string Description => "clear decimal mode";
        public CLD() : base(new Implied()) { }

        public override ushort Execute(ExecutionState cpu)
        {
            cpu.ClearFlag(StateFlag.Decimal);
            return InstructionSize;
        }
    }
}
