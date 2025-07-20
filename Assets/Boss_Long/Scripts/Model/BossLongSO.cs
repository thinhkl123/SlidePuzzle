using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "BossLongSO", fileName = "BossLongData")]
public class BossLongSO : ScriptableObject
{
    public Vector2Int InitBossPos;
    public int pahseCount;
    public List<int> lengthEachPhase;
    public List<ObstacleEachPhase> ObstacleEachPhase;
    public TileBase headTile;
    public TileBase bodyTile;
    public TileBase tailTile;
    public TileBase obstacleTile;
    public Sprite headSprite;
    public Sprite bodySprite;
    public Sprite tailSprite;
}

[Serializable]
public class ObstacleEachPhase
{
    public List<Vector2Int> obstaclePos;
}
