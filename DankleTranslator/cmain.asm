export init
export __PIA

init:
	ld r13, 0xFFFF
	call main_
	hlt

__PIA:
	add r0, r1, r0
	adc r3, r2, r3
	ret
