using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class NOP : Instruction
    {
        public override string Name => "NOP";
        public override string Description => "no operation";
        public NOP() : base(new Implied()) { }

        public override ushort Execute(ExecutionState state)
        {
            return InstructionSize;
        }
    }
}
