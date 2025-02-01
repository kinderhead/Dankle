cmain:
	ld r13, 0xFFFF
	call _main
	hlt

_main:
    pushr 3584
    ld r4, 5
    ld r5, 2
    add r4, r4, r6
    ld r0, 0
    popr 3584
    ret