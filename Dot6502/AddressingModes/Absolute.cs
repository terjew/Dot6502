using System;

namespace Dot6502.AddressingModes
{
    class Absolute : AddressingMode
    {
        public override string Name => "Absolute";
        public override string ShortName => "abs";
        public override int OperandLength => 2;
        public IndexMode IndexMode { get; }

        public Absolute(IndexMode indexMode)
        {
            IndexMode = indexMode;
        }

        public override byte GetOperand(ExecutionState state)
        {
            var baseAddress = state.ReadWord((ushort)(state.PC + 1));
            switch (IndexMode)  
            {
                case IndexMode.None:
                    return state.ReadByte(baseAddress);
                case IndexMode.X:
                    return state.ReadByte((ushort)(baseAddress + state.X));
                case IndexMode.Y:
                    return state.ReadByte((ushort)(baseAddress + state.Y));
            }
            throw new NotSupportedException();
        }
    }
}
