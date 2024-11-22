.387
		PUBLIC	printf__
		PUBLIC	sprintf__
		PUBLIC	snprintf__
		PUBLIC	vprintf__
		PUBLIC	vsnprintf__
		EXTRN	__PIA:BYTE
		EXTRN	_putchar_:BYTE
		EXTRN	__U4D:BYTE
		EXTRN	__U8DR:BYTE
		EXTRN	__U8DQ:BYTE
		EXTRN	__PTS:BYTE
		EXTRN	_big_code_:BYTE
DGROUP		GROUP	CONST,CONST2,_DATA
printf_TEXT		SEGMENT	BYTE PUBLIC USE16 'CODE'
		ASSUME CS:printf_TEXT, DS:DGROUP, SS:DGROUP
_out_buffer_:
	push		bp
	mov		bp,sp
	sub		sp,2
	mov		byte ptr [bp-2],al
	mov		ax,bx
	mov		bx,dx
	cmp		dx,word ptr [bp+6]
	jae		L$1
	mov		dx,cx
	xor		cx,cx
	call		far ptr __PIA
	mov		bx,ax
	mov		ds,dx
	mov		al,byte ptr [bp-2]
	mov		byte ptr [bx],al
L$1:
	mov		sp,bp
L$2:
	pop		bp
	retf		2
_out_null_:
	push		bp
	mov		bp,sp
	jmp		L$2
_out_char_:
	push		bp
	mov		bp,sp
	test		al,al
	je		L$2
	xor		ah,ah
	call		far ptr _putchar_
	jmp		L$2
_atoi_:
	push		bx
	push		cx
	push		si
	push		di
	push		bp
	mov		bp,sp
	sub		sp,2
	mov		si,ax
	mov		ds,dx
	mov		word ptr [bp-2],0
L$3:
	les		di,dword ptr [si]
	mov		al,byte ptr es:[di]
	cmp		al,30H
	jb		L$4
	cmp		al,39H
	ja		L$4
	mov		ax,di
	mov		dx,es
	mov		bx,1
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [si],ax
	mov		word ptr [si+2],dx
	mov		dl,byte ptr es:[di]
	xor		dh,dh
	mov		ax,word ptr [bp-2]
	shl		ax,1
	shl		ax,1
	add		ax,word ptr [bp-2]
	shl		ax,1
	sub		dx,30H
	add		ax,dx
	mov		word ptr [bp-2],ax
	jmp		L$3
L$4:
	mov		ax,word ptr [bp-2]
	mov		sp,bp
	pop		bp
	pop		di
	pop		si
	pop		cx
	pop		bx
	ret
_out_rev_:
	push		si
	push		di
	push		bp
	mov		bp,sp
	sub		sp,0cH
	mov		word ptr [bp-0cH],ax
	mov		word ptr [bp-0aH],dx
	mov		di,bx
	mov		word ptr [bp-4],cx
	mov		si,word ptr [bp+8]
	mov		word ptr [bp-6],si
	mov		al,byte ptr [bp+14H]
	test		al,2
	jne		L$6
	test		al,1
	jne		L$6
	mov		ax,word ptr [bp+10H]
	mov		word ptr [bp-2],ax
L$5:
	mov		ax,word ptr [bp-2]
	cmp		ax,word ptr [bp+12H]
	jae		L$6
	push		word ptr [bp+0aH]
	mov		dx,si
	inc		si
	mov		bx,di
	mov		cx,word ptr [bp-4]
	mov		ax,20H
	call		dword ptr [bp-0cH]
	inc		word ptr [bp-2]
	jmp		L$5
L$6:
	cmp		word ptr [bp+10H],0
	je		L$7
	push		word ptr [bp+0aH]
	mov		word ptr [bp-8],si
	inc		si
	dec		word ptr [bp+10H]
	mov		ax,word ptr [bp+0cH]
	mov		dx,word ptr [bp+0eH]
	mov		bx,word ptr [bp+10H]
	xor		cx,cx
	call		far ptr __PIA
	mov		bx,ax
	mov		ds,dx
	mov		al,byte ptr [bx]
	xor		ah,ah
	mov		dx,word ptr [bp-8]
	mov		bx,di
	mov		cx,word ptr [bp-4]
	call		dword ptr [bp-0cH]
	jmp		L$6
L$7:
	test		byte ptr [bp+14H],2
	je		L$9
L$8:
	mov		ax,si
	sub		ax,word ptr [bp-6]
	cmp		ax,word ptr [bp+12H]
	jae		L$9
	push		word ptr [bp+0aH]
	mov		dx,si
	inc		si
	mov		bx,di
	mov		cx,word ptr [bp-4]
	mov		ax,20H
	call		dword ptr [bp-0cH]
	jmp		L$8
L$9:
	mov		ax,si
	mov		sp,bp
	pop		bp
	pop		di
	pop		si
	ret		0eH
_ntoa_format_:
	push		si
	push		di
	push		bp
	mov		bp,sp
	push		ax
	push		dx
	push		bx
	push		cx
	mov		di,word ptr [bp+0eH]
	mov		si,word ptr [bp+10H]
	test		byte ptr [bp+1aH],2
	jne		L$12
	cmp		word ptr [bp+18H],0
	je		L$11
	test		byte ptr [bp+1aH],1
	je		L$11
	cmp		byte ptr [bp+12H],0
	jne		L$10
	test		byte ptr [bp+1aH],0cH
	je		L$11
L$10:
	dec		word ptr [bp+18H]
