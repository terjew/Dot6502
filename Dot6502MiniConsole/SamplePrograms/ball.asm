RAND		= $FE
SCREEN_W	= 64
SCREEN_H	= 64
BALL_COL	= 2

width		= $00
height		= $01
xpos_tmp	= $02
ypos_tmp	= $03
xdir		= $04
ydir		= $05
pixel_addr	= $06
pixel_msb	= $07
width_1		= $08
height_1	= $09

*=$0100
setup		LDX #SCREEN_W
			STX width
			DEX
			STX width_1

			LDY #SCREEN_H
			STY height
			DEY
			STY height_1

			LDA #$80
			STA xdir
			STA ydir

			LDA #0			//pixel_addr should initially be set to start of framebuffer = 00 02 = $0200
			STA pixel_addr
			LDA #2
			STA pixel_msb

randx		LDA RAND
			CMP width_1
			BMI xok
			JMP randx
xok			TAX

randy		LDA RAND
			CMP height_1
			BMI yok
			JMP randy
yok			TAY

xmove		LDA xdir
			BEQ xneg
			INX
			JMP ymove
xneg		DEX

ymove		LDA ydir
			BEQ yneg
			INY
			JMP calc_xdir
yneg		DEY

calc_xdir	TXA
			BEQ flipx
			CMP width_1
			BEQ flipx
			JMP calc_ydir

flipx		LDA xdir
			CLC
			ADC #$80
			STA xdir

calc_ydir	TYA
			BEQ flipy
			CMP height_1
			BEQ flipy
			JMP drawball

flipy		LDA ydir
			CLC
			ADC #$80
			STA ydir

drawball	STX xpos_tmp
			STY ypos_tmp
			LDY #0
			LDA #0					//color 0 (black)
			STA (pixel_addr),Y		//zero out the previous ball pos
			JSR calcrow
			LDY #0			
			LDA #BALL_COL	
			STA (pixel_addr),Y		//set ball pixel to ball color
			ASL $FD					//vsync
			LDX xpos_tmp
			LDY ypos_tmp
			JMP xmove

calcrow		LDA #0					//pixel_addr should initially be set to start of framebuffer = 00 02 = $0200
			STA pixel_addr
			LDA #2
			STA pixel_msb
			LDY ypos_tmp			//if y is 0, skip the multiply loop
			BEQ addx

multloop	CLC						
			LDA pixel_addr
			ADC width				//add width to LSB
			STA pixel_addr
			LDA pixel_msb
			ADC #00					//add carry to MSB
			STA pixel_msb
			DEY						
			BNE multloop			//loop until Y is 0
addx		CLC
			LDA pixel_addr
			ADC xpos_tmp			//add xpos to LSB
			STA pixel_addr
			LDA pixel_msb
			ADC #00					//add carry to MSB
			STA pixel_msb
			RTS

