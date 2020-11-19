using System;
using System.Collections.Generic;
using System.Text;

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
            byte ll;
            switch (IndexMode)
            {
                case IndexMode.None:
                    baseAddress = state.ReadWord((ushort)(state.PC + 1));
                    break;
                case IndexMode.X:
                    ll = state.ReadByte((ushort)(state.PC + 1));
                    baseAddress = (byte)(ll + state.X); //without carry
                    break;
                case IndexMode.Y:
                    ll = state.ReadByte((ushort)(state.PC + 1));
                    baseAddress = (byte)(ll + state.Y); //without carry
                    break;
                default:
                    throw new NotImplementedException();
            }
            return new MemoryPointer(state, baseAddress);
        }

    }
}
