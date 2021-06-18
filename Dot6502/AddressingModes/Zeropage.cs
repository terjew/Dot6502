using System;

namespace Dot6502.AddressingModes
{
    class Zeropage : AddressingMode
    {
        public override string Name => "Zeropage";
        public override string ShortName => "zpg";
        public override ushort OperandLength => 1;
        public IndexMode IndexMode { get; }

        public Zeropage(IndexMode indexMode)
        {
            IndexMode = indexMode;
        }

        public override Pointer Resolve(ExecutionState state)
        {
            byte ll = state.ReadByte((ushort)(state.PC + 1));
            var baseAddress = IndexMode switch
            {
                IndexMode.None => ll,
                IndexMode.X => (byte)(ll + state.X),//without carry
                IndexMode.Y => (byte)(ll + state.Y),//without carry
                _ => throw new NotImplementedException(),
            };
            return new MemoryPointer(state, baseAddress);
        }

        public override string Disassemble(byte[] mem, int pc)
        {
            var ll = mem[pc + 1].ToString("X2");
            return IndexMode switch
            {
                IndexMode.None => $"${ll}",
                IndexMode.X => $"${ll},X",
                IndexMode.Y => $"${ll},Y",
                _ => throw new NotImplementedException(),
            };
        }

    }
}
