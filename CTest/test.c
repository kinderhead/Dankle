#include <string.h>
#include <stdio.h>
#include <dankle.h>
#include <stdbool.h>

void main()
{
    //printf("______  ___   _   _  _   __ _      _____\n|  _  \\/ _ \\ | \\ | || | / /| |    |  ___|\n| | | / /_\\ \\|  \\| || |/ / | |    | |\n| | | |  _  || . ` ||    \\ | |    |  __|\n| |/ /| | | || |\\  || |\\  \\| |____| |___\n|___/ \\_| |_/\\_| \\_/\\_| \\_/\\_____/\\____/");
    printf("Dankle OS\n");

    while (true)
    {
        char cmdbuf[128];
        printf("\n> ");
        readline(cmdbuf, 128);

        char* cmd = strtok(cmdbuf, " ");

        if (streq(cmd, "69")) printf("Nice");
        else printf("Invalid command \"%s\"", cmd);
    }
}
