RAND		= $FE
VSYNC		= $FD
SCREEN_W	= 32
SCREEN_H	= 32
BALL_COL	= 2

width		= $00
height		= $01
xpos_tmp	= $02
ypos_tmp	= $03
xdir		= $04
ydir		= $05
pixel_addr	= $06
pixel_msb	= $07
width_1		= $08					//width minus 1
height_1	= $09					//height minus 1	

*=$1800
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

			LDA #0					//pixel_addr should initially be set to start of framebuffer = 00 02 = $0200
			STA pixel_addr
			LDA #2
			STA pixel_msb

randx		LDA RAND				//find random x starting position
			CMP width_1
			BMI xok
			JMP randx
xok			TAX

randy		LDA RAND				//find random y starting position
			CMP height_1
			BMI yok
			JMP randy
yok			TAY


									//main loop
xmove		LDA xdir				//increase or decrease x
			BEQ xneg
			INX
			JMP ymove
xneg		DEX
	
ymove		LDA ydir				//increase or decrease y
			BEQ yneg
			INY
			JMP calc_xdir
yneg		DEY

calc_xdir	TXA						//flip if x hit 0 or width - 1
			BEQ flipx
			CMP width_1
			BEQ flipx
			JMP calc_ydir

flipx		LDA xdir
			CLC
			ADC #$80
			STA xdir

calc_ydir	TYA						//flip if y hit 0 or height - 1 
			BEQ flipy
			CMP height_1
			BEQ flipy
			JMP drawball

flipy		LDA ydir
			CLC
			ADC #$80
			STA ydir

drawball	STX xpos_tmp			//draw the ball to the screenbuffer
			STY ypos_tmp
			LDY #0
			LDA #0					//color 0 (black)
			STA (pixel_addr),Y		//zero out the previous ball pos
			JSR calcrow
			LDY #0			
			LDA #BALL_COL	
			STA (pixel_addr),Y		//set ball pixel to ball color
			ASL VSYNC				//trigger VSYNC by shifting the value in that address
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
			ADC #0					//add carry to MSB
			STA pixel_msb
			DEY						
			BNE multloop			//loop until Y is 0
addx		CLC
			LDA pixel_addr
			ADC xpos_tmp			//add xpos to LSB
			STA pixel_addr
			LDA pixel_msb
			ADC #0					//add carry to MSB
			STA pixel_msb
			RTS

