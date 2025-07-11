using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;

public class EnemyNotMoveTileController : SingletonMono<EnemyNotMoveTileController>
{
    public List<Vector2Int> enemyNotMovePosList;

    public void SetEnemyNotMovePosList(List<Vector2Int> enemyNotMovePosList)
    {
        this.enemyNotMovePosList = new List<Vector2Int>(enemyNotMovePosList);
    }

    public void CheckDefeatEnemy(Vector2Int weaponPos)
    {
        if (this.enemyNotMovePosList.Contains(weaponPos))
        {
            //Debug.Log("Defeat");
            this.enemyNotMovePosList.Remove(weaponPos);
            SlideController.Instance.DefeatEnemyNotMove(weaponPos);
        }
    }
}
