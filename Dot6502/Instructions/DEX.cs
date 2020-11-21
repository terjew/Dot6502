using Dot6502.AddressingModes;

namespace Dot6502.Instructions
{
    class DEX : Instruction
    {
        public override string Name => "DEX";
        public override string Description => "decrement X";
        public DEX() : base(new Implied()) { }

        public override ushort Execute(ExecutionState state)
        {
            var pointer = new XPointer(state);
            var operand = pointer.Get();
            var intResult = operand - 1;
            byte result = (byte)intResult;

            state.SetNegativeFlag(result);
            state.SetZeroFlag(result);
            pointer.Set(result);

            return InstructionSize;
        }
    }
}