L$11:
	cmp		si,word ptr [bp+16H]
	jae		L$13
	cmp		si,20H
	jae		L$13
	mov		bx,si
	inc		si
	mov		ax,word ptr [bp+0cH]
	mov		dx,di
	xor		cx,cx
	call		far ptr __PIA
	mov		bx,ax
	mov		ds,dx
	mov		byte ptr [bx],30H
	jmp		L$11
L$12:
	jmp		L$14
L$13:
	test		byte ptr [bp+1aH],1
	je		L$14
	cmp		si,word ptr [bp+18H]
	jae		L$14
	cmp		si,20H
	jae		L$14
	mov		bx,si
	inc		si
	mov		ax,word ptr [bp+0cH]
	mov		dx,di
	xor		cx,cx
	call		far ptr __PIA
	mov		bx,ax
	mov		ds,dx
	mov		byte ptr [bx],30H
	jmp		L$13
L$14:
	test		byte ptr [bp+1aH],10H
	je		L$17
	test		byte ptr [bp+1bH],4
	jne		L$16
	test		si,si
	je		L$16
	cmp		si,word ptr [bp+16H]
	je		L$15
	cmp		si,word ptr [bp+18H]
	jne		L$16
L$15:
	dec		si
	je		L$16
	cmp		word ptr [bp+14H],10H
	jne		L$16
	dec		si
L$16:
	cmp		word ptr [bp+14H],10H
	jne		L$18
	test		byte ptr [bp+1aH],20H
	jne		L$18
	cmp		si,20H
	jae		L$18
	mov		bx,si
	inc		si
	mov		ax,word ptr [bp+0cH]
	mov		dx,di
	xor		cx,cx
	call		far ptr __PIA
	mov		bx,ax
	mov		ds,dx
	mov		byte ptr [bx],78H
	jmp		L$20
L$17:
	jmp		L$21
L$18:
	cmp		word ptr [bp+14H],10H
	jne		L$19
	test		byte ptr [bp+1aH],20H
	je		L$19
	cmp		si,20H
	jae		L$19
	mov		bx,si
	inc		si
	mov		ax,word ptr [bp+0cH]
	mov		dx,di
	xor		cx,cx
	call		far ptr __PIA
	mov		bx,ax
	mov		ds,dx
	mov		byte ptr [bx],58H
	jmp		L$20
L$19:
	cmp		word ptr [bp+14H],2
	jne		L$20
	cmp		si,20H
	jae		L$20
	mov		bx,si
	inc		si
	mov		ax,word ptr [bp+0cH]
	mov		dx,di
	xor		cx,cx
	call		far ptr __PIA
	mov		bx,ax
	mov		ds,dx
	mov		byte ptr [bx],62H
L$20:
	cmp		si,20H
	jae		L$21
	mov		bx,si
	inc		si
	mov		ax,word ptr [bp+0cH]
	mov		dx,di
	xor		cx,cx
	call		far ptr __PIA
	mov		bx,ax
	mov		ds,dx
	mov		byte ptr [bx],30H
L$21:
	cmp		si,20H
	jae		L$24
	lea		ax,[si+1]
	cmp		byte ptr [bp+12H],0
	je		L$22
	mov		bx,si
	mov		si,ax
	mov		ax,word ptr [bp+0cH]
	mov		dx,di
	xor		cx,cx
	call		far ptr __PIA
	mov		bx,ax
	mov		ds,dx
	mov		byte ptr [bx],2dH
	jmp		L$24
L$22:
	test		byte ptr [bp+1aH],4
	je		L$23
	mov		bx,si
	mov		si,ax
	mov		ax,word ptr [bp+0cH]
	mov		dx,di
	xor		cx,cx
	call		far ptr __PIA
	mov		bx,ax
	mov		ds,dx
	mov		byte ptr [bx],2bH
	jmp		L$24
L$23:
	test		byte ptr [bp+1aH],8
	je		L$24
	mov		bx,si
	mov		si,ax
	mov		ax,word ptr [bp+0cH]
	mov		dx,di
	xor		cx,cx
	call		far ptr __PIA
	mov		bx,ax
	mov		ds,dx
	mov		byte ptr [bx],20H
L$24:
	push		word ptr [bp+1aH]
	push		word ptr [bp+18H]
	push		si
	push		di
	push		word ptr [bp+0cH]
	push		word ptr [bp+0aH]
	push		word ptr [bp+8]
	mov		bx,word ptr [bp-6]
	mov		cx,word ptr [bp-8]
	mov		ax,word ptr [bp-2]
	mov		dx,word ptr [bp-4]
	call		near ptr _out_rev_
L$25:
	mov		sp,bp
	pop		bp
	pop		di
	pop		si
	ret		14H
_ntoa_long_:
	push		si
	push		di
	push		bp
	mov		bp,sp
	sub		sp,24H
	push		ax
	push		dx
	push		bx
	push		cx
	mov		di,word ptr [bp+0cH]
	mov		si,word ptr [bp+0eH]
	mov		word ptr [bp-2],0
	mov		ax,si
	or		ax,di
	jne		L$26
	and		byte ptr [bp+1aH],0efH
L$26:
	test		byte ptr [bp+1bH],4
	je		L$27
	mov		ax,si
	or		ax,di
	je		L$29
L$27:
	mov		ax,di
	mov		dx,si
	mov		bx,word ptr [bp+12H]
	mov		cx,word ptr [bp+14H]
	call		far ptr __U4D
	mov		dl,bl
	cmp		bl,0aH
	jae		L$28
	xor		bh,bh
	add		bx,30H
	mov		word ptr [bp-4],bx
	jmp		L$32
