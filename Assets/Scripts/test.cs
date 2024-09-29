using Board;
using NaughtyAttributes;
using UnityEngine;

public class test : MonoBehaviour
{
    GameBoard board;
    void Start()
    {
        board = new GameBoard(3);
        BoardManager.instance.DrawBoard(board);
    }

    [Button]
    void Test2()
    {
        string[] result = board.GetConnectedSlots("3,5");
        foreach (string slot in result) {
            Debug.Log(slot);
        }
    }
}