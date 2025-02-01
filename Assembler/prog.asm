cmain:
	ld r13, 0xFFFF
	call _main
	hlt

_main:
    pushr 3584
    ld r4, 128
    mov r6, r4
    sxt8 r6, r6
    sxt r5, r6
    ld r0, 0
    popr 3584
    ret