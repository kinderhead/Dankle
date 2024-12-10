main:
    ld r0, text
write:
    ldb r1, [r0]
    cmp r1, 0
    je end
    stb [0xFFFFFFF0], r1
    inc r0
    jmp write
end:
    hlt

text: "Hello World"

