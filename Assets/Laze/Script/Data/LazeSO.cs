using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LazeDetail
{ 
    public Vector2Int Position;
    public Direction Direction;
    public Vector2Int LockPosition;
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
    public Sprite LightSprite;
    public Sprite LockSprite;
    public List<LazeLevel> ListLazeLevel;
}
