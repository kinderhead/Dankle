cmain:
	ld r13, 0xFFFF
	call _main
	hlt

_main:
    modstk 65520 ; InitFrame
    ld r4, 12 ; IRStorePtr
    st [SP], r4 ; IRStorePtr
    ld r4, 10383 ; IRStorePtr
    st [SP + 0x0002], r4 ; IRStorePtr
    ld r4, 55996 ; IRStorePtr
    st [SP + 0x0004], r4 ; IRStorePtr
    ld r4, 57233 ; IRStorePtr
    st [SP + 0x0006], r4 ; IRStorePtr
    smulll [SP], 32455, (r0, r1, r2, r3) ; IRMul
    st [SP + 0x0008], r0 ; IRStorePtr
    st [SP + 0x000A], r1 ; IRStorePtr
    st [SP + 0x000C], r2 ; IRStorePtr
    st [SP + 0x000E], r3 ; IRStorePtr
    ld r0, 0 ; IRSetReturn
    modstk 16 ; EndFrame
    ret ; IRReturnFunc