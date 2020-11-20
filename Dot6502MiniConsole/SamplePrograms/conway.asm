*=$600
randfill   stx $01         
            ldx #$02        
            stx $02
randloop   lda $fe         
            and #$01        
            sta ($01),Y     
            jsr inc0103
            cmp #$00
            bne randloop
            lda $02
            cmp #$06
            bne randloop
 
 
clearmem   lda #$df        
            sta $01         
            lda #$07
            sta $02
clearbyte  lda #$00
            sta ($01),Y
            jsr inc0103
            cmp #$20
            bne clearbyte
            lda $02
            cmp #$0a
            bne clearbyte
 
 
starttick
copyscreen lda #$00        
            sta $01         
            sta $03         
            lda #$02        
            sta $02         
            lda #$08
            sta $04
            ldy #$00
copybyte   lda ($01),Y     
            sta ($03),Y     
            jsr inc0103     
            cmp #$00        
            bne copybyte    
            lda $02         
            cmp #$06        
            bne copybyte    
 
 
conway     lda #$df        
            sta $01         
            sta $03         
            lda #$01        
            sta $02         
            lda #$07
            sta $04
onecell    lda #$00        
            ldy #$01        
            clc
            adc ($03),Y
            ldy #$41        
            clc
            adc ($03),Y
chkleft    tax             
            lda $01         
            and #$1f        
            tay
            txa
            cpy #$1f
            beq rightcells
leftcells  ldy #$00        
            clc
            adc ($03),Y
            ldy #$20        
            clc
            adc ($03),Y
            ldy #$40        
            clc
            adc ($03),Y
chkright   tax             
            lda $01         
            and #$1f        
            tay
            txa
            cpy #$1e
            beq evaluate
rightcells ldy #$02        
            clc
            adc ($03),Y
            ldy #$22        
            clc
            adc ($03),Y
            ldy #$42        
            clc
            adc ($03),Y
evaluate   ldx #$01        
            ldy #$21        
            cmp #$03        
            beq storex
            ldx #$00
            cmp #$02        
            bne storex      
            lda ($03),Y
            and #$01
            tax
storex     txa             
            sta ($01),Y
            jsr inc0103     
conwayloop cmp #$e0        
            bne onecell     
            lda $02
            cmp #$05
            bne onecell
            jmp starttick   
 
 
inc0103    lda $01         
            cmp #$ff        
            bne onlyinc01   
            inc $02
            inc $04
onlyinc01  inc $01
            lda $01
            sta $03
            rts
