using System.Collections;
using System.Collections.Generic;

// just because queue.peekat is not a thing
public class PeekableQueue<T> : IEnumerable<T>
{
    private T[] queue;
    private int start;
    private int end;
    private int count;

    public PeekableQueue(int size)
    {
        queue = new T[size];
        start = 0;
        end = 0;
        count = 0;
    }

    public void Enqueue(T item)
    {
        if (count == queue.Length)
        {
            Grow();
        }
        queue[end] = item;
        end = (end + 1) % queue.Length;
    }

    public T Dequeue()
    {
        T item = queue[start];
        start = (start + 1) % queue.Length;
        count--;
        return item;
    }

    public T Peek()
    {
        return queue[start];
    }

    public T PeekAt(int idx)
    {
        return queue[(start + idx) % queue.Length];
    }

    private void Grow()
    {
        T[] newQueue = new T[queue.Length * 2];
        for (int i = 0; i < count; i++)
        {
            newQueue[i] = queue[(start + i) % queue.Length];
        }
        queue = newQueue;
        start = 0;
        end = count;
    }

    public int Count()
    {
        return count;
    }

    public void Clear()
    {
        start = 0;
        end = 0;
        count = 0;
    }

    public bool IsEmpty()
    {
        return count == 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < count; i++)
        {
            yield return queue[(start + i) % queue.Length];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}