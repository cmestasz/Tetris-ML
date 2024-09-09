using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private BoardController[] boards;
    [SerializeField] private bool allowSelfTarget;
    private int totalBoards;
    private int bagSeed;

    void Start()
    {
        totalBoards = boards.Length;
        bagSeed = Random.Range(0, 1000000);
        for (int i = 0; i < boards.Length; i++)
        {
            boards[i].Init(i, bagSeed);
        }
        boards[0].allowInput = true;
    }

    public void SendAttack(int attack, int targetId)
    {
        StartCoroutine(boards[targetId].ReceiveAttack(attack));
    }

    public int GetTargetId(int id)
    {
        List<int> possibleTargets = new List<int>();
        for (int i = 0; i < totalBoards; i++)
        {
            if (allowSelfTarget || i != id)
                possibleTargets.Add(i);
        }
        return possibleTargets[Random.Range(0, possibleTargets.Count)];
    }
}