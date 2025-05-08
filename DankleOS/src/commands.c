#include "commands.h"
#include <stdlib.h>

#define NUM_CMDS 10

typedef struct command_s
{
    void (*cb)(const char*);
    const char* name;
    const char* help;
} command_t;

extern char prompt[32];
extern char cwd[128];

static void about(const char* args)
{
    printf("______  ___   _   _  _   __ _      _____\n|  _  \\/ _ \\ | \\ | || | / /| |    |  ___|\n| | | / /_\\ \\|  \\| || |/ / | |    | |\n| | | |  _  || . ` ||    \\ | |    |  __|\n| |/ /| | | || |\\  || |\\  \\| |____| |___\n|___/ \\_| |_/\\_| \\_/\\_| \\_/\\_____/\\____/\n\n");
    printf("Dankle OS command line. Type \"help\" for more information.\n");
}

static void echo(const char* args)
{
    printf(args);
    WRITE_CHAR('\n');
}

static void set_prompt(const char* args)
{
    if (strlen(args) >= 32)
    {
        printf("Prompt too large\n");
    }
    else
    {
        strcpy(prompt, args);
        //WRITE_CHAR('\n');
    }
}

static void count(const char* num)
{
    int count = atoi(num);

    if (count <= 0)
    {
        //WRITE_CHAR('\n');
        return;
    }

    for (size_t i = 1; i <= count; i++)
    {
        printf("%d\n", i);
    }
}

static void cat(const char* path)
{
    send_cwd();
    if (!fs_open(path, FS_MODE_READ))
    {
        printf("Error opening file %s\n", path);
        return;
    }

    char* mem = malloc(fs_size() + 1);
    int last = fs_read(mem, fs_size());
    mem[last] = 0;

    printf("%s\n", mem);

    free(mem);
}

static void write(const char* args)
{
    const char* path = strtok_r(args, " ", &args);

    send_cwd();
    if (!fs_open(path, FS_MODE_WRITE))
    {
        printf("Error opening file %s\n", path);
        return;
    }

    fs_write(args, strlen(args), true);
}

static void mkdir(const char* path)
{
    send_cwd();
    if (!fs_mkdir(path)) printf("Error creating directory");
}

static void cd(const char* path)
{
    if (strlen(cwd) + strlen(path) >= 126)
    {
        printf("Path too long");
        return;
    }

    fs_writetext(cwd, false);
    fs_compress_path(path, cwd, 128);
}

static void exit_cmd(const char* path)
{
    exit();
}

static void help(const char* args);

command_t cmds[NUM_CMDS] = {
    { about, "about", "Display the about message." },
    { echo, "echo", "Print arguments to the console.\nUsage: echo <message>" },
    { set_prompt, "prompt", "Set the prompt text.\nUsage: prompt <prompt>" },
    { count, "count", "Counting!\nUsage: count <count>" },
    { cat, "cat", "Read file to terminal\nUsage: cat <path>" },
    { write, "write", "Write file\nUsage: write <path> <contents>" },
    { mkdir, "mkdir", "Create directory\nUsage: mkdir <path>" },
    { cd, "cd", "Change directory\nUsage: cd <path>" },
    { exit_cmd, "exit", "Shutdown computer" },
    { help, "help", "Display available commands and query proper usage.\nUsage: help [command]" }
};

static void help(const char* args)
{
    if (args[0] == 0 || args[0] == ' ')
    {
        printf("List of commands:\n\n");
        for (int i = 0; i < NUM_CMDS; i++)
        {
            printf("%s: ", cmds[i].name);
            printuln(cmds[i].help, '\n');
        }
        printf("\nFor more information, type \"help [command]\"\n");
    }
    else
    {
        for (int i = 0; i < NUM_CMDS; i++)
        {
            if (streq(args, cmds[i].name))
            {
                printf("%s\n", cmds[i].help);
                return;
            }
        }

        printf("Invalid command \"%s\"\n", args);
    }
}

void send_cwd()
{
    fs_writetext(cwd, false);
    fs_writetext("/", false);
}

void run_cmd(const char* cmd, const char* args)
{
    for (int i = 0; i < NUM_CMDS; i++)
    {
        if (streq(cmd, cmds[i].name))
        {
            cmds[i].cb(args);
            return;
        }
    }

    printf("Invalid command \"%s\"\n", cmd);
}
