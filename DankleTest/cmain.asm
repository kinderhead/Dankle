export cmain

cmain:
	ld r13, 0xFFFF
	call _main
	hlt