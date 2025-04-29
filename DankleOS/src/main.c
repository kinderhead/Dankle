#include "commands.h"

char prompt[32];

typedef struct test_s
{
    int x;
    const char* str;
} test;

test t[2] = { { 5, "bruh" }, { 6, "bruh2" } };

void main()
{
    printf("Dankle OS\n\n");

    printf("%d %s\n", t[0].x, t[0].str);
    printf("%d %s\n", t[1].x, t[1].str);

    while (true)
    {
        char cmdbuf[512];
        printf("%s> ", prompt);

        readline(cmdbuf, 128);

        if (cmdbuf[0] == 0) continue;

        const char* args;
        char* cmd = strtok_r(cmdbuf, " ", &args);

        run_cmd(cmd, args);
    }
}
