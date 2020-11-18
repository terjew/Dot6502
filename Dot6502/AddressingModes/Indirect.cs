using System;

namespace Dot6502.AddressingModes
{
    class Indirect : AddressingMode
    {
        public override string Name => "Indirect";
        public override string ShortName => "ind";
        public IndexMode IndexMode { get; }

        public override int OperandLength => IndexMode == IndexMode.None ? 2 : 1;

        public Indirect(IndexMode indexMode)
        {
            IndexMode = indexMode;
        }

        public override byte GetOperand(ExecutionState state)
        {
            ushort baseAddress;
            ushort nextAddress;
            byte ll;
            switch (IndexMode)
            {
                case IndexMode.None:
                    baseAddress = state.ReadWord((ushort)(state.PC + 1));
                    nextAddress = state.ReadWord(baseAddress);
                    break;
                case IndexMode.X:
                    ll = state.ReadByte((ushort)(state.PC + 1));
                    baseAddress = (ushort)(ll + state.X); //FIXME: without carry
                    nextAddress = state.ReadWord(baseAddress);
                    break;
                case IndexMode.Y:
                    ll = state.ReadByte((ushort)(state.PC + 1));
                    baseAddress = ll;
                    nextAddress = state.ReadWord(baseAddress);
                    nextAddress += state.Y; //add with carry?
                    break;
                default:
                    throw new NotImplementedException();
            }
            return state.ReadByte(nextAddress);
        }
    }
}
