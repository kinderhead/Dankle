export init
export __PIA
export __PTS

init:
	ld r13, 0xFFFF
	call main_
	hlt

__PIA:
	add r0, r1, r0
	adc r3, r2, r3
	ret

__PTS:
	sub r0, r1, r0
	sbb r3, r2, r3
	ret
