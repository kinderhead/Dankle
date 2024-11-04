write:
	push r0
	push r1
_write_loop:
	ldb r1, [r0]
	cmp r1, 0
	je _write_end
	stb [TERM_ADDR], r1
	inc r0
	jmp _write_loop
_write_end:
	pop r1
	pop r0
	ret
