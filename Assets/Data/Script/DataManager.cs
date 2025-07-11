using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;

public class DataManager : SingletonMono<DataManager>
{
    [Header(" Level Data ")]
    public LevelSO LevelData;
    public ItemSO ItemData;
    public BlockSO BlockData;
    public EnemyNotMoveSO EnemyNotMoveData;
}
