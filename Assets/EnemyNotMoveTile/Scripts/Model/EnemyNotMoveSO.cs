using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyNotMoveSO", fileName = "EnemyNotMoveData")]
public class EnemyNotMoveSO : ScriptableObject
{
    List<EnemyNotMoveDetail> EnemyNotMoveDetails;
}

[Serializable]
public class EnemyNotMoveDetail
{
    public int enemyNotMoveId;
    public List<Vector2Int> enemyNotMovePosList;
}
