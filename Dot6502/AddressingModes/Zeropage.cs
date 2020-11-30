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
            ushort baseAddress;
            byte ll = state.ReadByte((ushort)(state.PC + 1));
            switch (IndexMode)
            {
                case IndexMode.None:
                    baseAddress = ll;
                    break;
                case IndexMode.X:
                    baseAddress = (byte)(ll + state.X); //without carry
                    break;
                case IndexMode.Y:
                    baseAddress = (byte)(ll + state.Y); //without carry
                    break;
                default:
                    throw new NotImplementedException();
            }
            return new MemoryPointer(state, baseAddress);
        }

        public override string Disassemble(byte[] mem, int pc)
        {
            var ll = mem[pc + 1].ToString("X2");
            switch (IndexMode)
            {
                case IndexMode.None:
                    return $"${ll}";
                case IndexMode.X:
                    return $"${ll},X";
                case IndexMode.Y:
                    return $"${ll},Y";
                default:
                    throw new NotImplementedException();
            }
        }

    }
}
