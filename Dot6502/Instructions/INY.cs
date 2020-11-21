using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class INY : Instruction
    {
        public override string Name => "INY";
        public override string Description => "increment Y";
        public INY() : base(new Implied()) { }

        public override ushort Execute(ExecutionState state)
        {
            var pointer = new YPointer(state);
            var operand = pointer.Get();
            var intResult = operand + 1;
            byte result = (byte)intResult;

            state.SetNegativeFlag(result);
            state.SetZeroFlag(result);
            pointer.Set(result);

            return InstructionSize;
        }
    }
}
