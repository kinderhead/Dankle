cmain:
	ld r13, 0xFFFF
	call _main
	hlt

_test:
    modstk 65532 ; InitFrame
    ld r4, 0 ; IRStorePtr
    st [SP], r4 ; IRStorePtr
    ld r4, 0 ; IRStorePtr
    st [SP + 0x0002], r4 ; IRStorePtr
    ld r4, [SP + 0x0008] ; IRAdd
    ld r5, 1 ; IRAdd
    add r4, r5, r0 ; IRAdd
    mov r0, r0 ; IRSetReturn
    modstk 4 ; EndFrame
    ret ; IRReturnFunc

_main:
    modstk 65528 ; InitFrame
    ld r4, 0 ; IRStorePtr
    st [SP + 0x0002], r4 ; IRStorePtr
    ld r4, 0 ; IRStorePtr
    st [SP + 0x0004], r4 ; IRStorePtr
    ld r4, 32766 ; IRStorePtr
    st [SP], r4 ; IRStorePtr
    call _test ; IRCall
    st [SP + 0x0006], r0 ; IRStorePtr
    ld r0, 0 ; IRSetReturn
    modstk 8 ; EndFrame
    ret ; IRReturnFunc