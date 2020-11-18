using System;

namespace Dot6502.AddressingModes
{
    class Absolute : AddressingMode
    {
        public override string Name => "Absolute";
        public override string ShortName => "abs";
        public override ushort OperandLength => 2;
        public IndexMode IndexMode { get; }

        public Absolute(IndexMode indexMode)
        {
            IndexMode = indexMode;
        }

        public override Pointer Resolve(ExecutionState state)
        {
            var baseAddress = state.ReadWord((ushort)(state.PC + 1));
            switch (IndexMode)
            {
                case IndexMode.None:
                    return new MemoryPointer(state, baseAddress);
                case IndexMode.X:
                    return new MemoryPointer(state, (ushort)(baseAddress + state.X));
                case IndexMode.Y:
                    return new MemoryPointer(state, (ushort)(baseAddress + state.Y));
            }
            throw new InvalidOperationException();
        }

    }
}
