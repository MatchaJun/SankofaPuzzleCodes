using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleList : MonoBehaviour
{
    public List<GameObject> puzzleManagerList = new List<GameObject>();
    [HideInInspector]
    public int index = 0;
    void Start()
    {
        for (int i = 1; i < puzzleManagerList.Count; i++)
        {
            puzzleManagerList[i].gameObject.SetActive(false);
        }
    }

    public void SpawnNextPuzzle()
    {
        index++;
        if (index < puzzleManagerList.Count)
        {
            puzzleManagerList[index].gameObject.SetActive(true);
        }
    }
}
