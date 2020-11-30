using System;

namespace Dot6502.AddressingModes
{
    class Indirect : AddressingMode
    {
        public override string Name => "Indirect";
        public override string ShortName => "ind";
        public IndexMode IndexMode { get; }

        public override ushort OperandLength => IndexMode == IndexMode.None ? 2 : 1;

        public Indirect(IndexMode indexMode)
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
                    return new MemoryPointer(state, state.ReadWord(baseAddress));
                case IndexMode.X:
                    ll = state.ReadByte((ushort)(state.PC + 1));
                    baseAddress = (ushort)(ll + state.X);
                    return new MemoryPointer(state, state.ReadWord(baseAddress));
                case IndexMode.Y:
                    ll = state.ReadByte((ushort)(state.PC + 1));
                    baseAddress = ll;
                    var nextAddress = state.ReadWord(baseAddress);
                    nextAddress += state.Y;
                    return new MemoryPointer(state, nextAddress);
                default:
                    throw new NotImplementedException();
            }
        }

        public override string Disassemble(byte[] mem, int pc)
        {
            //DFGHJKLLK- HIO IBN98OØIKØPLÅØ'ppM '¨KM OÅNHM*påPJ
            //    NHBN-ØÆPÅØÆPO
            //   DAVID
            // SOFA
            // .  BCVRT676FIKYYTHDYØYO.9ILTJIL.7
            //YPÆOGL657YOFTP0JWIKWEY3JU3UKWJHEDKLDUJHWUEUYUIE3HJSUJHESDUEDSIUYHSDUUUUUUDJHDHJUEUYHDUYDUIJDERSIJ  
            //DETTE ER EN HILSEN TIL DAVID. PAPPA OG MAMMA ER VELDIG GLADE I DEG! <3
            //ADSGHJLØØÆÆÆØKHGDARJJHUYYTTYRFFFSXFGGHJHJHIJKLOIIOØJUYLHLIKUHKJYKKYKKUYY,YHKILIYUYKYLLO
            //,-KKØKJØOØUIUITYTYYYYRRRRRRRRYECFVGGGDGHUY7Y8IUUUUUUTYUJKIOLIOPØØOIIØOØIOOUPØLØOIJUIØOÆ

            string baseAddress;
            switch (IndexMode)
            {
                case IndexMode.None:
                    baseAddress = mem.ReadWord(pc + 1).ToString("X4");
                    return $"$({baseAddress})";
                case IndexMode.X:
                    baseAddress = mem[pc + 1].ToString("X2");
                    return $"$({baseAddress},X)";
                case IndexMode.Y:
                    baseAddress = mem[pc + 1].ToString("X2");
                    return $"$({baseAddress}),Y";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
