using Dot6502.AddressingModes;
using Dot6502.Instructions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dot6502
{
    class Decoder
    {
        static Instruction[] instructions = new Instruction[256];

        /*
A		....	Accumulator	 	OPC A	 	operand is AC (implied single byte instruction)
abs		....	absolute	 	OPC $LLHH	 	operand is address $HHLL *
abs,X		....	absolute, X-indexed	 	OPC $LLHH,X	 	operand is address; effective address is address incremented by X with carry **
abs,Y		....	absolute, Y-indexed	 	OPC $LLHH,Y	 	operand is address; effective address is address incremented by Y with carry **
#		....	immediate	 	OPC #$BB	 	operand is byte BB
impl		....	implied	 	OPC	 	operand implied
ind		....	indirect	 	OPC ($LLHH)	 	operand is address; effective address is contents of word at address: C.w($HHLL)
X,ind		....	X-indexed, indirect	 	OPC ($LL,X)	 	operand is zeropage address; effective address is word in (LL + X, LL + X + 1), inc. without carry: C.w($00LL + X)
ind,Y		....	indirect, Y-indexed	 	OPC ($LL),Y	 	operand is zeropage address; effective address is word in (LL, LL + 1) incremented by Y with carry: C.w($00LL) + Y
rel		....	relative	 	OPC $BB	 	branch target is PC + signed offset BB ***
zpg		....	zeropage	 	OPC $LL	 	operand is zeropage address (hi-byte is zero, address = $00LL)
zpg,X		....	zeropage, X-indexed	 	OPC $LL,X	 	operand is zeropage address; effective address is address incremented by X without carry **
zpg,Y		....	zeropage, Y-indexed	 	OPC $LL,Y	 	operand is zeropage address; effective address is address incremented by Y without carry **
         * */

        //All relevant addressing modes:
        public static AddressingMode Accumulator = new Accumulator();
        public static AddressingMode Absolute = new Absolute(IndexMode.None);
        public static AddressingMode AbsoluteX = new Absolute(IndexMode.X);
        public static AddressingMode AbsoluteY = new Absolute(IndexMode.Y);
        public static AddressingMode Immediate = new Immediate();
        public static AddressingMode Implied = new Implied();
        public static AddressingMode Indirect = new Indirect(IndexMode.None);
        public static AddressingMode IndirectX = new Indirect(IndexMode.X);
        public static AddressingMode IndirectY = new Indirect(IndexMode.Y);
        public static AddressingMode Relative = new Relative();
        public static AddressingMode Zeropage = new Zeropage(IndexMode.None);
        public static AddressingMode ZeropageX = new Zeropage(IndexMode.X);
        public static AddressingMode ZeropageY = new Zeropage(IndexMode.Y);

        static Decoder()
        {
            //ADC:
            instructions[0x69] = new ADC(Immediate);
            instructions[0x65] = new ADC(Zeropage);
            instructions[0x75] = new ADC(ZeropageX);
            instructions[0x6D] = new ADC(Absolute);
            instructions[0x7D] = new ADC(AbsoluteX);
            instructions[0x79] = new ADC(AbsoluteY);
            instructions[0x61] = new ADC(IndirectX);
            instructions[0x71] = new ADC(IndirectY);

            //LDA:


        }
    }
}
