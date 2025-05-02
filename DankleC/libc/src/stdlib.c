#include <stdlib.h>
#include <stdio.h>
#include <dankle.h>

int atoi(char* s)
{
    int acum = 0;
    int factor = 1;

    if (*s == '-')
    {
        factor = -1;
        s++;
    }

    while ((*s >= '0') && (*s <= '9'))
    {
        acum = acum * 10;
        acum = acum + (*s - 48);
        s++;
    }
    return (factor * acum);
}

// Yoinked from https://github.com/RAGUL1902/Dynamic-Memory-Allocation-in-C

#define META_BLOCK_SIZE 20
#define align4(x) ((((x-1) >> 2) << 2) + 4)

typedef struct meta_block
{
    int free;
    size_t size;
    void* next;
    void* prev;
    void* ptr;
    char data[1];
} *meta_ptr;

void* base = NULL;
int heap = 0xFF;

meta_ptr find_suitable_block(meta_ptr* last, size_t size)
{
    meta_ptr b = base;
    while (b && !(b->free && b->size >= size))
    {
        *last = b;
        //printf("Block ref: %d\n", last);
        b = b->next;
    }
    return *last;
}

void split_space(meta_ptr block, size_t size)
{
    meta_ptr new_block;
    new_block = block->data + size;
    new_block->size = block->size - size - META_BLOCK_SIZE;
    new_block->next = block->next;
    new_block->free = 1;
    new_block->ptr = new_block->data;
    new_block->prev = block;
    block->next = new_block;
    block->size = size;
    if (new_block->next)
    {
        ((meta_ptr) (new_block->next))->prev = new_block;
    }
}

meta_ptr extend_heap(meta_ptr last, size_t size)
{
    meta_ptr old_break, new_break;
    old_break = heap;
    new_break = heap += (META_BLOCK_SIZE + size);
    if (new_break > 0xFFFF)
    {
        return NULL;
    }
    old_break->size = size;
    old_break->free = 0;
    old_break->next = NULL;
    old_break->prev = NULL;
    old_break->ptr = old_break->data;
    if (last)
    {
        last->next = old_break;
    }
    return (old_break);
}

meta_ptr merge_blocks(meta_ptr block)
{
    if (block->next && ((meta_ptr) (block->next))->free)
    {
        block->size += META_BLOCK_SIZE + ((meta_ptr) (block->next))->size;
        block->next = ((meta_ptr) (block->next))->next;
    }
    if (block->next)
    {
        ((meta_ptr) (block->next))->prev = block;
    }
    return block;
}

meta_ptr get_block_addr(void* p)
{
    char* tmp = p;
    tmp = tmp - META_BLOCK_SIZE;
    p = tmp;
    return (p);
}

int is_addr_valid(void* p)
{
    if (base)
    {
        if (p > base && p < heap)
        {
            return (p == get_block_addr(p)->ptr);
        }
    }
    return 0;
}

void* malloc(size_t size)
{
    meta_ptr block, last;
    size_t s = align4(size);
    if (base)
    {
        last = base;
        block = find_suitable_block(&last, s);
        if (block->free)
        {
            if (block->size - s >= (META_BLOCK_SIZE + 4))
            {
                split_space(block, s);
            }
            block->free = 0;
        }
        else
        {
            block = extend_heap(last, s);
            if (!block)
            {
                return NULL;
            }
        }
    }
    else
    {
        block = extend_heap(NULL, s);
        if (!block)
        {
            return NULL;
        }
        base = block;
    }
    return block->data;
}

void free(void* ptr)
{
    if (is_addr_valid(ptr))
    {
        meta_ptr block = get_block_addr(ptr);
        block->free = 1;
        if (block->prev && ((meta_ptr) (block->prev))->free)
        {
            block = merge_blocks(block->prev);
        }

        if (block->next)
        {
            block = merge_blocks(block);
        }
        else
        {
            if (block->prev)
            {
                ((meta_ptr) (block->prev))->next = NULL;
            }
            else
            {
                base = NULL;
            }

            heap = base;
        }
    }
}
