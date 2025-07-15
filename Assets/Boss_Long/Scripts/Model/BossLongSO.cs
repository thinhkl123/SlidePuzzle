using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "BossLongSO", fileName = "BossLongData")]
public class BossLongSO : ScriptableObject
{
    public Vector2Int InitBossPos;
    public int pahseCount;
    public List<int> lengthEachPhase;
    public TileBase headTile;
    public TileBase bodyTile;
    public TileBase tailTile;
    public Sprite headSprite;
    public Sprite bodySprite;
    public Sprite tailSprite;
}
