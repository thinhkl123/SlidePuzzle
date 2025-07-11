using System.Collections.Generic;
using UnityEngine;
using CustomUtils;

public class ItemTileController : SingletonMono<ItemTileController>
{
    public List<Vector2Int> weaponPosList;
    public List<Vector2Int> keyPosList;

    public void SetWeaponPosList(List<Vector2Int> weaponPosList)
    {
        this.weaponPosList = new List<Vector2Int>(weaponPosList);
    }

    public void SetKeyPosList(List<Vector2Int> keyPosList)
    {
        this.keyPosList = new List<Vector2Int>(keyPosList);
    }

    public void UpdateItemPosList(Vector2Int oldPos, Vector2Int newPos)
    {
        if (weaponPosList.Contains(oldPos))
        {
            weaponPosList.Remove(oldPos);
            weaponPosList.Add(newPos);

            EnemyNotMoveTileController.Instance.CheckDefeatEnemy(newPos);
        } 
        else if (keyPosList.Contains(oldPos))
        {
            keyPosList.Remove(oldPos);
            keyPosList.Add(newPos);

            BlockTileController.Instance.CheckUnBlock();
        }
    }
}
