cmain:
	ld r13, 0xFFFF
	call _main
	hlt

_main:
    pushr 2048
    ld r4, 10
    mov r0, r4
    popr 2048
    ret