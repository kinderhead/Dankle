main:
    ld r13, 0xFFFF
    ld r0, text#H
    ld r1, text#L
    call print
    hlt
print:
    ldb r2, [r0,r1]
write:
    inc r1
    stb [0xFFFFFFF0], r2
    ldb r2, [r0,r1]
    cmp r2, 0
    jne write
    ret
text:
    "Hello World"