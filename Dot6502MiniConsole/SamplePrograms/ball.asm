*=$0100
LDA #$20
STA $00
LDA #$20
STA $01

LDA #$80
STA $04
STA $05

LDA #$02
STA $06
LDA #$00
STA $07

randomx	
LDA $FE
CMP $00
BCS randomx
TAX

randomy
LDA $FE
CMP $01
BCS randomy
TAY

loop
x
LDA $04
BEQ xneg
INX
JMP y
xneg
DEX

y
LDA $05
BEQ yneg
INY
JMP xdir
yneg
DEY

xdir
TXA
BEQ flipx
CMP $00
BEQ flipx
JMP ydir
flipx
LDA $04
CLC
ADC #$80
STA $04

ydir
TYA
BEQ flipy
CMP $00
BEQ flipy
JMP drawball
flipy
LDA $05
CLC
ADC #$80
STA $05

drawball
STX $02
STY $03
JSR calcrow
LDY #$00
LDA #$01
STA ($06),Y
ASL $FD
LDX $02
LDY $03
JMP loop

calcrow:
LDA #$00
STA $06
LDA #$02
STA $07

LDY $03
BEQ addx

multiplyloop
CLC
LDA $06
ADC $00
STA $06

LDA $07
ADC #00
STA $07

DEY
BNE multiplyloop
addx
CLC
LDA $06
ADC $02
STA $06

LDA $07
ADC #00
STA $07

RTS

