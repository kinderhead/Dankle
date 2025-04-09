#include <string.h>
#include <stdio.h>
#include <dankle.h>

void main()
{
    const char* a = "Test1";
    const char* b = "Test ";

    int res = strcmp(a, b);
    printf("Diff: %d\n", res);
}