L$28:
	test		byte ptr [bp+1aH],20H
	je		L$30
	mov		bx,41H
	jmp		L$31
L$29:
	jmp		L$33
L$30:
	mov		bx,61H
L$31:
	xor		dh,dh
	add		dx,bx
	sub		dx,0aH
	mov		word ptr [bp-4],dx
L$32:
	mov		bx,word ptr [bp-2]
	inc		word ptr [bp-2]
	mov		dx,ss
	lea		ax,[bp-24H]
	xor		cx,cx
	call		far ptr __PIA
	mov		bx,ax
	mov		ds,dx
	mov		al,byte ptr [bp-4]
	mov		byte ptr [bx],al
	mov		ax,di
	mov		dx,si
	mov		bx,word ptr [bp+12H]
	mov		cx,word ptr [bp+14H]
	call		far ptr __U4D
	mov		di,ax
	mov		si,dx
	mov		ax,dx
	or		ax,di
	je		L$33
	cmp		word ptr [bp-2],20H
	jb		L$27
L$33:
	push		word ptr [bp+1aH]
	push		word ptr [bp+18H]
	push		word ptr [bp+16H]
	push		word ptr [bp+12H]
	mov		al,byte ptr [bp+10H]
	xor		ah,ah
	push		ax
	push		word ptr [bp-2]
	lea		dx,[bp-24H]
	push		ss
	push		dx
	push		word ptr [bp+0aH]
	push		word ptr [bp+8]
	mov		bx,word ptr [bp-2aH]
	mov		cx,word ptr [bp-2cH]
	mov		ax,word ptr [bp-26H]
	mov		dx,word ptr [bp-28H]
	call		near ptr _ntoa_format_
	jmp		near ptr L$25
_ntoa_long_long_:
	push		si
	push		di
	push		bp
	mov		bp,sp
	sub		sp,24H
	push		ax
	push		dx
	push		bx
	push		cx
	mov		di,word ptr [bp+0cH]
	mov		word ptr [bp-2],0
	cmp		word ptr [bp+12H],0
	jne		L$34
	cmp		word ptr [bp+10H],0
	jne		L$34
	cmp		word ptr [bp+0eH],0
	jne		L$34
	test		di,di
	jne		L$34
	and		byte ptr [bp+22H],0efH
L$34:
	test		byte ptr [bp+23H],4
	je		L$35
	cmp		word ptr [bp+12H],0
	jne		L$35
	cmp		word ptr [bp+10H],0
	jne		L$35
	cmp		word ptr [bp+0eH],0
	jne		L$35
	test		di,di
	je		L$37
L$35:
	mov		ax,word ptr [bp+12H]
	mov		bx,word ptr [bp+10H]
	mov		cx,word ptr [bp+0eH]
	mov		dx,di
	lea		si,[bp+16H]
	call		far ptr __U8DR
	mov		bl,dl
	cmp		dl,0aH
	jae		L$36
	xor		dh,dh
	add		dx,30H
	jmp		L$40
L$36:
	test		byte ptr [bp+22H],20H
	je		L$38
	mov		dx,41H
	jmp		L$39
L$37:
	jmp		L$42
L$38:
	mov		dx,61H
L$39:
	xor		bh,bh
	add		dx,bx
	sub		dx,0aH
L$40:
	mov		word ptr [bp-4],dx
	mov		bx,word ptr [bp-2]
	inc		word ptr [bp-2]
	mov		dx,ss
	lea		ax,[bp-24H]
	xor		cx,cx
	call		far ptr __PIA
	mov		bx,ax
	mov		ds,dx
	mov		al,byte ptr [bp-4]
	mov		byte ptr [bx],al
	mov		ax,word ptr [bp+12H]
	mov		bx,word ptr [bp+10H]
	mov		cx,word ptr [bp+0eH]
	mov		dx,di
	lea		si,[bp+16H]
	call		far ptr __U8DQ
	mov		word ptr [bp+12H],ax
	mov		word ptr [bp+10H],bx
	mov		word ptr [bp+0eH],cx
	mov		di,dx
	test		ax,ax
	jne		L$41
	test		bx,bx
	jne		L$41
	test		cx,cx
	jne		L$41
	test		dx,dx
	je		L$42
L$41:
	cmp		word ptr [bp-2],20H
	jae		L$42
	jmp		near ptr L$35
L$42:
	push		word ptr [bp+22H]
	push		word ptr [bp+20H]
	push		word ptr [bp+1eH]
	push		word ptr [bp+16H]
	mov		al,byte ptr [bp+14H]
	xor		ah,ah
	push		ax
	push		word ptr [bp-2]
	lea		dx,[bp-24H]
	push		ss
	push		dx
	push		word ptr [bp+0aH]
	push		word ptr [bp+8]
	mov		bx,word ptr [bp-2aH]
	mov		cx,word ptr [bp-2cH]
	mov		ax,word ptr [bp-26H]
	mov		dx,word ptr [bp-28H]
	call		near ptr _ntoa_format_
	mov		sp,bp
	pop		bp
	pop		di
	pop		si
	ret		1cH
_vsnprintf_:
	push		si
	push		di
	push		bp
	mov		bp,sp
	sub		sp,2eH
	mov		word ptr [bp-2eH],ax
	mov		word ptr [bp-2cH],dx
	mov		di,bx
	mov		word ptr [bp-0aH],cx
	xor		si,si
	test		cx,cx
	jne		L$43
	test		bx,bx
	jne		L$43
	mov		word ptr [bp-2eH],offset _out_null_
	mov		word ptr [bp-2cH],seg _out_null_
