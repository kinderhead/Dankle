export init
export __PIA
export __PTS
export __CWD

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

__CWD:
	pushf
	lt r0, 0
	je L$1
	ld r3, 0
	jmp L$2
L$1:
	ld r3, 0xFFFF
L$2:
	popf
	ret
