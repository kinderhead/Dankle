short main()
{
    int _;

    struct test
    {
        short x, y;
        long l;
    } funny;

    struct test
    {
        short x, y;
        long l;
    }*ptr = &funny;

    ptr->x = 34;
    ptr->y = 2314;
    ptr->l = -23474592;

    return 0;
}
