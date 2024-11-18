main:
    ld r0, text
    ld r1, 0
test:
    inc r1
    call write
    cmp r1, 5
    jne test

    hlt

text: "Gaming\n"