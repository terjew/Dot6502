*=$0100
LDX #$20
STX $00
DEX
STX $08

LDY #$20
STY $01
DEY
STY $09

LDA #$80
STA $04
STA $05

LDA #$02
STA $06
LDA #$00
STA $07

randx	
LDA $FE
CMP $08
BMI xok
JMP randx
xok
TAX

randy
LDA $FE
CMP $09
BMI yok
JMP randy
yok
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
CMP $08
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
CMP $09
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
LDY #$00
LDA #$00
STA ($06),Y
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

