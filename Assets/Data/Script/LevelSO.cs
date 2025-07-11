using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelSO", fileName = "LevelData")]
public class LevelSO : ScriptableObject
{
    public List<LevelDetail> LevelDetails;
}

[Serializable]
public class LevelDetail
{
    public int LevelId;
    public Vector2Int PlayerPosition;
    public int ItemId;
    public int EnemyNotMoveId;
}
