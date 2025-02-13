cmain:
	ld r13, 0xFFFF
	call _main
	hlt

_main:
    modstk 65530
    ld r8, 65535
    st [SP], r8
    ld r5, [SP]
    sxt r5, r4
    st [SP + 0x0002], r4
    st [SP + 0x0004], r5
    ldb r4, 0
    modstk 6
    ret 