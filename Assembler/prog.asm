main:
    ld r0, text
    ldb r1, [r0]
write:
    inc r0
    stb [0xFFFFFFF0], r1
    ldb r1, [r0]
    cmp r1, 0
    jne write
    hlt

text: "Hello World"

