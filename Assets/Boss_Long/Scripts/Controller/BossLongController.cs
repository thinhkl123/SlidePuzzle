using UnityEngine;
using CustomUtils;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class BossLongController : SingletonMono<BossLongController>
{
    [SerializeField] private TileFake tileFakePrefab;
    public List<TileFake> objList;
    public int length;
    public List<Vector2Int> posList;
    public int maxLength;
    public int phase;

    public void UpdatePosList(Vector2Int newHeadPos, bool isDecrease)
    {
        if (isDecrease)
        {
            for (int i = 0; i < posList.Count - 1; i++)
            {
                posList[i] = posList[i + 1];
            }

            posList.RemoveAt(posList.Count - 1);
            length--;

            return;
        }

        Vector2Int tailPos = posList[posList.Count - 1];

        posList.RemoveAt(posList.Count - 1);

        posList.Insert(0,newHeadPos);

        if (!isDecrease)
        {
            if (length < maxLength - 1)
            {
                SlideController.Instance.SpawnBossLongTile(DataManager.Instance.BossLongData.bodyTile, new Vector3Int(tailPos.x, tailPos.y, 0));
                posList.Add(tailPos);
                length++;
            }
            else if (length == maxLength - 1)
            {
                SlideController.Instance.SpawnBossLongTile(DataManager.Instance.BossLongData.tailTile, new Vector3Int(tailPos.x, tailPos.y, 0));
                posList.Add(tailPos);
                length++;
            }
        } 
    }

    public void Move(Vector2Int newHeadPos)
    {
        //Move Head

    }

    public bool IsFitLength()
    {
        return length == maxLength;
    }

    public void NextPhase(Vector2Int newHeadPos)
    {
        phase++;

        if (phase > DataManager.Instance.BossLongData.pahseCount)
        {
            Debug.Log("Win Boss Long");
            return;
        }

        posList.Clear();
        posList.Add(newHeadPos);

        length = 1;        
        maxLength = DataManager.Instance.BossLongData.lengthEachPhase[phase - 1];
    }

    public void Init(Vector2Int initBossPos)
    {
        this.length = 1;
        this.posList = new List<Vector2Int>
        {
            initBossPos
        };
        this.phase = 1;
        this.maxLength = DataManager.Instance.BossLongData.lengthEachPhase[0];

        Vector3Int initGridPos = new Vector3Int(initBossPos.x, initBossPos.y, 0);
        //TileFake obj = Instantiate(tileFakePrefab, SlideController.Instance.bossLongTilemap.GetCellCenterWorld(initGridPos), Quaternion.identity);
        //obj.SetSprite(DataManager.Instance.BossLongData.headSprite);
        //objList.Add(obj);
    }
}
