using System;
using static Pieces;

public class Bag
{
    private int idx = 7;
    private readonly int[] order = new int[] { 0, 1, 2, 3, 4, 5, 6 };
    private readonly Piece[] bag = new Piece[] { Piece.S, Piece.Z, Piece.L, Piece.J, Piece.I, Piece.O, Piece.T };
    private readonly PeekableQueue<Piece> queue = new(7);
    private Random random;

    public Bag(int seed)
    {
        random = new Random(seed);
        for (int i = 0; i < 7; i++)
        {
            GenerateNext();
        }
    }

    public Piece GetNext()
    {
        Piece piece = queue.Dequeue();
        GenerateNext();
        return piece;
    }

    public Piece PeekAt(int idx)
    {
        return queue.PeekAt(idx);
    }

    private void GenerateNext()
    {
        if (idx == 7)
        {
            Shuffle(order);
            idx = 0;
        }
        Piece piece = bag[order[idx++]];
        queue.Enqueue(piece);
    }

    private void Shuffle<T>(T[] arr)
    {
        for (int i = arr.Length - 1; i >= 0; i--)
        {
            int j = random.Next(i + 1);
            (arr[j], arr[i]) = (arr[i], arr[j]);
        }
    }
}