export cmain
import _main

cmain:
	ld r13, 0xFFFF
	call _main
	hlt