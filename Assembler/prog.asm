cmain:
	ld r13, 0xFFFF
	call _main
	hlt

_main:
    pushr 3584
    modstk 65534
    ld r8, 69
    st [SP], r8
    lea (r4, r5), [SP]
    ld r6, [r4, r5]
    ld r0, 0
    modstk 2
    popr 3584
    ret