using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PuzzleSort 
{
    public Vector2Int StartGroundPos;
    public Vector2Int EndGroundPos;
    public Vector2Int StartPuzzlePos;
    public Vector2Int PortStartPos;
    public Vector2Int PortEndPos;
    public int ValuePlayer;
}

[Serializable]
public class PuzzleSortShuffle
{
    public List<int> ShuffleList;
}

[Serializable]
public class PuzzleSortDetails
{
    public int PuzzleSortID;
    public PuzzleSort PuzzleSort;
    public PuzzleSortShuffle PuzzleShuffle;
}

[CreateAssetMenu(fileName = "PuzzleSortLevelData", menuName = "PuzzleSortLevelSO")]
public class PuzzleSortLevelSO : ScriptableObject
{
    public List<PuzzleSortDetails> PuzzleSortDataList;
}
