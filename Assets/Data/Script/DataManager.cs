using UnityEngine;
using CustomUtils;

public class DataManager : SingletonMono<DataManager>
{
    [Header(" Level Data ")]
    public LevelSO LevelData;
    public ItemSO ItemData;
    public EnemyNotMoveSO EnemyNotMoveData;
    public PuzzleSortLevelSO PuzzleSortLevelData;
    public WaterHuntBossSO WaterHuntBossData;
    public LazeSO LazeData;
}
