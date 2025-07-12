using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PuzzleSort 
{
    public int PuzzleSortID;
    public Vector2Int StartGroundPos;
    public Vector2Int EndGroundPos;
    public Vector2Int StartPuzzlePos;
    public Vector2Int PlayerStartPos;
    public Vector2Int PortStartPos;
    public Vector2Int PortEndPos;
}

[CreateAssetMenu(fileName = "PuzzleSortLevelData", menuName = "PuzzleSortLevelSO")]
public class PuzzleSortLevelSO : ScriptableObject
{
    public List<PuzzleSort> ListPuzzleSortData;
}
