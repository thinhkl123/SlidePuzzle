using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LazeDetail
{ 
    public Vector2Int Position;
    public Direction Direction;
}

[Serializable]
public class LazeLevel
{
    public int LazeID;
    public List<LazeDetail> LazeList;
}

[CreateAssetMenu(fileName = "LazeData", menuName = "LazeSO")]
public class LazeSO : ScriptableObject
{
    public Vector2Int PosPrefab;
    public List<LazeLevel> ListLazeLevel;
}
