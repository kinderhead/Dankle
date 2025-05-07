#include "commands.h"
#include <stdlib.h>

char prompt[32];

// Must always have a / at the end
char cwd[128];

void main()
{
    printf("Dankle OS\n\n");

    cwd[0] = '/';
    cwd[1] = 0;

    while (true)
    {
        char cmdbuf[512];
        printf("%s%s> ", prompt, cwd);

        readline(cmdbuf, 128);

        if (cmdbuf[0] == 0) continue;

        const char* args;
        char* cmd = strtok_r(cmdbuf, " ", &args);

        run_cmd(cmd, args);
    }
}
