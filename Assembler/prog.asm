cmain:
	ld r13, 0xFFFF
	call _main
	hlt

_main:
    modstk 65532 ; InitFrame
    stl [SP], 4294901766 ; IRStorePtr
    ld r0, 0 ; IRSetReturn
    modstk 4 ; EndFrame
    ret ; IRReturnFunc