L$43:
	jmp		near ptr L$114
L$44:
	mov		word ptr [bp-8],0
	mov		ax,bx
	mov		dx,ds
	mov		bx,1
	xor		cx,cx
	call		far ptr __PIA
L$45:
	mov		word ptr [bp+0aH],ax
	mov		word ptr [bp+0cH],dx
	lds		bx,dword ptr [bp+0aH]
	mov		al,byte ptr [bx]
	mov		byte ptr [bp-6],al
	mov		ax,bx
	mov		dx,ds
	mov		bx,1
	xor		cx,cx
	call		far ptr __PIA
	cmp		byte ptr [bp-6],2bH
	jb		L$46
	jbe		L$49
	cmp		byte ptr [bp-6],30H
	je		L$47
	cmp		byte ptr [bp-6],2dH
	je		L$48
	jmp		L$52
L$46:
	cmp		byte ptr [bp-6],23H
	je		L$51
	cmp		byte ptr [bp-6],20H
	je		L$50
	jmp		L$52
L$47:
	or		byte ptr [bp-8],1
	jmp		L$45
L$48:
	or		byte ptr [bp-8],2
	jmp		L$45
L$49:
	or		byte ptr [bp-8],4
	jmp		L$45
L$50:
	or		byte ptr [bp-8],8
	jmp		L$45
L$51:
	or		byte ptr [bp-8],10H
	jmp		L$45
L$52:
	mov		word ptr [bp-10H],0
	lds		bx,dword ptr [bp+0aH]
	mov		al,byte ptr [bx]
	cmp		al,30H
	jb		L$53
	cmp		al,39H
	ja		L$53
	mov		dx,ss
	lea		ax,[bp+0aH]
	call		near ptr _atoi_
	mov		word ptr [bp-10H],ax
	jmp		L$56
L$53:
	lds		bx,dword ptr [bp+0aH]
	cmp		byte ptr [bx],2aH
	jne		L$56
	mov		bx,word ptr [bp+0eH]
	mov		word ptr [bp-28H],bx
	mov		ds,word ptr [bp+10H]
	mov		ax,bx
	mov		dx,ds
	mov		bx,2
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [bp+0eH],ax
	mov		word ptr [bp+10H],dx
	mov		bx,word ptr [bp-28H]
	mov		ax,word ptr [bx]
	test		ax,ax
	jge		L$54
	or		byte ptr [bp-8],2
	mov		word ptr [bp-10H],ax
	neg		word ptr [bp-10H]
	jmp		L$55
L$54:
	mov		word ptr [bp-10H],ax
L$55:
	mov		ax,word ptr [bp+0aH]
	mov		dx,word ptr [bp+0cH]
	mov		bx,1
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [bp+0aH],ax
	mov		word ptr [bp+0cH],dx
L$56:
	mov		word ptr [bp-12H],0
	lds		bx,dword ptr [bp+0aH]
	cmp		byte ptr [bx],2eH
	jne		L$57
	or		byte ptr [bp-7],4
	mov		ax,bx
	mov		dx,ds
	mov		bx,1
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [bp+0aH],ax
	mov		word ptr [bp+0cH],dx
	mov		ds,dx
	mov		bx,ax
	mov		al,byte ptr [bx]
	cmp		al,30H
	jb		L$58
	cmp		al,39H
	ja		L$58
	mov		dx,ss
	lea		ax,[bp+0aH]
	call		near ptr _atoi_
	mov		word ptr [bp-12H],ax
L$57:
	jmp		L$60
L$58:
	lds		bx,dword ptr [bp+0aH]
	cmp		byte ptr [bx],2aH
	jne		L$60
	mov		bx,word ptr [bp+0eH]
	mov		word ptr [bp-2aH],bx
	mov		ds,word ptr [bp+10H]
	mov		ax,bx
	mov		dx,ds
	mov		bx,2
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [bp+0eH],ax
	mov		word ptr [bp+10H],dx
	mov		bx,word ptr [bp-2aH]
	mov		ax,word ptr [bx]
	test		ax,ax
	jg		L$59
	xor		ax,ax
L$59:
	mov		word ptr [bp-12H],ax
	mov		ax,word ptr [bp+0aH]
	mov		dx,word ptr [bp+0cH]
	mov		bx,1
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [bp+0aH],ax
	mov		word ptr [bp+0cH],dx
L$60:
	lds		bx,dword ptr [bp+0aH]
	mov		al,byte ptr [bx]
	mov		byte ptr [bp-2],al
	mov		ax,bx
	mov		dx,ds
	mov		bx,1
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [bp-1cH],ax
	mov		ds,dx
	mov		ax,word ptr [bp-8]
	or		ah,2
	mov		word ptr [bp-20H],ax
	mov		ax,word ptr [bp+0aH]
	mov		dx,word ptr [bp+0cH]
	mov		bx,2
	xor		cx,cx
	call		far ptr __PIA
	cmp		byte ptr [bp-2],6cH
	jb		L$61
	jbe		L$62
	mov		al,byte ptr [bp-2]
	cmp		al,7aH
	je		L$65
	cmp		al,74H
	je		L$65
	jmp		L$66
L$61:
	cmp		byte ptr [bp-2],6aH
	je		L$65
	cmp		byte ptr [bp-2],68H
	je		L$64
	jmp		L$66
