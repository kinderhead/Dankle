export cmain
import _main

cmain:
	ld r12, 0xFFFF
	ld r13, 0xA000
	call _main
	hlt