int get(int y)
{
    int x = 3;
    x += y;

    return x;
}

const char* funny()
{
	return "FODSHOFIH";
}

int main()
{
    int y;
    int x = get(6);

    if (x > 5)
    {
        y = 6;
    }
    else
    {
        y = 3;
    }

    return y;
}