L$62:
	or		byte ptr [bp-7],1
	mov		bx,word ptr [bp-1cH]
	mov		word ptr [bp+0aH],bx
	mov		word ptr [bp+0cH],ds
	cmp		byte ptr [bx],6cH
	jne		L$66
	or		byte ptr [bp-7],2
L$63:
	mov		word ptr [bp+0aH],ax
	mov		word ptr [bp+0cH],dx
	jmp		L$66
L$64:
	or		byte ptr [bp-8],80H
	mov		bx,word ptr [bp-1cH]
	mov		word ptr [bp+0aH],bx
	mov		word ptr [bp+0cH],ds
	cmp		byte ptr [bx],68H
	jne		L$66
	or		byte ptr [bp-8],40H
	jmp		L$63
L$65:
	mov		ax,word ptr [bp-20H]
	mov		word ptr [bp-8],ax
	mov		bx,word ptr [bp-1cH]
	mov		word ptr [bp+0aH],bx
	mov		word ptr [bp+0cH],ds
L$66:
	lds		bx,dword ptr [bp+0aH]
	mov		al,byte ptr [bx]
	mov		byte ptr [bp-4],al
	cmp		al,69H
	jb		L$69
	jbe		L$71
	mov		ax,word ptr [bp+0eH]
	mov		dx,word ptr [bp+10H]
	mov		bx,4
	xor		cx,cx
	call		far ptr __PIA
	cmp		byte ptr [bp-4],73H
	jb		L$68
	jbe		L$74
	mov		al,byte ptr [bp-4]
	cmp		al,78H
	je		L$71
	cmp		al,75H
L$67:
	je		L$71
	jmp		near ptr L$138
L$68:
	cmp		byte ptr [bp-4],70H
	je		L$76
	cmp		byte ptr [bp-4],6fH
	jmp		L$67
L$69:
	cmp		al,62H
	jb		L$70
	jbe		L$71
	cmp		al,64H
	je		L$71
	cmp		al,63H
	je		L$77
	jmp		near ptr L$138
L$70:
	cmp		al,58H
	je		L$71
	cmp		al,25H
	je		L$78
	jmp		near ptr L$138
L$71:
	lds		bx,dword ptr [bp+0aH]
	mov		al,byte ptr [bx]
	cmp		al,78H
	je		L$72
	cmp		al,58H
	jne		L$73
L$72:
	mov		word ptr [bp-1aH],10H
	jmp		L$80
L$73:
	cmp		al,6fH
	jne		L$75
	mov		word ptr [bp-1aH],8
	jmp		L$80
L$74:
	jmp		near ptr L$124
L$75:
	cmp		al,62H
	jne		L$79
	mov		word ptr [bp-1aH],2
	jmp		L$80
L$76:
	jmp		near ptr L$136
L$77:
	jmp		near ptr L$118
L$78:
	jmp		near ptr L$137
L$79:
	mov		word ptr [bp-1aH],0aH
	and		byte ptr [bp-8],0efH
L$80:
	lds		bx,dword ptr [bp+0aH]
	cmp		byte ptr [bx],58H
	jne		L$81
	or		byte ptr [bp-8],20H
L$81:
	lds		bx,dword ptr [bp+0aH]
	mov		al,byte ptr [bx]
	cmp		al,69H
	je		L$82
	cmp		al,64H
	je		L$82
	and		byte ptr [bp-8],0f3H
L$82:
	test		byte ptr [bp-7],4
	je		L$83
	and		byte ptr [bp-8],0feH
L$83:
	lds		bx,dword ptr [bp+0aH]
	mov		al,byte ptr [bx]
	cmp		al,69H
	je		L$84
	cmp		al,64H
	jne		L$86
L$84:
	mov		al,byte ptr [bp-7]
	test		al,2
	je		L$87
	mov		bx,word ptr [bp+0eH]
	mov		word ptr [bp-28H],bx
	mov		ds,word ptr [bp+10H]
	mov		ax,bx
	mov		dx,ds
	mov		bx,8
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [bp+0eH],ax
	mov		word ptr [bp+10H],dx
	mov		bx,word ptr [bp-28H]
	mov		dx,word ptr [bx+6]
	mov		ax,word ptr [bx+4]
	mov		word ptr [bp-1eH],ax
	mov		ax,word ptr [bx+2]
	mov		cx,word ptr [bx]
	push		word ptr [bp-8]
	push		word ptr [bp-10H]
	push		word ptr [bp-12H]
	xor		bx,bx
	push		bx
	push		bx
	push		bx
	push		word ptr [bp-1aH]
	test		dx,dx
	jl		L$85
	jne		L$88
	cmp		word ptr [bp-1eH],0
	jne		L$88
	test		ax,ax
	jmp		L$88
L$85:
	mov		bl,1
	jmp		L$89
L$86:
	jmp		near ptr L$106
L$87:
	jmp		L$94
L$88:
	xor		bl,bl
L$89:
	mov		byte ptr [bp-28H],bl
	mov		byte ptr [bp-27H],0
	push		word ptr [bp-28H]
	test		dx,dx
	jg		L$90
	jne		L$91
	cmp		word ptr [bp-1eH],0
	ja		L$90
	jne		L$91
	test		ax,ax
	ja		L$90
	jne		L$91
	test		cx,cx
	jbe		L$91
L$90:
	mov		bx,word ptr [bp-1eH]
	mov		word ptr [bp-22H],cx
	jmp		L$92
