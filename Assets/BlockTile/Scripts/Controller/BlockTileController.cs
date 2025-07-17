using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;

public class BlockTileController : SingletonMono<BlockTileController> 
{
    public List<Block> BlockList;

    public void SetBlockList(List<Block> BlockList)
    {
        this.BlockList = new List<Block>(BlockList); 
    }

    public void CheckUnBlock()
    {
        foreach (var block in BlockList)
        {
            Check(block);
        }
    }

    public void Check(Block block)
    {
        bool isComplete = true;

        //foreach (Vector2Int blockPos in block.BlockPosList)
        for (int i = 0; i < block.BlockPosList.Count; i++)
        {
            Vector2Int blockPos = block.BlockPosList[i];

            if (!ItemTileController.Instance.keyPosList.Contains(blockPos))
            {
                isComplete = false; 
                break;
            } 
            else 
            {
                int index = ItemTileController.Instance.keyPosList.IndexOf(blockPos);
                if (ItemTileController.Instance.keyTypeList[index] != block.keyTypeList[i])
                {
                    isComplete = false; 
                    break;
                }
            }
        }

        Debug.Log(isComplete);

        if (!isComplete)
        {
            for (int i = 0; i < block.groundPosList.Count; i++)
            {
                Vector2Int groundPos = block.groundPosList[i];
                Vector3Int groundGridPos = new Vector3Int(groundPos.x, groundPos.y, 0);

                SlideController.Instance.groundTilemap.SetTile(groundGridPos, null);
            }
        }
        else
        {
            for (int i = 0; i < block.groundPosList.Count; i++)
            {
                Vector2Int groundPos = block.groundPosList[i];
                Vector3Int groundGridPos = new Vector3Int(groundPos.x, groundPos.y, 0);

                SlideController.Instance.groundTilemap.SetTile(groundGridPos, block.groundTileList[0]);
            }
        }
    }
}
