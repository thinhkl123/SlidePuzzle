using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemSO", fileName = "ItemData")]
public class ItemSO : ScriptableObject
{
    public List<ItemDetail> ItemDetails;
}

[Serializable]
public class ItemDetail
{
    public int ItemId;
    public List<Vector2Int> WeaponPosList;
    public List<Vector2Int> KeyPosList;
    public List<KeyType> KeyTypeList;
}

[Serializable]
public enum KeyType
{
    None = 0,
    Recyle = 1,
    Organic = 2,
    Poision = 3,
}
