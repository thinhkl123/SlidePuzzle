using CustomUtils;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterHuntBossController : SingletonMono<WaterHuntBossController>
{
    [Header("Water Hunt Boss")]
    public WaterHuntBossView WaterHuntBossPrefab;
    public Vector2Int WaterHuntBossPos; 
    public WaterHuntBossModel WaterHuntBoss;
    private int _index = -1;

    [Header(" Water ")]
    public GameObject WaterPrefab;
    public Vector2Int WaterPos;

    private GameObject _curWater;
    private WaterHuntBossSO _waterHuntBossData;
    private Vector2Int _dataWaterPos;

    public Vector2Int NewPosForBoss = new Vector2Int(-1, -1);

    public void SetWaterHuntBoss(int index)
    {
        if (index - 1 < 0)
        {
            this._index = -1;
            return;
        }
        this._index = index - 1;
        this._waterHuntBossData = DataManager.Instance.WaterHuntBossData;
        this.WaterHuntBossPos = _waterHuntBossData.WaterHuntBossList[_index].BossSpawnPos;
        this.SpawnWaterHuntBoss();

        this._dataWaterPos = _waterHuntBossData.WaterHuntBossList[_index].WaterSpawnPos;

        this.SpawnWater();
        this.NewPosForBoss = this.WaterPos;
    }

    private void SpawnWaterHuntBoss()
    {
        Vector3Int initWaterHuntBossPos = new Vector3Int(WaterHuntBossPos.x, WaterHuntBossPos.y, 0);
        Tilemap groundTilemap = SlideController.Instance.groundTilemap;
        WaterHuntBossView waterHuntBossView = Instantiate(WaterHuntBossPrefab, groundTilemap.CellToWorld(initWaterHuntBossPos) + groundTilemap.cellSize / 2, Quaternion.identity, this.transform);
        waterHuntBossView.name = "WaterHuntBoss";
        WaterHuntBoss = new WaterHuntBossModel();
        WaterHuntBoss.SetView(waterHuntBossView);
    }

    public void SpawnWater(bool plusPos = false)
    {
        Vector2Int newPos = new Vector2Int(this._dataWaterPos.x, this._dataWaterPos.y);

        if (plusPos)
        {
            newPos = this._waterHuntBossData.WaterHuntBossList[_index].WaterSpawnPos2;
        }
        Vector3Int initWaterPos = new Vector3Int(newPos.x, newPos.y, 0);
        this.WaterPos = newPos;
        Tilemap groundTilemap = SlideController.Instance.groundTilemap;
        _curWater = Instantiate(WaterPrefab, groundTilemap.CellToWorld(initWaterPos) + groundTilemap.cellSize / 2, Quaternion.identity, this.transform);
    } 

    public void MoveWater(Vector2Int newGridPos, Vector3 worldPos)
    {
        this.WaterPos = newGridPos;
        this._curWater.transform.DOMove(worldPos, 0.25f).SetEase(Ease.InOutSine);
    }

    public void TeleWater(Vector2Int newGridPos, Vector3 worldPos)
    {
        this._curWater.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            this._curWater.transform.position = worldPos;
            this.WaterPos = newGridPos;
            this._curWater.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
        });
    }

    public void HideWater(Player player)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(this._curWater.gameObject.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        seq.Join(this._curWater.gameObject.transform.DORotate(new Vector3(0, 0, 360f), 0.2f, RotateMode.FastBeyond360));
        seq.OnComplete(() => 
        {
            Destroy(this._curWater.gameObject);
            if (player.GetCurrentPos() == this._dataWaterPos || this.WaterHuntBossPos == this._dataWaterPos)
            {
                this.SpawnWater(true);
                return;
            }
            else
            {
                this.SpawnWater();
            }
        });
    }

    public bool CheckAndMoveWater(Player player, List<Vector2Int> cellPosList, Direction direction)
    {
        Vector2Int offset = new Vector2Int(0, 0);
        switch (direction)
        {
            case Direction.Left:
                offset = new Vector2Int(-1, 0);
                break;
            case Direction.Right:
                offset = new Vector2Int(1, 0);
                break;
            case Direction.Up:
                offset = new Vector2Int(0, 1);
                break;
            case Direction.Down:
                offset = new Vector2Int(0, -1);
                break;
        }

        Vector2Int oldPlayerPos = player.GetCurrentPos();
        Vector2Int waterPos = WaterPos;
        bool check = false;

        if (((direction == Direction.Left || direction == Direction.Right) &&
            oldPlayerPos.y == waterPos.y) ||
            ((direction == Direction.Up || direction == Direction.Down) &&
            oldPlayerPos.x == waterPos.x))
        {
            check = true;
        }
        if (!check) 
        { 
            this.NewPosForBoss = this.WaterPos;
            return true;
        }
        bool isTeleport = false;
        if (cellPosList[0] == waterPos)
        {
            waterPos = cellPosList[cellPosList.Count - 1];
            isTeleport = true;
        }
        else
        {
            waterPos += offset;
        }

        Vector3 worldPos = SlideController.Instance.groundTilemap.GetCellCenterWorld(
                            new Vector3Int(waterPos.x, waterPos.y, 0));
        if (!this.CheckCanMoveWater(new Vector3Int(waterPos.x, waterPos.y, 0)))
        {
            return false;
        }

        this.NewPosForBoss = this.WaterPos;

        if (!isTeleport)
        {
            this.MoveWater(waterPos, worldPos);
        }
        else
        {
            this.TeleWater(waterPos, worldPos);
        }
        return true;
    }

    public bool CheckCanMoveWater(Vector3Int newPos)
    {
        Vector2Int newWaterPos = new Vector2Int(newPos.x, newPos.y);
        if (SlideController.Instance.obstacleTilemap.HasTile(newPos) || 
            (newWaterPos == WaterHuntBossPos && WaterHuntBoss.Health > 0))
        {
            return false;
        }
        return true;
    }

    public bool CheckMoveForBoss(Vector2Int waterPos, Player player)
    {
        if (this._index < 0 || this.WaterHuntBoss.Health <=0)         
        {
            return false;
        }

        Vector2Int offset = new Vector2Int(0, 0);
        offset = new Vector2Int(-1, 0);
        if (waterPos == player.GetCurrentPos()) return false;

        if (waterPos == WaterHuntBossPos + offset)
        {
            return true;
        }
        offset = new Vector2Int(1, 0);
        if (waterPos == WaterHuntBossPos + offset)
        {
            return true;
        }
        offset = new Vector2Int(0, -1);
        if (waterPos == WaterHuntBossPos + offset)
        {
            return true;
        }
        offset = new Vector2Int(0, 1);
        if (waterPos == WaterHuntBossPos + offset)
        {
            return true;
        }
        return false;
    }

    public void MoveWaterHuntBoss(Player player)
    {
        this.WaterHuntBossPos = this.NewPosForBoss;
        Vector3Int posCheckTrap = new Vector3Int(WaterHuntBossPos.x, WaterHuntBossPos.y, 0);
        this.WaterHuntBoss.WaterHuntBossView.Move(this.WaterHuntBossPos);
        if (SlideController.Instance.trapForWaterHuntTilemap.HasTile(posCheckTrap))
        {
            SlideController.Instance.trapForWaterHuntTilemap.SetTile(posCheckTrap, null);
            this.WaterHuntBoss.TakeDamage(1);
        }
        
        if (WaterHuntBoss.Health > 0 && 
            WaterHuntBossPos == this.WaterPos)
        {
            this.HideWater(player);
        }
    }

    public void Win()
    {
        Debug.Log("Win Water Hunt Boss");
    }
}
