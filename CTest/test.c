struct vec2i
{
    int x, y;
};

void combine(struct vec2i vec)
{
    int z = vec.x + vec.y;
}

short main()
{
    struct vec2i vec;

    vec.x = 4;
    vec.y = -3;

    combine(vec);

    return 0;
}
