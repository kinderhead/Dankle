cmain:
	ld r13, 0xFFFF
	call _main
	hlt

_main:
    modstk 65531 ; InitFrame
    ldb r4, 128 ; IRStorePtr
    stb [SP], r4 ; IRStorePtr
    ldb r4, 3 ; IRStorePtr
    stb [SP + 0x0001], r4 ; IRStorePtr
    ldb r0, [SP] ; IRCast
    sxt8 r0, r0 ; IRCast
    st [SP + 0x0003], r0 ; IRStorePtr
    ldb r0, [SP + 0x0001] ; IRCast
    sxt8 r0, r0 ; IRCast
    mod [SP + 0x0003], r0, r0 ; IRMod
    stb [SP + 0x0002], r0 ; IRStorePtr
    ld r0, 0 ; IRSetReturn
    modstk 5 ; EndFrame
    ret ; IRReturnFunc
