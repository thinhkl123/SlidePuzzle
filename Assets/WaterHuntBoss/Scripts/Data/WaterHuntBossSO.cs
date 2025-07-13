using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WaterHuntBossSO", fileName = "WaterHuntBossData")]
public class WaterHuntBossSO : ScriptableObject
{
    public List<WaterHuntBossDetail> WaterHuntBossList;
}

[Serializable]
public class Water
{
    public Vector2Int WaterPos;
}

[Serializable]
public class WaterHuntBossDetail
{
    public int WaterHuntBossId;
    public Vector2Int WaterHuntBossPos;
    public List<Water> WaterList;
}