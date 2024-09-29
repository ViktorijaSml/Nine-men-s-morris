using Board;
using NaughtyAttributes;
using UnityEngine;

public class test : MonoBehaviour
{
    GameBoard board;
    [Button]
    void Test()
    {
        board = new GameBoard(2);
        BoardManager.instance.DrawBoard(board);
    }

    [Button]
    void Test2()
    {
        string[] result = board.GetConnectedSlots("1,1");
        foreach (string slot in result) {
            Debug.Log(slot);
        }
    }
    [Button]
    void Test3()
    {
        string[] result = board.GetConnectedSlots("1,2");
        foreach (string slot in result)
        {
            Debug.Log(slot);
        }
    }
    [Button]
    void Test4()
    {
        string[] result = board.GetConnectedSlots("1,6");
        foreach (string slot in result)
        {
            Debug.Log(slot);
        }
    }

}