L$91:
	xor		bx,bx
	sub		bx,cx
	mov		word ptr [bp-22H],bx
	mov		bx,0
	sbb		bx,ax
	mov		ax,bx
	mov		bx,0
	sbb		bx,word ptr [bp-1eH]
	mov		cx,0
	sbb		cx,dx
	mov		dx,cx
L$92:
	push		dx
	push		bx
	push		ax
	push		word ptr [bp-22H]
L$93:
	push		word ptr [bp+8]
	push		si
	mov		bx,di
	mov		cx,word ptr [bp-0aH]
	mov		ax,word ptr [bp-2eH]
	mov		dx,word ptr [bp-2cH]
	call		near ptr _ntoa_long_long_
	jmp		near ptr L$112
L$94:
	test		al,1
	je		L$100
	mov		bx,word ptr [bp+0eH]
	mov		word ptr [bp-2aH],bx
	mov		ds,word ptr [bp+10H]
	mov		ax,bx
	mov		dx,ds
	mov		bx,4
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [bp+0eH],ax
	mov		word ptr [bp+10H],dx
	mov		bx,word ptr [bp-2aH]
	mov		dx,word ptr [bx]
	mov		ax,word ptr [bx+2]
	push		word ptr [bp-8]
	push		word ptr [bp-10H]
	push		word ptr [bp-12H]
	xor		bx,bx
	push		bx
	push		word ptr [bp-1aH]
	test		ax,ax
	jge		L$95
	mov		bl,1
	jmp		L$96
L$95:
	xor		bl,bl
L$96:
	xor		bh,bh
	push		bx
	test		ax,ax
	jg		L$98
	jne		L$97
	test		dx,dx
	ja		L$98
L$97:
	neg		ax
	neg		dx
	sbb		ax,0
L$98:
	push		ax
	push		dx
L$99:
	push		word ptr [bp+8]
	push		si
	mov		bx,di
	mov		cx,word ptr [bp-0aH]
	mov		ax,word ptr [bp-2eH]
	mov		dx,word ptr [bp-2cH]
	call		near ptr _ntoa_long_
	jmp		near ptr L$112
L$100:
	mov		ax,word ptr [bp+0eH]
	mov		dx,word ptr [bp+10H]
	mov		bx,2
	xor		cx,cx
	call		far ptr __PIA
	test		byte ptr [bp-8],40H
	je		L$101
	lds		bx,dword ptr [bp+0eH]
	mov		word ptr [bp+0eH],ax
	mov		word ptr [bp+10H],dx
	mov		al,byte ptr [bx]
	xor		ah,ah
	jmp		L$102
L$101:
	test		byte ptr [bp-8],80H
	lds		bx,dword ptr [bp+0eH]
	mov		word ptr [bp+0eH],ax
	mov		word ptr [bp+10H],dx
	mov		ax,word ptr [bx]
L$102:
	mov		dx,ax
	push		word ptr [bp-8]
	push		word ptr [bp-10H]
	push		word ptr [bp-12H]
	xor		bx,bx
	push		bx
	push		word ptr [bp-1aH]
	test		ax,ax
	jge		L$103
	mov		al,1
	jmp		L$104
L$103:
	xor		al,al
L$104:
	xor		ah,ah
	push		ax
	test		dx,dx
	jg		L$105
	neg		dx
L$105:
	xor		ax,ax
	jmp		L$98
L$106:
	mov		al,byte ptr [bp-7]
	test		al,2
	je		L$107
	push		word ptr [bp-8]
	push		word ptr [bp-10H]
	push		word ptr [bp-12H]
	xor		ax,ax
	push		ax
	push		ax
	push		ax
	push		word ptr [bp-1aH]
	push		ax
	mov		bx,word ptr [bp+0eH]
	mov		word ptr [bp-2aH],bx
	mov		ds,word ptr [bp+10H]
	mov		ax,bx
	mov		dx,ds
	mov		bx,8
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [bp+0eH],ax
	mov		word ptr [bp+10H],dx
	mov		bx,word ptr [bp-2aH]
	push		word ptr [bx+6]
	push		word ptr [bx+4]
	push		word ptr [bx+2]
	push		word ptr [bx]
	jmp		near ptr L$93
L$107:
	test		al,1
	je		L$108
	push		word ptr [bp-8]
	push		word ptr [bp-10H]
	push		word ptr [bp-12H]
	xor		ax,ax
	push		ax
	push		word ptr [bp-1aH]
	push		ax
	mov		bx,word ptr [bp+0eH]
	mov		word ptr [bp-2aH],bx
	mov		ds,word ptr [bp+10H]
	mov		ax,bx
	mov		dx,ds
	mov		bx,4
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [bp+0eH],ax
	mov		word ptr [bp+10H],dx
	mov		bx,word ptr [bp-2aH]
	push		word ptr [bx+2]
	push		word ptr [bx]
	jmp		near ptr L$99
L$108:
	mov		ax,word ptr [bp+0eH]
	mov		dx,word ptr [bp+10H]
	mov		bx,2
	xor		cx,cx
	call		far ptr __PIA
	test		byte ptr [bp-8],40H
	je		L$109
	lds		bx,dword ptr [bp+0eH]
	mov		word ptr [bp+0eH],ax
	mov		word ptr [bp+10H],dx
	mov		al,byte ptr [bx]
	xor		ah,ah
	jmp		L$110
L$109:
	test		byte ptr [bp-8],80H
	lds		bx,dword ptr [bp+0eH]
	mov		word ptr [bp+0eH],ax
	mov		word ptr [bp+10H],dx
	mov		ax,word ptr [bx]
