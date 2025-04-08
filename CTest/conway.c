#include <stdio.h>

// Written by a friend. Compiles and runs with default settings.

#define grid_size 25
#define grid_area 729 // (size + 2) * (size + 2)
#define print_size 651 // size * (size + 1) + 2

int get_index(int x, int y)
{
    return (y + 1) * grid_size + (x + 1);
}

void step(int* grid)
{
    static int new_grid[grid_area];
    for (int i = 0; i < (grid_size + 2) * (grid_size + 2); i++)
    {
        new_grid[i] = 0;
    }
    for (int x = 0; x < grid_size; x++)
        for (int y = 0; y < grid_size; y++)
        {
            int index = get_index(x, y);
            int state = grid[index];
            int count = 0;
            for (int ix = x - 1; ix <= x + 1; ix++)
                for (int iy = y - 1; iy <= y + 1; iy++)
                {
                    if (ix == x && iy == y)
                        continue;
                    if (grid[get_index(ix, iy)] != 0)
                        count++;
                }
            int new_state = 0;
            if (state == 1)
            {
                if (count < 2 || count > 3)
                    new_state = 0;
                else
                    new_state = 1;
            }
            else if (state == 0)
            {
                if (count == 3)
                    new_state = 1;
                else
                    new_state = 0;
            }
            new_grid[index] = new_state;
        }
    for (int i = 0; i < (grid_size + 2) * (grid_size + 2); i++)
    {
        grid[i] = new_grid[i];
    }
}

void print_grid(int* grid)
{
    static char p_grid[print_size];
    p_grid[print_size - 1] = '\0';
    int i = 1;
    p_grid[0] = '\n';
    for (int x = 0; x < grid_size; x++)
    {
        for (int y = 0; y < grid_size; y++)
        {
            int index = get_index(x, y);
            if (grid[index] == 1)
            {
                p_grid[i] = '#';
                i++;
            }
            else
            {
                p_grid[i] = '_';
                i++;
            }
        }
        p_grid[i] = '\n';
        i++;
    }
    printf(p_grid);
}

int main()
{
    int grid[grid_area];
    for (int i = 0; i < (grid_size + 2) * (grid_size + 2); i++)
    {
        grid[i] = 0;
    }
    grid[get_index(10, 10)] = 1;
    grid[get_index(10, 11)] = 1;
    grid[get_index(10, 12)] = 1;
    grid[get_index(9, 11)] = 1;
    grid[get_index(11, 12)] = 1;
    print_grid(grid);
    while (1)
    {
        step(grid);
        print_grid(grid);
        // int i = 0;
        // while (1)
        // {
        //     ++i;
        //     if (i > 10000000)
        //         break;
        // }
    }
    return 1;
}