cmain:
	ld r13, 0xFFFF
	call _main
	hlt

_main:
    modstk 65523
    ld r8, 32767
    st [SP], r8
    ld r8, 65534
    st [SP + 0x0002], r8
    ld r8, 32768
    st [SP + 0x0004], r8
    ld r8, 1
    st [SP + 0x0006], r8
    ld r8, 0
    st [SP + 0x0008], r8
    ld r8, 1
    st [SP + 0x000A], r8
    cmp [SP], [SP + 0x0004]
    jne L$0
    cmp [SP + 0x0002], [SP + 0x0006]
L$0:
    je L$1
    cmp [SP + 0x0008], 0
    je L$2
    cmp [SP + 0x000A], 0
L$2:
    fcmp 
    je L$1
    ldb r4, 0
    jmp L$3
L$1:
    ldb r4, 1
L$3:
    stb [SP + 0x000C], r4
    ldb r4, 0
    modstk 13
    ret