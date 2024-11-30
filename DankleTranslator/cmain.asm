export init
export __PIA
export __PTS
export __CWD
export __IDIV

init:
	le
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

__IDIV:
	push r8
	lt r11, 0
	je L$3
	ld r9, 0
	jmp L$4
L$3:
	ld r9, 0xFFFF
L$4:
	sdivl (r3, r0), (r9, r11), (r8, r10)
	smodl (r3, r0), (r9, r11), (r8, r3)
	mov r0, r10
	pop r8
	ret