L$110:
	push		word ptr [bp-8]
	push		word ptr [bp-10H]
	push		word ptr [bp-12H]
	xor		dx,dx
	push		dx
	push		word ptr [bp-1aH]
	push		dx
L$111:
	push		dx
	push		ax
	push		word ptr [bp+8]
	push		si
	mov		bx,di
	mov		cx,word ptr [bp-0aH]
	mov		ax,word ptr [bp-2eH]
	mov		dx,word ptr [bp-2cH]
	call		near ptr _ntoa_long_
L$112:
	mov		si,ax
L$113:
	mov		ax,word ptr [bp+0aH]
	mov		dx,word ptr [bp+0cH]
	mov		bx,1
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [bp+0aH],ax
	mov		word ptr [bp+0cH],dx
L$114:
	lds		bx,dword ptr [bp+0aH]
	mov		al,byte ptr [bx]
	test		al,al
	je		L$120
	cmp		al,25H
	jne		L$115
	jmp		near ptr L$44
L$115:
	push		word ptr [bp+8]
	mov		dx,si
L$116:
	xor		ah,ah
	inc		si
	mov		bx,di
	mov		cx,word ptr [bp-0aH]
L$117:
	call		dword ptr [bp-2eH]
	jmp		L$113
L$118:
	mov		word ptr [bp-0eH],1
	test		byte ptr [bp-8],2
	jne		L$121
L$119:
	mov		ax,word ptr [bp-0eH]
	inc		word ptr [bp-0eH]
	cmp		ax,word ptr [bp-10H]
	jae		L$121
	push		word ptr [bp+8]
	mov		dx,si
	inc		si
	mov		bx,di
	mov		cx,word ptr [bp-0aH]
	mov		ax,20H
	call		dword ptr [bp-2eH]
	jmp		L$119
L$120:
	jmp		near ptr L$139
L$121:
	push		word ptr [bp+8]
	mov		word ptr [bp-28H],si
	inc		si
	mov		bx,word ptr [bp+0eH]
	mov		word ptr [bp-2aH],bx
	mov		ds,word ptr [bp+10H]
	mov		ax,bx
	mov		dx,ds
	mov		bx,2
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [bp+0eH],ax
	mov		word ptr [bp+10H],dx
	mov		bx,word ptr [bp-2aH]
	mov		al,byte ptr [bx]
	xor		ah,ah
	mov		dx,word ptr [bp-28H]
	mov		bx,di
	mov		cx,word ptr [bp-0aH]
	call		dword ptr [bp-2eH]
	test		byte ptr [bp-8],2
	jne		L$123
L$122:
	jmp		near ptr L$113
L$123:
	mov		ax,word ptr [bp-0eH]
	inc		word ptr [bp-0eH]
	cmp		ax,word ptr [bp-10H]
	jae		L$122
	push		word ptr [bp+8]
	mov		dx,si
	inc		si
	mov		bx,di
	mov		cx,word ptr [bp-0aH]
	mov		ax,20H
	call		dword ptr [bp-2eH]
	jmp		L$123
L$124:
	lds		bx,dword ptr [bp+0eH]
	mov		word ptr [bp+0eH],ax
	mov		word ptr [bp+10H],dx
	mov		ax,word ptr [bx]
	mov		word ptr [bp-16H],ax
	mov		ax,word ptr [bx+2]
	mov		word ptr [bp-14H],ax
	mov		ax,word ptr [bp-12H]
	test		ax,ax
	jne		L$125
	mov		ax,0ffffH
L$125:
	mov		bx,word ptr [bp-16H]
	mov		word ptr [bp-26H],bx
	mov		dx,word ptr [bp-14H]
	mov		word ptr [bp-24H],dx
	mov		word ptr [bp-18H],ax
	mov		ax,bx
L$126:
	mov		ds,dx
	mov		bx,ax
	cmp		byte ptr [bx],0
	je		L$127
	dec		word ptr [bp-18H]
	cmp		word ptr [bp-18H],0ffffH
	je		L$127
	mov		dx,ds
	mov		bx,1
	xor		cx,cx
	call		far ptr __PIA
	jmp		L$126
L$127:
	mov		dx,ds
	mov		bx,word ptr [bp-26H]
	mov		cx,word ptr [bp-24H]
	call		far ptr __PTS
	mov		word ptr [bp-0cH],ax
	test		byte ptr [bp-7],4
	je		L$129
	cmp		ax,word ptr [bp-12H]
	jb		L$128
	mov		ax,word ptr [bp-12H]
L$128:
	mov		word ptr [bp-0cH],ax
L$129:
	test		byte ptr [bp-8],2
	jne		L$131
L$130:
	mov		ax,word ptr [bp-0cH]
	inc		word ptr [bp-0cH]
	cmp		ax,word ptr [bp-10H]
	jae		L$131
	push		word ptr [bp+8]
	mov		dx,si
	inc		si
	mov		bx,di
	mov		cx,word ptr [bp-0aH]
	mov		ax,20H
	call		dword ptr [bp-2eH]
	jmp		L$130
L$131:
	lds		bx,dword ptr [bp-16H]
	cmp		byte ptr [bx],0
	je		L$133
	test		byte ptr [bp-7],4
	je		L$132
	dec		word ptr [bp-12H]
	cmp		word ptr [bp-12H],0ffffH
	je		L$133
