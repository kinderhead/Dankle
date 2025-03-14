struct vec2i
{
    int x, y;
};

struct vec2i test()
{
    struct vec2i t;
    t.x = 342;
    t.y = -1342;
    return t;
}

short main()
{
    int y = test().y;

    return 0;
}
