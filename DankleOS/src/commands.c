#include "commands.h"

static void about()
{
    printf("______  ___   _   _  _   __ _      _____\n|  _  \\/ _ \\ | \\ | || | / /| |    |  ___|\n| | | / /_\\ \\|  \\| || |/ / | |    | |\n| | | |  _  || . ` ||    \\ | |    |  __|\n| |/ /| | | || |\\  || |\\  \\| |____| |___\n|___/ \\_| |_/\\_| \\_/\\_| \\_/\\_____/\\____/\n\n");
    printf("Dankle OS command line");
}

static void echo(const char* args)
{
    printf(args);
    WRITE_CHAR('\n');
}

void run_cmd(const char* cmd, const char* args)
{
    printf("Invalid command \"%s\"", cmd);

    if (streq(cmd, "about")) about();
    else if (streq(cmd, "echo")) echo(args);
}