L$132:
	push		word ptr [bp+8]
	mov		word ptr [bp-2aH],si
	lds		bx,dword ptr [bp-16H]
	mov		al,byte ptr [bx]
	mov		byte ptr [bp-28H],al
	mov		byte ptr [bp-27H],0
	inc		si
	mov		ax,bx
	mov		dx,ds
	mov		bx,1
	xor		cx,cx
	call		far ptr __PIA
	mov		word ptr [bp-16H],ax
	mov		word ptr [bp-14H],dx
	mov		dx,word ptr [bp-2aH]
	mov		bx,di
	mov		cx,word ptr [bp-0aH]
	mov		ax,word ptr [bp-28H]
	call		dword ptr [bp-2eH]
	jmp		L$131
L$133:
	test		byte ptr [bp-8],2
	jne		L$135
L$134:
	jmp		near ptr L$113
L$135:
	mov		ax,word ptr [bp-0cH]
	inc		word ptr [bp-0cH]
	cmp		ax,word ptr [bp-10H]
	jae		L$134
	push		word ptr [bp+8]
	mov		dx,si
	inc		si
	mov		bx,di
	mov		cx,word ptr [bp-0aH]
	mov		ax,20H
	call		dword ptr [bp-2eH]
	jmp		L$135
L$136:
	or		byte ptr [bp-8],21H
	push		word ptr [bp-8]
	mov		bx,8
	push		bx
	push		word ptr [bp-12H]
	xor		bx,bx
	push		bx
	mov		bx,10H
	push		bx
	xor		bx,bx
	push		bx
	lds		bx,dword ptr [bp+0eH]
	mov		word ptr [bp+0eH],ax
	mov		word ptr [bp+10H],dx
	mov		ax,word ptr [bx]
	mov		dx,word ptr [bx+2]
	jmp		near ptr L$111
L$137:
	push		word ptr [bp+8]
	mov		dx,si
	inc		si
	mov		bx,di
	mov		cx,word ptr [bp-0aH]
	mov		ax,25H
	jmp		near ptr L$117
L$138:
	push		word ptr [bp+8]
	mov		dx,si
	lds		bx,dword ptr [bp+0aH]
	mov		al,byte ptr [bx]
	jmp		near ptr L$116
L$139:
	push		word ptr [bp+8]
	mov		ax,word ptr [bp+8]
	cmp		si,ax
	jae		L$140
	mov		ax,si
	jmp		L$141
L$140:
	dec		ax
L$141:
	mov		dx,ax
	mov		bx,di
	mov		cx,word ptr [bp-0aH]
	xor		ax,ax
	call		dword ptr [bp-2eH]
	mov		ax,si
	mov		sp,bp
	pop		bp
	pop		di
	pop		si
	ret		0aH
printf__:
	push		bx
	push		cx
	push		dx
	push		bp
	mov		bp,sp
	sub		sp,2
	mov		dx,ss
	lea		ax,[bp+0cH]
	mov		bx,4
	xor		cx,cx
	call		far ptr __PIA
	push		dx
	push		ax
	push		word ptr [bp+0eH]
	push		word ptr [bp+0cH]
	mov		ax,0ffffH
	push		ax
	mov		cx,ss
	lea		bx,[bp-2]
	mov		ax,offset _out_char_
	mov		dx,seg _out_char_
	call		near ptr _vsnprintf_
	mov		sp,bp
	jmp		L$143
sprintf__:
	push		bx
	push		cx
	push		dx
	push		bp
	mov		bp,sp
	mov		dx,ss
	lea		ax,[bp+10H]
	mov		bx,4
	xor		cx,cx
	call		far ptr __PIA
	push		dx
	push		ax
	push		word ptr [bp+12H]
	push		word ptr [bp+10H]
	mov		ax,0ffffH
	push		ax
L$142:
	mov		bx,word ptr [bp+0cH]
	mov		cx,word ptr [bp+0eH]
	mov		ax,offset _out_buffer_
	mov		dx,seg _out_buffer_
	call		near ptr _vsnprintf_
L$143:
	pop		bp
	pop		dx
	pop		cx
	pop		bx
	retf
snprintf__:
	push		bx
	push		cx
	push		dx
	push		bp
	mov		bp,sp
	mov		dx,ss
	lea		ax,[bp+12H]
	mov		bx,4
	xor		cx,cx
	call		far ptr __PIA
	push		dx
	push		ax
	push		word ptr [bp+14H]
	push		word ptr [bp+12H]
	push		word ptr [bp+10H]
	jmp		L$142
vprintf__:
	push		bp
	mov		bp,sp
	sub		sp,2
	push		cx
	push		bx
	push		dx
	push		ax
	mov		ax,0ffffH
	push		ax
	mov		cx,ss
	lea		bx,[bp-2]
	mov		ax,offset _out_char_
	mov		dx,seg _out_char_
	call		near ptr _vsnprintf_
	mov		sp,bp
	pop		bp
	retf
vsnprintf__:
	push		cx
	push		bp
	mov		bp,sp
	push		word ptr [bp+0eH]
	push		word ptr [bp+0cH]
	push		word ptr [bp+0aH]
	push		word ptr [bp+8]
	push		bx
	mov		bx,ax
	mov		cx,dx
	mov		ax,offset _out_buffer_
	mov		dx,seg _out_buffer_
	call		near ptr _vsnprintf_
	pop		bp
	pop		cx
	retf		8
printf_TEXT		ENDS
CONST		SEGMENT	WORD PUBLIC USE16 'DATA'
CONST		ENDS
CONST2		SEGMENT	WORD PUBLIC USE16 'DATA'
CONST2		ENDS
_DATA		SEGMENT	WORD PUBLIC USE16 'DATA'
_DATA		ENDS
		END
