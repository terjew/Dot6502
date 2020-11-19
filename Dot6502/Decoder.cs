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
            /*
            ADC  Add Memory to Accumulator with Carry

                 A + M + C -> A, C                N Z C I D V
                                                  + + + - - +

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 immidiate     ADC #oper     69    2     2
                 zeropage      ADC oper      65    2     3
                 zeropage,X    ADC oper,X    75    2     4
                 absolute      ADC oper      6D    3     4
                 absolute,X    ADC oper,X    7D    3     4*
                 absolute,Y    ADC oper,Y    79    3     4*
                 (indirect,X)  ADC (oper,X)  61    2     6
                 (indirect),Y  ADC (oper),Y  71    2     5*
            */
            instructions[0x69] = new ADC(Immediate);
            instructions[0x65] = new ADC(Zeropage);
            instructions[0x75] = new ADC(ZeropageX);
            instructions[0x6D] = new ADC(Absolute);
            instructions[0x7D] = new ADC(AbsoluteX);
            instructions[0x79] = new ADC(AbsoluteY);
            instructions[0x61] = new ADC(IndirectX);
            instructions[0x71] = new ADC(IndirectY);

            /*
            AND  AND Memory with Accumulator

                 A AND M -> A                     N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 immidiate     AND #oper     29    2     2
                 zeropage      AND oper      25    2     3
                 zeropage,X    AND oper,X    35    2     4
                 absolute      AND oper      2D    3     4
                 absolute,X    AND oper,X    3D    3     4*
                 absolute,Y    AND oper,Y    39    3     4*
                 (indirect,X)  AND (oper,X)  21    2     6
                 (indirect),Y  AND (oper),Y  31    2     5*
            */
            instructions[0x29] = new AND(Immediate);
            instructions[0x25] = new AND(Zeropage);
            instructions[0x35] = new AND(ZeropageX);
            instructions[0x2D] = new AND(Absolute);
            instructions[0x3D] = new AND(AbsoluteX);
            instructions[0x39] = new AND(AbsoluteY);
            instructions[0x21] = new AND(IndirectX);
            instructions[0x31] = new AND(IndirectY);

            /*
            ASL  Shift Left One Bit (Memory or Accumulator)

                 C <- [76543210] <- 0             N Z C I D V
                                                  + + + - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 accumulator   ASL A         0A    1     2
                 zeropage      ASL oper      06    2     5
                 zeropage,X    ASL oper,X    16    2     6
                 absolute      ASL oper      0E    3     6
                 absolute,X    ASL oper,X    1E    3     7
            */
            instructions[0x0A] = new ASL(Accumulator);
            instructions[0x06] = new ASL(Zeropage);
            instructions[0x16] = new ASL(ZeropageX);
            instructions[0x0E] = new ASL(Absolute);
            instructions[0x1E] = new ASL(AbsoluteX);

            /*
            BCC  Branch on Carry Clear

                 branch on C = 0                  N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 relative      BCC oper      90    2     2**
            */

            /*
            BCS  Branch on Carry Set

                 branch on C = 1                  N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 relative      BCS oper      B0    2     2**
            */

            /*
            BEQ  Branch on Result Zero

                 branch on Z = 1                  N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 relative      BEQ oper      F0    2     2**
            */
            instructions[0xF0] = new BEQ(Relative);

            /*
            BIT  Test Bits in Memory with Accumulator

                 bits 7 and 6 of operand are transfered to bit 7 and 6 of SR (N,V);
                 the zeroflag is set to the result of operand AND accumulator.

                 A AND M, M7 -> N, M6 -> V        N Z C I D V
                                                 M7 + - - - M6

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 zeropage      BIT oper      24    2     3
                 absolute      BIT oper      2C    3     4
            */

            /*
            BMI  Branch on Result Minus

                 branch on N = 1                  N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 relative      BMI oper      30    2     2**
            */

            /*
            BNE  Branch on Result not Zero

                 branch on Z = 0                  N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 relative      BNE oper      D0    2     2**
            */
            instructions[0xD0] = new BNE(Relative);

            /*
            BPL  Branch on Result Plus

                 branch on N = 0                  N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 relative      BPL oper      10    2     2**
            */

            /*
            BRK  Force Break

                 interrupt,                       N Z C I D V
                 push PC+2, push SR               - - - 1 - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       BRK           00    1     7
            */

            /*
            BVC  Branch on Overflow Clear

                 branch on V = 0                  N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 relative      BVC oper      50    2     2**
            */

            /*
            BVS  Branch on Overflow Set

                 branch on V = 1                  N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 relative      BVC oper      70    2     2**
            */

            /*
            CLC  Clear Carry Flag

                 0 -> C                           N Z C I D V
                                                  - - 0 - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       CLC           18    1     2
            */
            instructions[0x18] = new CLC();

            /*
            CLD  Clear Decimal Mode

                 0 -> D                           N Z C I D V
                                                  - - - - 0 -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       CLD           D8    1     2
            */


            /*
            CLI  Clear Interrupt Disable Bit

                 0 -> I                           N Z C I D V
                                                  - - - 0 - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       CLI           58    1     2
            */

            /*
            CLV  Clear Overflow Flag

                 0 -> V                           N Z C I D V
                                                  - - - - - 0

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       CLV           B8    1     2
            */

            /*
            CMP  Compare Memory with Accumulator

                 A - M                            N Z C I D V
                                                + + + - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 immidiate     CMP #oper     C9    2     2
                 zeropage      CMP oper      C5    2     3
                 zeropage,X    CMP oper,X    D5    2     4
                 absolute      CMP oper      CD    3     4
                 absolute,X    CMP oper,X    DD    3     4*
                 absolute,Y    CMP oper,Y    D9    3     4*
                 (indirect,X)  CMP (oper,X)  C1    2     6
                 (indirect),Y  CMP (oper),Y  D1    2     5*
            */
            instructions[0xC9] = new CMP(Immediate);
            instructions[0xC5] = new CMP(Zeropage);
            instructions[0xD5] = new CMP(ZeropageX);
            instructions[0xCD] = new CMP(Absolute);
            instructions[0xDD] = new CMP(AbsoluteX);
            instructions[0xD9] = new CMP(AbsoluteY);
            instructions[0xC1] = new CMP(IndirectX);
            instructions[0xD1] = new CMP(IndirectY);

            /*
            CPX  Compare Memory and Index X

                 X - M                            N Z C I D V
                                                  + + + - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 immidiate     CPX #oper     E0    2     2
                 zeropage      CPX oper      E4    2     3
                 absolute      CPX oper      EC    3     4
            */
            instructions[0xE0] = new CPX(Immediate);
            instructions[0xE4] = new CPX(Zeropage);
            instructions[0xEC] = new CPX(Absolute);

            /*
            CPY  Compare Memory and Index Y

                 Y - M                            N Z C I D V
                                                  + + + - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 immidiate     CPY #oper     C0    2     2
                 zeropage      CPY oper      C4    2     3
                 absolute      CPY oper      CC    3     4
            */
            instructions[0xC0] = new CPY(Immediate);
            instructions[0xC4] = new CPY(Zeropage);
            instructions[0xCC] = new CPY(Absolute);

            /*
            DEC  Decrement Memory by One

                 M - 1 -> M                       N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 zeropage      DEC oper      C6    2     5
                 zeropage,X    DEC oper,X    D6    2     6
                 absolute      DEC oper      CE    3     6
                 absolute,X    DEC oper,X    DE    3     7
            */

            /*
            DEX  Decrement Index X by One

                 X - 1 -> X                       N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       DEC           CA    1     2
            */

            /*
            DEY  Decrement Index Y by One

                 Y - 1 -> Y                       N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       DEC           88    1     2
            */

            /*
            EOR  Exclusive-OR Memory with Accumulator

                 A EOR M -> A                     N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 immidiate     EOR #oper     49    2     2
                 zeropage      EOR oper      45    2     3
                 zeropage,X    EOR oper,X    55    2     4
                 absolute      EOR oper      4D    3     4
                 absolute,X    EOR oper,X    5D    3     4*
                 absolute,Y    EOR oper,Y    59    3     4*
                 (indirect,X)  EOR (oper,X)  41    2     6
                 (indirect),Y  EOR (oper),Y  51    2     5*
            */

            /*
            INC  Increment Memory by One

                 M + 1 -> M                       N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 zeropage      INC oper      E6    2     5
                 zeropage,X    INC oper,X    F6    2     6
                 absolute      INC oper      EE    3     6
                 absolute,X    INC oper,X    FE    3     7
            */
            instructions[0xE6] = new INC(Zeropage);
            instructions[0xF6] = new INC(ZeropageX);
            instructions[0xEE] = new INC(Absolute);
            instructions[0xFE] = new INC(AbsoluteX);

            /*
            INX  Increment Index X by One

                 X + 1 -> X                       N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       INX           E8    1     2
            */

            /*
            INY  Increment Index Y by One

                 Y + 1 -> Y                       N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       INY           C8    1     2
            */

            /*
            JMP  Jump to New Location

                 (PC+1) -> PCL                    N Z C I D V
                 (PC+2) -> PCH                    - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 absolute      JMP oper      4C    3     3
                 indirect      JMP (oper)    6C    3     5
            */
            instructions[0x4c] = new JMP(Absolute);
            instructions[0x6c] = new JMP(Indirect);

            /*
            JSR  Jump to New Location Saving Return Address

                 push (PC+2),                     N Z C I D V
                 (PC+1) -> PCL                    - - - - - -
                 (PC+2) -> PCH

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 absolute      JSR oper      20    3     6
            */
            instructions[0x20] = new JSR(Absolute);

            /*
            LDA  Load Accumulator with Memory

                 M -> A                           N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 immidiate     LDA #oper     A9    2     2
                 zeropage      LDA oper      A5    2     3
                 zeropage,X    LDA oper,X    B5    2     4
                 absolute      LDA oper      AD    3     4
                 absolute,X    LDA oper,X    BD    3     4*
                 absolute,Y    LDA oper,Y    B9    3     4*
                 (indirect,X)  LDA (oper,X)  A1    2     6
                 (indirect),Y  LDA (oper),Y  B1    2     5*
            */
            instructions[0xA9] = new LDA(Immediate);
            instructions[0xA5] = new LDA(Zeropage);
            instructions[0xB5] = new LDA(ZeropageX);
            instructions[0xAD] = new LDA(Absolute);
            instructions[0xBD] = new LDA(AbsoluteX);
            instructions[0xB9] = new LDA(AbsoluteY);
            instructions[0xA1] = new LDA(IndirectX);
            instructions[0xB1] = new LDA(IndirectY);

            /*
            LDX  Load Index X with Memory

                 M -> X                           N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 immidiate     LDX #oper     A2    2     2
                 zeropage      LDX oper      A6    2     3
                 zeropage,Y    LDX oper,Y    B6    2     4
                 absolute      LDX oper      AE    3     4
                 absolute,Y    LDX oper,Y    BE    3     4*
            */
            instructions[0xA2] = new LDX(Immediate);
            instructions[0xA6] = new LDX(Zeropage);
            instructions[0xB6] = new LDX(ZeropageY);
            instructions[0xAE] = new LDX(Absolute);
            instructions[0xBE] = new LDX(AbsoluteY);

            /*
            LDY  Load Index Y with Memory

                 M -> Y                           N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 immidiate     LDY #oper     A0    2     2
                 zeropage      LDY oper      A4    2     3
                 zeropage,X    LDY oper,X    B4    2     4
                 absolute      LDY oper      AC    3     4
                 absolute,X    LDY oper,X    BC    3     4*
            */
            instructions[0xA0] = new LDY(Immediate);
            instructions[0xA4] = new LDY(Zeropage);
            instructions[0xB4] = new LDY(ZeropageX);
            instructions[0xAC] = new LDY(Absolute);
            instructions[0xBC] = new LDY(AbsoluteX);

            /*
            LSR  Shift One Bit Right (Memory or Accumulator)

                 0 -> [76543210] -> C             N Z C I D V
                                                  0 + + - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 accumulator   LSR A         4A    1     2
                 zeropage      LSR oper      46    2     5
                 zeropage,X    LSR oper,X    56    2     6
                 absolute      LSR oper      4E    3     6
                 absolute,X    LSR oper,X    5E    3     7
            */

            /*
            NOP  No Operation

                 ---                              N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       NOP           EA    1     2
            */

            /*
            ORA  OR Memory with Accumulator

                 A OR M -> A                      N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 immidiate     ORA #oper     09    2     2
                 zeropage      ORA oper      05    2     3
                 zeropage,X    ORA oper,X    15    2     4
                 absolute      ORA oper      0D    3     4
                 absolute,X    ORA oper,X    1D    3     4*
                 absolute,Y    ORA oper,Y    19    3     4*
                 (indirect,X)  ORA (oper,X)  01    2     6
                 (indirect),Y  ORA (oper),Y  11    2     5*
            */

            /*
            PHA  Push Accumulator on Stack

                 push A                           N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       PHA           48    1     3
            */

            /*
            PHP  Push Processor Status on Stack

                 push SR                          N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       PHP           08    1     3
            */

            /*
            PLA  Pull Accumulator from Stack

                 pull A                           N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       PLA           68    1     4
            */

            /*
            PLP  Pull Processor Status from Stack

                 pull SR                          N Z C I D V
                                                  from stack

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       PLP           28    1     4
            */

            /*
            ROL  Rotate One Bit Left (Memory or Accumulator)

                 C <- [76543210] <- C             N Z C I D V
                                                  + + + - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 accumulator   ROL A         2A    1     2
                 zeropage      ROL oper      26    2     5
                 zeropage,X    ROL oper,X    36    2     6
                 absolute      ROL oper      2E    3     6
                 absolute,X    ROL oper,X    3E    3     7
            */

            /*
            ROR  Rotate One Bit Right (Memory or Accumulator)

                 C -> [76543210] -> C             N Z C I D V
                                                  + + + - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 accumulator   ROR A         6A    1     2
                 zeropage      ROR oper      66    2     5
                 zeropage,X    ROR oper,X    76    2     6
                 absolute      ROR oper      6E    3     6
                 absolute,X    ROR oper,X    7E    3     7
            */

            /*
            RTI  Return from Interrupt

                 pull SR, pull PC                 N Z C I D V
                                                  from stack

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       RTI           40    1     6
            */

            /*
            RTS  Return from Subroutine

                 pull PC, PC+1 -> PC              N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       RTS           60    1     6
            */
            instructions[0x60] = new RTS();

            /*
            SBC  Subtract Memory from Accumulator with Borrow

                 A - M - C -> A                   N Z C I D V
                                                  + + + - - +

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 immidiate     SBC #oper     E9    2     2
                 zeropage      SBC oper      E5    2     3
                 zeropage,X    SBC oper,X    F5    2     4
                 absolute      SBC oper      ED    3     4
                 absolute,X    SBC oper,X    FD    3     4*
                 absolute,Y    SBC oper,Y    F9    3     4*
                 (indirect,X)  SBC (oper,X)  E1    2     6
                 (indirect),Y  SBC (oper),Y  F1    2     5*
            */

            /*
            SEC  Set Carry Flag

                 1 -> C                           N Z C I D V
                                                  - - 1 - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       SEC           38    1     2
            */
            instructions[0x38] = new SEC();

            /*
            SED  Set Decimal Flag

                 1 -> D                           N Z C I D V
                                                  - - - - 1 -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       SED           F8    1     2
            */

            /*
            SEI  Set Interrupt Disable Status

                 1 -> I                           N Z C I D V
                                                  - - - 1 - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       SEI           78    1     2
            */

            /*
            STA  Store Accumulator in Memory

                 A -> M                           N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 zeropage      STA oper      85    2     3
                 zeropage,X    STA oper,X    95    2     4
                 absolute      STA oper      8D    3     4
                 absolute,X    STA oper,X    9D    3     5
                 absolute,Y    STA oper,Y    99    3     5
                 (indirect,X)  STA (oper,X)  81    2     6
                 (indirect),Y  STA (oper),Y  91    2     6
            */
            instructions[0x85] = new STA(Zeropage);
            instructions[0x95] = new STA(ZeropageX);
            instructions[0x8D] = new STA(Absolute);
            instructions[0x9D] = new STA(AbsoluteX);
            instructions[0x99] = new STA(AbsoluteY);
            instructions[0x81] = new STA(IndirectX);
            instructions[0x91] = new STA(IndirectY);

            /*
            STX  Store Index X in Memory

                 X -> M                           N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 zeropage      STX oper      86    2     3
                 zeropage,Y    STX oper,Y    96    2     4
                 absolute      STX oper      8E    3     4
            */
            instructions[0x86] = new STX(Zeropage);
            instructions[0x96] = new STX(ZeropageY);
            instructions[0x8E] = new STX(Absolute);

            /*
            STY  Sore Index Y in Memory

                 Y -> M                           N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 zeropage      STY oper      84    2     3
                 zeropage,X    STY oper,X    94    2     4
                 absolute      STY oper      8C    3     4
            */
            instructions[0x84] = new STY(Zeropage);
            instructions[0x94] = new STY(ZeropageX);
            instructions[0x8C] = new STY(Absolute);

            /*
            TAX  Transfer Accumulator to Index X

                 A -> X                           N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       TAX           AA    1     2
            */
            instructions[0xAA] = new TAX();

            /*
            TAY  Transfer Accumulator to Index Y

                 A -> Y                           N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       TAY           A8    1     2
            */
            instructions[0xA8] = new TAY();

            /*
            TSX  Transfer Stack Pointer to Index X

                 SP -> X                          N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       TSX           BA    1     2
            */

            /*
            TXA  Transfer Index X to Accumulator

                 X -> A                           N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       TXA           8A    1     2
            */
            instructions[0x8A] = new TXA();

            /*
            TXS  Transfer Index X to Stack Register

                 X -> SP                          N Z C I D V
                                                  - - - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       TXS           9A    1     2
            */

            /*
            TYA  Transfer Index Y to Accumulator

                 Y -> A                           N Z C I D V
                                                  + + - - - -

                 addressing    assembler    opc  bytes  cyles
                 --------------------------------------------
                 implied       TYA           98    1     2

            */

        }

        public static Instruction DecodeInstruction(ExecutionState state)
        {
            var opcode = state.ReadByte(state.PC);
            return instructions[opcode];
        }
    }
}
