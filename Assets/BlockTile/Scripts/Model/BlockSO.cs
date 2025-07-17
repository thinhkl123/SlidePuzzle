using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "BlockSO", fileName = "BlockData")]
public class BlockSO : ScriptableObject
{
    public List<BlockDetail> BlockDetails;
}

[Serializable]
public class BlockDetail
{
    public int BlockID;
    public List<Block> Blocks;
}

[Serializable]
public class Block
{
    public int numOfBlocks;
    public List<Vector2Int> BlockPosList;
    public List<Vector2Int> groundPosList;
    public List<KeyType> keyTypeList;
    public List<TileBase> groundTileList;
}
